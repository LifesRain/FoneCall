using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using FoneCall;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class RunSCRCPYTests
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

            // Backup original AppData and computed AppDB/AppLog values
            _originalAppData = GetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField" ) as string;
            _originalAppDB = Paths.AppDB;   // computed property -> no backing field
            _originalAppLog = Paths.AppLog; // computed property -> no backing field

            // Override AppData so the computed AppDB/AppLog reflect our temp folder.
            SetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField", _tempDir + Path.DirectorySeparatorChar );
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (_originalAppData != null)
                SetBackingFieldValue( typeof( Paths ), "<AppData>k__BackingField", _originalAppData );

            // AppDB and AppLog are computed from AppData; restoring AppData is sufficient.
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
        public void DataReceived_IgnoresEmptyData()
        {
            RunInSta<object?>( () =>
            {
                var t = typeof( RunSCRCPY );
                // reset flags
                SetPrivateStaticField( t, "_hasProcErr", false );
                SetPrivateStaticField( t, "_nuProgMsg", false );

                var dreType = typeof( System.Diagnostics.DataReceivedEventArgs );
                // Fix for CS8625: Cannot convert null literal to non-nullable reference type.
                // The constructor for DataReceivedEventArgs expects a string? (nullable string) argument, but
                // in .NET Framework, the constructor is internal and expects a string. To avoid CS8625, explicitly pass null as string?.
                var dataEvent = Activator.CreateInstance(
                    dreType,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new object?[] { null },
                    null
                );
                var method = t.GetMethod( "DataReceived", BindingFlags.Static | BindingFlags.NonPublic );
                Assert.IsNotNull( method );
                // Fix for CS8625: Pass a non-null object for the sender parameter in method.Invoke
                method.Invoke( null, new object[] { new object(), dataEvent } );

                bool nuProg = GetPrivateStaticField( t, "_nuProgMsg" ) as bool? ?? false;
                bool hasErr = GetPrivateStaticField( t, "_hasProcErr" ) as bool? ?? false;
                Assert.IsFalse( nuProg );
                Assert.IsFalse( hasErr );
                return null;
            } );
        }

        [TestMethod]
        public void ExeSCRCPY_SimulateProcessWindow_ReturnsTrue_AndCleansUp()
        {
            RunInSta<object?>( () =>
            {
                var caller = new MsgBoxImplement();
                var form = new RunSCRCPY( caller, noDelay: true );
                try
                {
                    // prepare ExeSpec pointing to a GUI process (notepad) to simulate scrcpy creating a window
                    var notepad = Path.Combine( Environment.SystemDirectory, "notepad.exe" );
                    Assert.IsTrue( File.Exists( notepad ), "notepad.exe must exist on the test machine for this test." );

                    // insert ExeSpec record with RecID='GetMod' so DBC.GetExeSpec() will return it
                    using (var cn = new SQLiteConnection( $"Data Source={_dbPath};Version=3;" ))
                    {
                        cn.Open();
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = "DELETE FROM ExeSpec";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO ExeSpec (RecID,ScrcpyFQN,Arguments) VALUES (@r,@f,@a)";
                            var p1 = cmd.CreateParameter(); p1.ParameterName = "@r"; p1.Value = Xfer.DBaMod; cmd.Parameters.Add( p1 );
                            var p2 = cmd.CreateParameter(); p2.ParameterName = "@f"; p2.Value = notepad; cmd.Parameters.Add( p2 );
                            var p3 = cmd.CreateParameter(); p3.ParameterName = "@a"; p3.Value = ""; cmd.Parameters.Add( p3 );
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // capture existing notepad PIDs to avoid killing unrelated instances
                    var before = new HashSet<int>( Process.GetProcessesByName( "notepad" ).Select( p => p.Id ) );

                    var t = form.GetType();
                    var exeMethod = t.GetMethod( "ExeSCRCPY", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( exeMethod );

                    var task = (Task<bool>)exeMethod.Invoke( form, null );
                    Assert.IsNotNull( task );

                    // wait up to 10s for task completion
                    if (!task.Wait( 10000 )) Assert.Inconclusive( "ExeSCRCPY did not complete in time." );
                    bool result = task.Result;

                    // method should observe a GUI main window and return true for this simulation
                    Assert.IsTrue( result, "ExeSCRCPY should return true when a GUI process window appears." );

                    // cleanup: find any new notepad processes started and close them
                    var after = Process.GetProcessesByName( "notepad" );
                    foreach (var p in after)
                    {
                        if (!before.Contains( p.Id ))
                        {
                            try { p.CloseMainWindow(); if (!p.WaitForExit( 2000 )) p.Kill(); }
                            catch { try { p.Kill(); } catch { } }
                        }
                    }
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        [TestMethod]
        public void DataReceived_ParsesHttpAndSetsInitMsg()
        {
            RunInSta<object?>( () =>
            {
                var t = typeof( RunSCRCPY );
                SetPrivateStaticField( t, "_hasProcErr", false );
                SetPrivateStaticField( t, "_nuProgMsg", false );

                string sample = "some output <http://example.com>";
                var dreType = typeof( System.Diagnostics.DataReceivedEventArgs );
                var dataEvent = Activator.CreateInstance( dreType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { sample }, null );
                var method = t.GetMethod( "DataReceived", BindingFlags.Static | BindingFlags.NonPublic );
                Assert.IsNotNull( method );
                method.Invoke( null, new object[] { new object(), dataEvent } );

                bool nuProg = GetPrivateStaticField( t, "_nuProgMsg" ) as bool? ?? false;
                string? initMsg = GetPrivateStaticField( t, "_initMsg" ) as string;
                Assert.IsTrue( nuProg );
                Assert.IsNotNull( initMsg );
                Assert.Contains( "executable found", initMsg );
                return null;
            } );
        }

        [TestMethod]
        public void DataReceived_ErrorMessage_SetsHasProcErr()
        {
            RunInSta<object?>( () =>
            {
                var t = typeof( RunSCRCPY );
                SetPrivateStaticField( t, "_hasProcErr", false );
                SetPrivateStaticField( t, "_nuProgMsg", false );

                string sample = "ERROR: something went wrong";
                var dreType = typeof( System.Diagnostics.DataReceivedEventArgs );
                var dataEvent = Activator.CreateInstance( dreType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { sample }, null );
                var method = t.GetMethod( "DataReceived", BindingFlags.Static | BindingFlags.NonPublic );
                Assert.IsNotNull( method );
                method.Invoke( null, new object[] { new object(), dataEvent } );

                bool hasErr = GetPrivateStaticField( t, "_hasProcErr" ) as bool? ?? false;
                string? initMsg = GetPrivateStaticField( t, "_initMsg" ) as string;
                Assert.IsTrue( hasErr );
                Assert.Contains( "something went wrong", initMsg! );
                return null;
            } );
        }

        [TestMethod]
        public void StartDelayTimer_And_DelayTimer_OnTick_UpdatesCountdown()
        {
            RunInSta<object?>( () =>
            {
                var caller = new MsgBoxImplement();
                var form = new RunSCRCPY( caller, noDelay: false );
                try
                {
                    var t = form.GetType();
                    var startMethod = t.GetMethod( "StartDelayTimer", BindingFlags.Instance | BindingFlags.NonPublic );
                    var tickMethod = t.GetMethod( "DelayTimer_OnTick", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( startMethod );
                    Assert.IsNotNull( tickMethod );

                    startMethod.Invoke( form, null );

                    var countdownField = t.GetField( "CountDownLBL", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( countdownField );
                    var lbl = countdownField.GetValue( form ) as Label;
                    Assert.IsNotNull( lbl );

                    // simulate a tick (should update to "7 seconds..." initially)
                    tickMethod.Invoke( form, new object[] { EventArgs.Empty, EventArgs.Empty } );
                    Assert.IsTrue( lbl.Text.Contains( "seconds" ) );
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        [TestMethod]
        public void DisposeDelayTimer_ClearsRunningFlag()
        {
            RunInSta<object?>( () =>
            {
                var caller = new MsgBoxImplement();
                var form = new RunSCRCPY( caller, noDelay: false );
                try
                {
                    var t = form.GetType();
                    // manually set private static timer and running flag
                    var timer = new System.Windows.Forms.Timer();
                    SetPrivateStaticField( t, "_delayTimer", timer );
                    SetPrivateStaticField( t, "_delayTimerRunning", true );

                    var disposeMethod = t.GetMethod( "DisposeDelayTimer", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( disposeMethod );
                    disposeMethod.Invoke( form, null );

                    bool running = GetPrivateStaticField( t, "_delayTimerRunning" ) as bool? ?? false;
                    Assert.IsFalse( running );
                }
                finally { try { form.Close(); } catch { } try { form.Dispose(); } catch { } }
                return null;
            } );
        }

        [TestMethod]
        public void ExeSCRCPY_NoExeSpec_SetsUIError()
        {
            RunInSta<object?>( () =>
            {
                var caller = new MsgBoxImplement();
                var form = new RunSCRCPY( caller, noDelay: true );
                try
                {
                    var t = form.GetType();
                    // ensure ExeSpec table exists but is empty (created in TestInitialize)

                    // call private ExeSCRCPY
                    var exeMethod = t.GetMethod( "ExeSCRCPY", BindingFlags.Instance | BindingFlags.NonPublic );
                    Assert.IsNotNull( exeMethod );

                    var task = (Task<bool>)exeMethod.Invoke( form, null );
                    Assert.IsNotNull( task );
                    bool result = task.Result;

                    // match actual RunSCRCPY private member name (`_hasUIerror`)
                    bool hasUI = GetPrivateStaticField( t, "_hasUIerror" ) as bool? ?? false;
                    string? initMsg = GetPrivateStaticField( t, "_initMsg" ) as string;

                    Assert.IsFalse( result );
                    Assert.IsTrue( hasUI );
                    Assert.IsNotNull( initMsg );
                    Assert.Contains( "Cannot find scrcpy.exe", initMsg );
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
        private static void SetPrivateStaticField( Type t, string name, object? value )
        {
            // Try private/static property (auto-properties are properties)
            var pi = t.GetProperty( name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            if (pi != null)
            {
                pi.SetValue( null, value );
                return;
            }

            // Try explicit static field
            var fi = t.GetField( name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            if (fi != null)
            {
                fi.SetValue( null, value );
                return;
            }

            // Try compiler-generated backing field for an auto-property
            var bf = t.GetField( $"<{name}>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            if (bf != null)
            {
                bf.SetValue( null, value );
                return;
            }

            // Case-insensitive fallbacks (reduce brittleness across minor renames/casing)
            var piMatch = t.GetProperties( BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public )
                           .FirstOrDefault( p => string.Equals( p.Name, name, StringComparison.OrdinalIgnoreCase ) );
            if (piMatch != null)
            {
                piMatch.SetValue( null, value );
                return;
            }

            var fiMatch = t.GetFields( BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public )
                           .FirstOrDefault( f => string.Equals( f.Name, name, StringComparison.OrdinalIgnoreCase )
                                               || string.Equals( f.Name, $"<{name}>k__BackingField", StringComparison.OrdinalIgnoreCase ) );
            if (fiMatch != null)
            {
                fiMatch.SetValue( null, value );
                return;
            }

            Assert.Inconclusive( $"Field or property '{name}' not found on {t.FullName}" );
        }

        private static object? GetPrivateStaticField( Type t, string name )
        {
            var pi = t.GetProperty( name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            if (pi != null) return pi.GetValue( null );

            var fi = t.GetField( name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            if (fi != null) return fi.GetValue( null );

            var bf = t.GetField( $"<{name}>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            if (bf != null) return bf.GetValue( null );

            // Case-insensitive fallbacks (reduce brittleness across minor renames/casing)
            var piMatch = t.GetProperties( BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public )
                           .FirstOrDefault( p => string.Equals( p.Name, name, StringComparison.OrdinalIgnoreCase ) );
            if (piMatch != null) return piMatch.GetValue( null );

            var fiMatch = t.GetFields( BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public )
                           .FirstOrDefault( f => string.Equals( f.Name, name, StringComparison.OrdinalIgnoreCase )
                                               || string.Equals( f.Name, $"<{name}>k__BackingField", StringComparison.OrdinalIgnoreCase ) );
            if (fiMatch != null) return fiMatch.GetValue( null );

            Assert.Inconclusive( $"Field or property '{name}' not found on {t.FullName}" );
            return null;
        }

[System.Runtime.InteropServices.DllImport("user32.dll")]
private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
[System.Runtime.InteropServices.DllImport("user32.dll")]
private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
[System.Runtime.InteropServices.DllImport("user32.dll")]
[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
private static extern bool IsWindowVisible(IntPtr hWnd);

private static bool WaitForProcessWindow(Process p, int timeoutMs)
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    while (!p.HasExited && sw.ElapsedMilliseconds < timeoutMs)
    {
        p.Refresh(); // keep process information current

        bool found = false;
        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd)) return true; // continue enumeration
            GetWindowThreadProcessId(hWnd, out uint pid);
            if (pid == (uint)p.Id)
            {
                found = true;
                return false; // stop enumeration
            }
            return true; // continue
        }, IntPtr.Zero);

        if (found) return true;
        Thread.Sleep(200);
    }
    return false;
}
    }
}
