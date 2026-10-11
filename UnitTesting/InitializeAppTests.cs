using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

using Dapper;
using FoneCall;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class InitializeAppTests
    {
        private string _tempDir = string.Empty;
        private string _dbPath = string.Empty;
        private string _logPath = string.Empty;
        private string? _originalAppDB = string.Empty;
        private string? _originalAppLog = string.Empty;
        private string? _originalAppData = string.Empty;

        [TestInitialize]
        public void TestInitialize()
        {
            // create isolated temp folder for DB and logs
            _tempDir = Path.Combine( Path.GetTempPath(), "FoneCallTest_" + Guid.NewGuid().ToString( "N" ) );
            Directory.CreateDirectory( _tempDir );

            _dbPath = Path.Combine( _tempDir, "FoneCall.db" );
            _logPath = Path.Combine( _tempDir, "FoneCall.log" );

            // create an empty sqlite DB and required tables
            SQLiteConnection.CreateFile( _dbPath );
            using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Args (
                            RecID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Role TEXT,
                            Arg TEXT,
                            Explain TEXT
                        );
                        CREATE TABLE IF NOT EXISTS KVP (
                            KVP_key TEXT PRIMARY KEY,
                            KVP_valu TEXT
                        );
                        CREATE TABLE IF NOT EXISTS ExeSpec (
                            RecID TEXT PRIMARY KEY,
                            ScrcpyFQN TEXT,
                            Arguments TEXT
                        );
                        CREATE TABLE IF NOT EXISTS StartPos (
                            RecID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ScreenName TEXT,
                            FormName TEXT,
                            Form_L INTEGER,
                            Form_T INTEGER
                        );
                        ";
                    cmd.ExecuteNonQuery();
                }
            }

            // Backup current values:
            // AppData backing field exists and must be changed via reflection.
            // AppDB and AppLog are computed read-only properties -> read them directly.
            _originalAppData = GetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField" ) as string;
            _originalAppDB = Paths.AppDB;
            _originalAppLog = Paths.AppLog;

            // Override only Paths.AppData backing field so computed AppDB/AppLog reflect our temp folder.
            SetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField", _tempDir + Path.DirectorySeparatorChar );
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Restore only AppData backing field. AppDB/AppLog are computed from AppData.
            if (_originalAppData != null)
                SetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField", _originalAppData );

            try
            {
                if (Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
            }
            catch { /* best-effort cleanup */ }
        }

        [TestMethod]
        public void SaveOptsToDB_InsertsArguments_WhenHelpFileContainsOptions()
        {
            // Arrange - create the help file at the path used in InitializeApp.SaveOptsToDB
            string helpDir = Path.Combine( "E:\\FoneCall\\Data" );
            Directory.CreateDirectory( helpDir );
            string helpFile = Path.Combine( helpDir, "scrcpy help.txt" );

            var content = string.Join( Environment.NewLine, new[]
            {
                "Options",
                "    -option1   Description line 1",
                "       continued detail for option1",
                "    -option2   Description option2",
                "Shortcuts"
            } );
            File.WriteAllText( helpFile, content );

            try
            {
                // quick sanity checks before calling SaveOptsToDB()
                string expectedHelp = Path.Combine("E:\\FoneCall\\Data", "scrcpy help.txt");

                Console.WriteLine($"Paths.AppLog = {Paths.AppLog}");
                Console.WriteLine($"Paths.Help   = {Paths.Help}");
                Console.WriteLine($"Paths.AppData= {Paths.AppData}");

                // Ensure the app will read the help file you wrote
                Assert.AreEqual(expectedHelp, Paths.Help, "Paths.Help should point to the help file created by the test.");

                // Ensure the app log is not the same file as the help (prevents log-trimmer deleting your help file)
                Assert.AreNotEqual(Paths.AppLog, Paths.Help, "Paths.AppLog must not point to the help file.");

                // Act
                InitializeApp.SaveOptsToDB();

                // Assert - read Args table to confirm entries were inserted
                using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Role, Arg, Explain FROM Args";
                        using (var rdr = cmd.ExecuteReader())
                        {
                            int rows = 0;
                            bool hasOption1 = false;
                            bool hasOption2 = false;
                            while (rdr.Read())
                            {
                                rows++;
                                var arg = rdr["Arg"] as string;
                                if (arg != null && arg.Contains( "-option1" )) hasOption1 = true;
                                if (arg != null && arg.Contains( "-option2" )) hasOption2 = true;
                            }
                            Assert.IsTrue( rows >= 2, "Expected at least two Arg rows inserted." );
                            Assert.IsTrue( hasOption1, "option1 header not saved." );
                            Assert.IsTrue( hasOption2, "option2 header not saved." );
                        }
                    }
                }
            }
            finally
            {
                // cleanup created file
                try { File.Delete( helpFile ); } catch { }
            }
        }

        [TestMethod]
        public void SaveOptsToDB_NoInserts_WhenHelpFileMissingOrEmpty()
        {
            // Arrange - ensure help file does not exist
            string helpFile = @"E:\FoneCall\Data\scrcpy help.txt";
            try { if (File.Exists( helpFile )) File.Delete( helpFile ); } catch { }

            // Act & Assert - method should not throw and DB remains empty
            InitializeApp.SaveOptsToDB();

            using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Args";
                    var count = Convert.ToInt32( cmd.ExecuteScalar() );
                    Assert.AreEqual( 0, count, "Args table should be empty when no help file present." );
                }
            }
        }

        [TestMethod]
        public void SaveRTFtoDB_SavesIntroToKVP()
        {
            // Arrange - create the RTF file path used by SaveRTFtoDB
            string docsDir = @"E:\__DOCS";
            Directory.CreateDirectory( docsDir );
            string rtfFile = Path.Combine( docsDir, "FoneCallWelcome.rtf" );
            string rtfContent = "This is a test welcome RTF with a single quote: ' and some text.";
            File.WriteAllText( rtfFile, rtfContent );

            try
            {
                // Act
                InitializeApp.SaveRTFtoDB();

                // Assert - KVP table should contain Intro key with (processed) content
                using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT KVP_valu FROM KVP WHERE KVP_key = @k";
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@k";
                        p.Value = KVP_Keys.Intro;
                        cmd.Parameters.Add( p );

                        var val = cmd.ExecuteScalar() as string;
                        Assert.IsNotNull( val, "Intro KVP should exist after SaveRTFtoDB." );
                        Assert.IsTrue( val.Contains( "test welcome" ), "Saved value should contain processed RTF text." );
                        Assert.IsFalse( val.Contains( "\0" ), "Non-printable characters should be removed." );
                    }
                }
            }
            finally
            {
                try { File.Delete( rtfFile ); } catch { }
            }
        }

        // Helper: reflection helpers to set/get readonly auto-property backing fields
        private object? GetBackingFieldValue( Type t, string fieldName )
        {
            var fi = t.GetField( fieldName, BindingFlags.Static | BindingFlags.NonPublic );
            return fi?.GetValue( null );
        }

        private void SetBackingFieldValue( Type t, string fieldName, object value )
        {
            var fi = t.GetField( fieldName, BindingFlags.Static | BindingFlags.NonPublic );
            if (fi == null) Assert.Inconclusive( $"Could not find backing field '{fieldName}' on type {t.FullName}." );
            fi.SetValue( null, value );
        }
    }
}