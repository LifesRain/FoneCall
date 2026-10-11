using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using FoneCall;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class RunOptMngrTests
    {
        private string _tempDir = string.Empty;
        private string _dbPath = string.Empty;
        private string _logPath = string.Empty;
        private string? _originalAppData = string.Empty;

        [TestInitialize]
        public void TestInitialize()
        {
            _tempDir = Path.Combine( Path.GetTempPath(), "FoneCallTest_" + Guid.NewGuid().ToString( "N" ) );
            Directory.CreateDirectory( _tempDir );

            _dbPath = Path.Combine( _tempDir, "FoneCall.db" );
            _logPath = Path.Combine( _tempDir, "FoneCall.log" );

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

            // Backup original AppData backing field only
            _originalAppData = GetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField" ) as string;

            // Override Paths.AppData to point to our temp folder (AppDB/AppLog are computed)
            SetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField", _tempDir + Path.DirectorySeparatorChar );
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (_originalAppData != null)
                SetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField", _originalAppData );

            try { if (Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true ); } catch { }
        }

        // STA runner helper
        private static T? RunInSta<T>( Func<T> func )
        {
            T? result = default;
            Exception? exception = null;
            var thread = new Thread( () =>
            {
                try { result = func(); }
                catch (Exception ex) { exception = ex; }
                Application.DoEvents();
            } );
            thread.SetApartmentState( ApartmentState.STA );
            thread.Start();
            thread.Join();
            if (exception != null) throw new AggregateException( "Exception in STA thread", exception );
            return result;
        }

        [TestMethod]
        public void Constructor_InitializesControls_NoThrow()
        {
            RunInSta<object?>( () =>
            {
                var form = new RunOptMngr();
                try
                {
                    var t = form.GetType();
                    var dgvField = t.GetField( "DGV", BindingFlags.Instance | BindingFlags.NonPublic );
                    var saveField = t.GetField( "SaveBTN", BindingFlags.Instance | BindingFlags.NonPublic );
                    var closeField = t.GetField( "CloseBTN", BindingFlags.Instance | BindingFlags.NonPublic );

                    Assert.IsNotNull( dgvField );
                    Assert.IsNotNull( saveField );
                    Assert.IsNotNull( closeField );

                    var dgv = dgvField.GetValue( form ) as DataGridView;
                    var save = saveField.GetValue( form ) as Button;
                    var close = closeField.GetValue( form ) as Button;

                    Assert.IsNotNull( dgv );
                    Assert.IsNotNull( save );
                    Assert.IsNotNull( close );
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        [TestMethod]
        public void SaveBTN_Click_SavesRowsToDB()
        {
            RunInSta<object?>( () =>
            {
                var form = new RunOptMngr();
                try
                {
                    var t = form.GetType();
                    var dgv = t.GetField( "DGV", BindingFlags.Instance | BindingFlags.NonPublic )?.GetValue( form ) as DataGridView;
                    Assert.IsNotNull( dgv );

                    // Ensure DB empty
                    using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
                    {
                        cn.Open();
                        using (var cmd = cn.CreateCommand()) { cmd.CommandText = "DELETE FROM Args"; cmd.ExecuteNonQuery(); }
                    }

                    // Add one valid row and one empty row (which should be skipped)
                    dgv.Rows.Add( new object[] { "001", "R1", "--opt1", "explain1" } );
                    dgv.Rows.Add( new object?[] { "002", null, null, null } );

                    var saveMethod = t.GetMethod( "SaveBTN_Click", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( saveMethod );
                    var saveBtn = t.GetField( "SaveBTN", BindingFlags.Instance | BindingFlags.NonPublic )?.GetValue( form ) as Button;

                    Assert.IsNotNull( saveBtn );
                    saveMethod.Invoke( form, new object[] { saveBtn, EventArgs.Empty } );

                    // Verify DB contains the single saved arg
                    using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
                    {
                        cn.Open();
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT Role,Arg,Explain FROM Args";
                            using (var rdr = cmd.ExecuteReader())
                            {
                                int rows = 0; bool found = false;
                                while (rdr.Read())
                                {
                                    rows++;
                                    var role = rdr["Role"] as string;
                                    var arg = rdr["Arg"] as string;
                                    if (arg == "--opt1" && role == "R1") found = true;
                                }
                                Assert.AreEqual( 1, rows );
                                Assert.IsTrue( found );
                            }
                        }
                    }
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        [TestMethod]
        public void PopulateDGVs_NoArgs_LeavesZeroRows()
        {
            RunInSta<object?>( () =>
            {
                var form = new RunOptMngr();
                try
                {
                    var t = form.GetType();
                    var pop = t.GetMethod( "PopulateDGVs", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( pop );

                    pop.Invoke( form, null );

                    var dgv = t.GetField( "DGV", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( form ) as DataGridView;
                    Assert.IsNotNull( dgv );

                    // Count only actual data rows (exclude the DataGridView "new row")
                    var actualDataRowCount = dgv.Rows.Cast<DataGridViewRow>().Count( r => !r.IsNewRow );
                    Assert.AreEqual( 0, actualDataRowCount );
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        [TestMethod]
        public void PopulateDGVs_WithArgs_PopulatesRowsAndHandlesNulls()
        {
            RunInSta<object?>( () =>
            {
                // insert two Arg rows, one with null Role
                using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Args (Role,Arg,Explain) VALUES (@r,@a,@e)";
                        var p1 = cmd.CreateParameter(); p1.ParameterName = "@r"; p1.Value = "R1"; cmd.Parameters.Add( p1 );
                        var p2 = cmd.CreateParameter(); p2.ParameterName = "@a"; p2.Value = "--opt1"; cmd.Parameters.Add( p2 );
                        var p3 = cmd.CreateParameter(); p3.ParameterName = "@e"; p3.Value = "explain1"; cmd.Parameters.Add( p3 );
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        cmd.CommandText = "INSERT INTO Args (Role,Arg,Explain) VALUES (@r,@a,@e)";
                        var q1 = cmd.CreateParameter(); q1.ParameterName = "@r"; q1.Value = DBNull.Value; cmd.Parameters.Add( q1 );
                        var q2 = cmd.CreateParameter(); q2.ParameterName = "@a"; q2.Value = "--opt2"; cmd.Parameters.Add( q2 );
                        var q3 = cmd.CreateParameter(); q3.ParameterName = "@e"; q3.Value = DBNull.Value; cmd.Parameters.Add( q3 );
                        cmd.ExecuteNonQuery();
                    }
                }

                var form = new RunOptMngr();
                try
                {
                    var t = form.GetType();
                    var pop = t.GetMethod( "PopulateDGVs", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( pop );

                    pop.Invoke( form, null );

                    var dgv = t.GetField( "DGV", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( form ) as DataGridView;
                    Assert.IsNotNull( dgv );
                    var actualDataRowCount = dgv.Rows.Cast<DataGridViewRow>().Count( r => !r.IsNewRow );
                    Assert.AreEqual( 2, actualDataRowCount );
                    // first row checks
                    var row0 = dgv.Rows[0];
                    Assert.IsTrue( row0.Cells[0].Value.ToString().EndsWith( "1" ) );
                    Assert.AreEqual( "R1", row0.Cells[1].Value );
                    Assert.AreEqual( "--opt1", row0.Cells[2].Value );

                    // second row checks (null role/explain should be empty string)
                    var row1 = dgv.Rows[1];
                    Assert.IsNotNull( row1.Cells[1].Value );
                    Assert.AreEqual( string.Empty, row1.Cells[1].Value );
                    Assert.AreEqual( "--opt2", row1.Cells[2].Value );
                    Assert.AreEqual( string.Empty, row1.Cells[3].Value );
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        [TestMethod]
        public void DGV_Edit_Copy_CopiesSelectedRowsToClipboard()
        {
            RunInSta<object?>( () =>
            {
                // insert one Arg row
                using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Args (Role,Arg,Explain) VALUES ('R1','--opt1','explain1')";
                        cmd.ExecuteNonQuery();
                    }
                }

                var form = new RunOptMngr();
                try
                {
                    var t = form.GetType();
                    var pop = t.GetMethod( "PopulateDGVs", BindingFlags.Instance | BindingFlags.NonPublic );
                    pop.Invoke( form, null );

                    var dgv = t.GetField( "DGV", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( form ) as DataGridView;
                    Assert.IsNotNull( dgv );

                    // select first row
                    dgv.Rows[0].Selected = true;

                    // call DGV_Edit(copy:true,paste:false)
                    var edit = t.GetMethod( "DGV_Edit", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( edit );
                    edit.Invoke( form, new object[] { true, false } );

                    // check clipboard contains the row number text
                    string clip = Clipboard.GetText();
                    Assert.IsTrue( !string.IsNullOrEmpty( clip ) && clip.Contains( "1" ) );
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        // Reflection helpers
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
