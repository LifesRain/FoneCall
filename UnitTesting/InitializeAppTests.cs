using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

using Dapper;

using FoneCall;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize]
    public class InitializeAppTests
    {
        private string _tempDir = string.Empty;
        private string _dbPath { get; set; } = string.Empty;
        private string _logPath = string.Empty;

        [TestInitialize]
        public void TestInitialize()
        {
            // Create an isolated temp folder for this specific test run
            _tempDir = Path.Combine( Path.GetTempPath(), "FoneCallTest_" + Guid.NewGuid().ToString( "N" ) );
            Directory.CreateDirectory( _tempDir );

            _dbPath = Path.Combine( _tempDir, "FoneCall.db" );
            _logPath = Path.Combine( _tempDir, "FoneCall.log" );

            // Create an empty sqlite DB and required tables
            DBC.InitializeTestDatabase( _dbPath );
            // Force the DBC context to point directly to your test file
            DBC.ConnectionStringOverride = $"Data Source={_dbPath};Version=3;";

            // relies on
            // [assembly: System.Runtime.CompilerServices.InternalsVisibleTo( "UnitTesting" )]
            // in AssemblyInfo.cs
            Paths.AppData = _tempDir + Path.DirectorySeparatorChar;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DBC.ConnectionStringOverride = null;

            try
            {
                if (Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
            }
            catch { /* best-effort cleanup */ }
        }

        [TestMethod]
        public void SaveOptsToDB_InsertsArguments_WhenHelpFileContainsOptions()
        {
            // Arrange - Create the help file inside the dynamic temp environment
            string helpDir = Path.Combine( _tempDir, "Data" );
            Directory.CreateDirectory( helpDir );

            string helpFile = Path.Combine(
                helpDir,
                "scrcpy_Options.txt" );

            var content = string.Join(
                Environment.NewLine,
                new[]
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
                // Verify against the dynamically overridden path structures
                Console.WriteLine( $"Paths.AppLog      = {Paths.AppLog}" );
                Console.WriteLine( $"Paths.Options     = {Paths.Options}" );
                Console.WriteLine( $"Paths.AppData     = {Paths.AppData}" );
                Console.WriteLine( $"Paths.SolutionData = {Paths.SolutionData}" );

                Assert.AreEqual(
                    Path.Combine( Paths.SolutionData, "scrcpy_Options.txt" ),
                    Paths.Options,
                    "Paths.Options should resolve to the solution Data directory." );

                Assert.AreNotEqual(
                    Paths.AppLog,
                    Paths.Options,
                    "Paths.AppLog must not point to the options file." );

                // Act
                InitializeApp.SaveOptsToDB( testHelpFQN: helpFile );

                string sql =
                    "SELECT Role, Option, Desc " +
                    "FROM Options " +
                    "ORDER BY Option";

                List<Options> testResult;

                using (var dbc = new SQLiteConnection(
                    $"Data Source={_dbPath};Version=3;" ))
                {
                    testResult = dbc.Query<Options>( sql ).ToList();
                }

                Assert.IsTrue(
                    testResult.Count >= 2,
                    "Expected at least two option rows inserted." );

                Assert.IsTrue(
                    testResult.Any( x =>
                        !string.IsNullOrEmpty( x.Option ) &&
                        x.Option.Contains( "-option1" ) ),
                    "option1 header was not saved." );

                Assert.IsTrue(
                    testResult.Any( x =>
                        !string.IsNullOrEmpty( x.Option ) &&
                        x.Option.Contains( "-option2" ) ),
                    "option2 header was not saved." );
            }
            finally
            {
                try
                {
                    File.Delete( helpFile );
                }
                catch
                {
                    // Best-effort cleanup.
                }
            }
        }

        [TestMethod]
        public void SaveOptsToDB_NoInserts_WhenHelpFileMissingOrEmpty()
        {
            // Arrange - Ensure help file does not exist in the environment path context
            string helpDir = Path.Combine( _tempDir, "Data" );
            string helpFile = Path.Combine( helpDir, "scrcpy help.txt" );
            try { if (File.Exists( helpFile )) File.Delete( helpFile ); } catch { }

            // Act & Assert
            InitializeApp.SaveOptsToDB( testHelpFQN: helpFile );

            using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Options";
                    var count = Convert.ToInt32( cmd.ExecuteScalar() );
                    Assert.AreEqual( 0, count, "Args table should be empty when no help file present." );
                }
            }
        }

        [TestMethod]
        public void SaveRTFtoDB_SavesIntroToKVP()
        {
            // Arrange - Create the RTF file location dynamically inside the tracking root
            string docsDir = Path.Combine( _tempDir, "__DOCS" );
            Directory.CreateDirectory( docsDir );
            string rtfFile = Path.Combine( docsDir, "FoneCallWelcome.rtf" );
            string rtfContent = "This is a test welcome RTF with a single quote: ' and some text.";
            File.WriteAllText( rtfFile, rtfContent );

            try
            {
                // Act
                InitializeApp.SaveRTFtoDB( rtfFile );

                // Assert
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
    }
}
