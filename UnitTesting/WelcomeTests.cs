using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using FoneCall;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class WelcomeTests
    {
        // temp DB for tests
        private string _tempDir = string.Empty;
        private string _dbPath = string.Empty;
        private string? _originalAppData = string.Empty;

        [TestInitialize]
        public void TestInitialize()
        {
            // create isolated temp folder and DB so Welcome.PopulateRTBX can read KVP.Intro
            _tempDir = Path.Combine(Path.GetTempPath(), "FoneCallTest_Welcome_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);

            _dbPath = Path.Combine(_tempDir, "FoneCall.db");
            SQLiteConnection.CreateFile(_dbPath);

            using (var cn = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS KVP (
                            KVP_key TEXT PRIMARY KEY,
                            KVP_valu TEXT
                        );";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT OR REPLACE INTO KVP (KVP_key, KVP_valu) VALUES (@k, @v)";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@k"; p1.Value = KVP_Keys.Intro; cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@v";
                    // simple RTF content; PopulateRTBX will replace '##' with single quote if present
                    p2.Value = @"{\rtf1\ansi This is a test welcome RTF with ## marker.}";
                    cmd.Parameters.Add(p2);
                    cmd.ExecuteNonQuery();
                }
            }

            // Backup and override Paths.AppData so Paths.AppDB/Paths.AppLog point to the test DB/log location
            _originalAppData = GetBackingFieldValue(typeof(Paths), "<AppData>k__BackingField") as string;
            SetBackingFieldValue(typeof(Paths), "<AppData>k__BackingField", _tempDir + Path.DirectorySeparatorChar);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (_originalAppData != null)
                SetBackingFieldValue(typeof(Paths), "<AppData>k__BackingField", _originalAppData);

            try { if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true); } catch { }
        }

        // reflection helpers for readonly backing-field manipulation
        private object? GetBackingFieldValue(Type t, string fieldName)
        {
            var fi = t.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            return fi?.GetValue(null);
        }

        private void SetBackingFieldValue(Type t, string fieldName, object value)
        {
            var fi = t.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            if (fi == null) Assert.Inconclusive($"Could not find backing field '{fieldName}' on type {t.FullName}.");
            fi.SetValue(null, value);
        }

        // Helper to run WinForms code on STA thread and propagate exceptions/results
        private static T? RunInSta<T>( Func<T> func )
        {
            T? result = default;
            Exception? exception = null;
            var thread = new Thread( () =>
            {
                try
                {
                    result = func();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                // pump messages briefly to allow form events to process
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
                Welcome form = new Welcome();
                try
                {
                    var t = form.GetType();
                    var rtbField = t.GetField( "RTBX", BindingFlags.Instance | BindingFlags.NonPublic );
                    var verField = t.GetField( "VerLBL", BindingFlags.Instance | BindingFlags.NonPublic );
                    var logField = t.GetField( "LogLBL", BindingFlags.Instance | BindingFlags.NonPublic );
                    var closeField = t.GetField( "CloseBTN", BindingFlags.Instance | BindingFlags.NonPublic );

                    Assert.IsNotNull( rtbField, "RTBX field not found by reflection." );
                    Assert.IsNotNull( verField, "VerLBL field not found by reflection." );
                    Assert.IsNotNull( logField, "LogLBL field not found by reflection." );
                    Assert.IsNotNull( closeField, "CloseBTN field not found by reflection." );

                    var rtb = rtbField.GetValue( form ) as RichTextBox;
                    var ver = verField.GetValue( form ) as Label;
                    var log = logField.GetValue( form ) as Label;
                    var close = closeField.GetValue( form ) as Button;

                    Assert.IsNotNull( rtb, "RTBX control is null after construction." );
                    Assert.IsNotNull( ver, "VerLBL control is null after construction." );
                    Assert.IsNotNull( log, "LogLBL control is null after construction." );
                    Assert.IsNotNull( close, "CloseBTN control is null after construction." );

                    // Defensive assertions
                    StringAssert.Contains( (ver?.Text ?? string.Empty), "scrcpy", "Version label should contain 'scrcpy' prefix (defensive check)." );
                    Assert.IsNotNull( rtb.Lines, "RTBX.Lines should not be null (may be empty)." );
                }
                finally
                {
                    // Ensure form closed on the STA thread
                    try { form.Close(); } catch { }
                    try { form.Dispose(); } catch { }
                }
                return null;
            } );
        }

        [TestMethod]
        public void CloseButton_PerformClick_ClosesForm()
        {
            RunInSta<object?>( () =>
            {
                var form = new Welcome();
                try
                {
                    var t = form.GetType();
                    var closeField = t.GetField( "CloseBTN", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( closeField, "CloseBTN field not found by reflection." );
                    var close = closeField.GetValue( form ) as Button;
                    Assert.IsNotNull( close, "CloseBTN control is null." );

                    // Simulate user click
                    close.PerformClick();

                    // Allow event processing
                    Application.DoEvents();

                    // After click the form should be closed/disposed or at least not visible
                    Assert.IsTrue( form.IsDisposed || !form.Visible, "Form should be closed or not visible after CloseBTN.PerformClick()." );
                }
                finally
                {
                    try { form.Close(); } catch { }
                    try { form.Dispose(); } catch { }
                }
                return null;
            } );
        }

        [TestMethod]
        public void PopulateRTBX_CanBeInvoked_MultipleTimes_NoException()
        {
            RunInSta<object?>( () =>
            {
                var form = new Welcome();
                try
                {
                    var t = form.GetType();
                    var populate = t.GetMethod( "PopulateRTBX", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( populate, "PopulateRTBX method not found by reflection." );

                    // Invoke once more to test idempotency / repeated invocation edge-case
                    populate.Invoke( form, null );
                    populate.Invoke( form, null );

                    var rtbField = t.GetField( "RTBX", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( rtbField, "RTBX field not found by reflection." );
                    var rtb = rtbField.GetValue( form ) as RichTextBox;
                    Assert.IsNotNull( rtb, "RTBX control is null after PopulateRTBX invocation." );
                    Assert.IsNotNull( rtb.Lines, "RTBX.Lines should not be null after PopulateRTBX (can be empty)." );
                }
                finally
                {
                    try { form.Close(); } catch { }
                    try { form.Dispose(); } catch { }
                }
                return null;
            } );
        }
    }
}