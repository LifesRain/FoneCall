using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoneCall
{
    public partial class RunSCRCPY : MsgBoxImplement
    {
        #region Declare Fiels
        private static int _elapsed { get; set; } = 0;
        private static bool _hasUIerror { get; set; } = false;
        private static bool _hasProcErr { get; set; } = false;
        private static bool _nuProgMsg { get; set; } = false;
        private static bool _delayTimerRunning { get; set; } = false;
        private static bool _msgTimerRunning { get; set; } = false;
        private static string _uiErrorMsg { get; set; } = default;
        private static string _initMsg { get; set; } = default;

        private static System.Windows.Forms.Timer _activeTimer { get; set; }
        private static System.Windows.Forms.Timer _delayTimer { get; set; }
        private static System.Windows.Forms.Timer _msgTimer { get; set; }
        private static bool _noDelay { get; set; } = false;

        private ICaller _caller { get; set; }
        #endregion
        public RunSCRCPY( ICaller caller, bool noDelay )
        {
            InitializeComponent();

            WinCtrl.RestoreDBwinPos( this );
            _caller = caller;
            _noDelay = noDelay;
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            this.FormBorderStyle = FormBorderStyle.None;
            I_form = this;
            VerLBL.Text = $"scrcpy {DBC.GetVer()}";
            WinCtrl.RegisterWin( this );

            InitEventHandlers();
            AdjustForm();
        }
        protected override void OnShown( EventArgs e )
        {
            base.OnShown( e );

            _hasProcErr = false;

            if (_caller.I_form != null) _caller.I_form.Close();

            if (_noDelay) ProgMsgSetup();
            else StartDelayTimer();
        }
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if (keyData == (Keys.Escape)) { Close(); return true; }
            if (keyData == (Keys.Enter))
            {
                if (_delayTimerRunning) AbortStart();

                return true;
            }
            if (keyData == (Keys.F1))
            {
                LogR.Norm( $"hot key skip dellay && start immediately" );
                if (_delayTimerRunning) ProgMsgSetup();
                return true;
            }
            return base.ProcessCmdKey( ref msg, keyData );
        }
        private void AdjustForm()
        {
            //int margin = 20;
            //ClientSize = new Size( MsgPNL.Right + margin, MsgPNL.Bottom + 20 );
            CstmBorders.ClearBorder( this );
            CstmBorders.Vari_W( this, 2, Color.WhiteSmoke );
            foreach (var lbl in Controls.OfType<Label>()) if (lbl.Name.Contains( "bord_" )) lbl.BringToFront();

            VerLBL.Location = new Point( 5, ClientSize.Height - VerLBL.Height - 5 );
            WinCtrlPNL.Location = new Point( ClientSize.Width - WinCtrlPNL.Width, 0 );
            Application.DoEvents();
        }

        #region Timeer
        // timers split by function since using one timer with different Intervals/function pruced unreliable results
        private void StartDelayTimer()
        {
            LogR.Norm( $"-> starting scrcpy execution delay timer" );
            _delayTimer = new System.Windows.Forms.Timer();
            _delayTimer.Interval = 1000;
            _delayTimer.Tick += DelayTimer_OnTick;
            _delayTimer.Enabled = true;
            _delayTimerRunning = true;
            MsgTitleLBL.Height = 18;
            //MsgTitleLBL.Visible = true;
            CountDownLBL.Height = 22;
            //ountDownLBL.Visible = true;
        }
        private void DelayTimer_OnTick( object sender, EventArgs e )
        {
            if (!_delayTimerRunning) return; // safety check

            _elapsed += _delayTimer.Interval;
            if (_elapsed < 10000 && !_noDelay)
            {
                CountDownLBL.Text = $"{10 - (_elapsed / 1000)} seconds...";
            }
            else ProgMsgSetup();
        }
        private void DisposeDelayTimer()
        {
            if (!_delayTimerRunning) return; // safety check

            LogR.Norm( $"-> disposing display timer" );
            _delayTimer.Enabled = false;
            _delayTimer.Dispose();
            _delayTimerRunning = false;
        }
        private void StartMsgTimer()
        {
            LogR.Norm( $"-> starting scrcpy execution msg timer;" );
            _msgTimer = new System.Windows.Forms.Timer();
            _msgTimer.Interval = 70;
            _msgTimer.Tick += MsgTimer_OnTick;
            _msgTimer.Enabled = true;
            _msgTimerRunning = true;
        }
        private void MsgTimer_OnTick( object sender, EventArgs e )
        {
            if (!_msgTimerRunning) return; // safety check

            if (_nuProgMsg)
            {
                MsgLBL.Text = _initMsg;
                _nuProgMsg = false;
            }
        }
        private void DisposeMsgTimer()
        {
            if (!_msgTimerRunning) return; // safety check

            LogR.Norm( $"-> disposing prog msg timer;" );
            _msgTimer.Enabled = false;
            _msgTimer.Dispose();
            _msgTimerRunning = false;
        }
        #endregion
        private void InitEventHandlers()
        {
            Click += Abort_Click;
            CountDownLBL.Click += Abort_Click;
            MsgLBL.Click += Abort_Click;
            Click += Abort_Click;

            WinCtrlPNL.MouseEnter += WinCtrlPNL_Enter;
            WinCtrlPNL.MouseLeave += WinCtrlPNL_Leave;

            WinCtrl.MoveWin( MoveWinPB, Handle );
            WinCtrl.MinWin( MinWinPB );
            WinCtrl.CloseWin( CloseWinPB );
        }
        private void ProgMsgSetup()
        {
            DisposeDelayTimer();
            CountDownLBL.Visible = false;
            StartNowLBL.Visible = false;
            EnterSymLBL.Visible = false;

            MsgTitleLBL.Text = "Initializing scrcpy. ..";
            MsgTitleLBL.Location = new Point( MsgTitleLBL.Left, 41 );
            MsgLBL.Font = CstmStyles.Std_9_R;
            MsgLBL.Height = 48;
            MsgLBL.Location = new Point( MsgLBL.Left, 73 );

            WinCtrlPNL.Visible = false;
            Application.DoEvents();

            InitProcRun();
        }
        private async void InitProcRun()
        {
            StartMsgTimer();

            // must execute scrcpy on non-UI thread in order to display prog msgs
            // in the MsgPNL while scrcpy is executed
            bool scrcpyRunning = await Task.Run( () =>
            {
                return ExeSCRCPY();
            } );
            DisposeMsgTimer();

            if (scrcpyRunning) Close();
            else
            {
                #region MSG
                string errType = _hasUIerror ? "FoneCall" : _hasProcErr ? "scrcpy execution" : "scrcpy display.",
                       intro = _hasUIerror ? _uiErrorMsg : _hasProcErr ? _initMsg : "scrcpy failed to display.",
                       instruct = _hasUIerror ? "This issue may be transitory or FoneCall may be corrupted."
                                                : Xfer.ErrChks,
                       btn1 = _hasUIerror ? "Retry" : "Fix",
                       opt1 = _hasUIerror ? "running scrcpy" : "the problem in Data Entry",
                       msg = $"{errType} error: {intro}" +
                            $"{Xfer.Splitter2}{instruct}\n\n" +
                            $"The FoneCall database and log are located in\n{Paths.AppData}\n\n" +
                            $"You can \"{btn1}\" {opt1} or \"Exit\" FoneCall,";
                #endregion
                #region msgBox Params
                I_form = this;
                I_MBcaller = Name;
                I_MsgType = States.Warn;
                I_Msg = msg;
                I_Txt_Btn1 = btn1;
                I_Txt_Btn2 = "Exit";
                #endregion
                string mbReturn = WinCtrl.CstmmMB_Call( this );
                if (mbReturn == I_Txt_Btn2) Close();
                else
                {
                    if (_hasUIerror) InitProcRun();
                    else WinCtrl.Call_DataEntry( this );
                }
            }
        }
        private Task<bool> ExeSCRCPY()
        {
            LogR.Norm( "-> starting scrcpy execution" );

            bool scrcpyRunning = false;
            ExeSpec exeSpec = DBC.GetExeSpec();
            if (exeSpec == null)
            {
                _hasUIerror = true;
                _initMsg = "Cannot find scrcpy.exe FQN\nsaved in the database.";
                _uiErrorMsg = "scrcpy.exe FQN not found in the database.";
                return Task.FromResult( scrcpyRunning );
            }

            var exePath = exeSpec.ScrcpyFQN?.Trim( '"' ) ?? throw new InvalidOperationException( "missing exe path" );
            var workDir = Path.GetDirectoryName( exePath );

            ProcessStartInfo si = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = workDir,
                RedirectStandardOutput = true, // async event handler
                RedirectStandardError = true, // async event handler
                RedirectStandardInput = true,

                UseShellExecute = false, // must be false to redirect input, output, and error streams
                CreateNoWindow = true,
            };
            if (!string.IsNullOrWhiteSpace( exeSpec.Arguments ))
                si.Arguments = exeSpec.Arguments.Replace( ',', ' ' );
            //si.Arguments = target.Arguments.Replace('-', '?'); // for testing invalid args

            LogR.Norm( $"\texe Dir  = {si.WorkingDirectory}\n\t" +
                         $"exe FQN  = {si.FileName}\n\t" +
                         $"exe Args = {(si.Arguments ?? "no arguments; running with defaults")}" );

            using (var p = new Process() { StartInfo = si, EnableRaisingEvents = true })
            {
                p.ErrorDataReceived += DataReceived;
                p.OutputDataReceived += DataReceived;

                p.Start();
                if (p.HasExited)
                {
                    #region try-catch insufficient explanation
                    //  This is a safeguard to catch cases where scrcpy.exe fails
                    //  to start or exits immediately.if that occurs the culprit
                    //  is ne or more invalid arguments sinnce the executable path
                    //  is validated prior to saving it to the DB but an invalid
                    //  argument can occur if the user fails to fails to separate
                    //  the arguments as instructed. The condition doesn't throw
                    //  an exception,attempting to assign a PriorityClass so this
                    //  process will be ab aborted and the user will be notified
                    //  of the error in the UI and  the log.
                    #endregion

                    _hasProcErr = true;
                    _initMsg = "Invalid arguments strongly suspected";
                    return Task.FromResult( scrcpyRunning );
                }
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                try { p.PriorityClass = ProcessPriorityClass.High; }
                catch (Exception ex)
                {
                    _hasUIerror = true;
                    _uiErrorMsg = ex.Message;
                    return Task.FromResult( scrcpyRunning );
                }

                LogR.Norm( "Waiting for the scrcpy phone mirroring window to appear..." );
                #region Run scrcpy.exe Process.WaitForInputIdle() workaaround explanation
                // Process.WaitForInputIdle() is typically used to detect when a
                // process has finished initializing and is ready for user input.
                // However,  starting scrcpy.exe from a C# .NET 9 application, Process
                //.WaitForInputIdle() will throw an InvalidOperationException.
                //
                // This happens because scrcpy.exe behaves initially like a console
                // application (or relies on an underlying console engine/wrapper) and
                // lacks a standard Windows messaging loop at startup.
                //
                // WORKAROUND FOR WaitForInputIdle(): 
                // Wait for the mirroring window's handle to become available after it
                // is spawned to detect when scrcpy is ready for user input and FoneCall
                // can automatically edit gracefully. This is done by polling the value
                // of MainWindowHandle. Source: Google AI (amazing!)
                #endregion
                while (!p.HasExited && p.MainWindowHandle == IntPtr.Zero)
                {
                    // CRITICAL  since MainWindowHandle isn't automatically updated
                    // Refresh the internal process state cache
                    p.Refresh();
                    Thread.Sleep( 500 ); // Small delay to avoid pegging the CPU
                }
                scrcpyRunning = !p.HasExited && p.MainWindowHandle != IntPtr.Zero;
                LogR.Norm( $"scrcpy window is ready = {scrcpyRunning}" );

                p.WaitForExit( 5000 );
            }
            return Task.FromResult( scrcpyRunning );
        }
        static void DataReceived( object sender, DataReceivedEventArgs e )
        {
            if (_hasProcErr) return; // if error already detected don't process any more output
            if (!string.IsNullOrWhiteSpace( e.Data ))
            {
                //LogR.Norm( $"scrcpy msg: {e.Data}" );
                if (e.Data.Contains( "<http" ))
                {
                    _initMsg = e.Data.Substring( 0, e.Data.IndexOf( "<http" ) );
                    _initMsg += "executable found";
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.ToLower().Contains( "error" ))
                {
                    _hasProcErr = true;
                    _initMsg = e.Data.Replace( "ERROR: ", "" );
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "running;" ))
                {
                    _initMsg = e.Data.Replace( "running;", "running;\n" );
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "daemon started" ))
                {
                    _initMsg = e.Data;
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "INFO: ADB" ))
                {
                    _initMsg = e.Data;
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "scrcpy-server:" ))
                {
                    _initMsg = e.Data.Substring( e.Data.IndexOf( "scrcpy-server:" ) );
                    _initMsg = _initMsg.Replace( "pushed,", "pushed,\n" );
                    _initMsg = _initMsg.Replace( "MB/s ", "MB/s\n" );
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "INFO:     -->   (" ))
                {
                    _initMsg = e.Data;
                    _initMsg = _initMsg.Replace( "INFO:     -->   (", "INFO:--> (" );
                    _initMsg = _initMsg.Replace( "                     ", "\n" );
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "Device:" ))
                {
                    _initMsg = e.Data.Substring( e.Data.IndexOf( "Device:" ) );
                    _initMsg = _initMsg.Replace( "]", "]\n" );
                    _initMsg = _initMsg.Replace( "(", "\n(" );
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "INFO: Renderer" ))
                {
                    _initMsg = e.Data;
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
                else if (e.Data.Contains( "INFO: Texture" ))
                {
                    _initMsg = e.Data;
                    LogR.Norm( $"scrcpy msg: {_initMsg}" );
                    _nuProgMsg = true;
                }
            }
        }
        private void Abort_Click( object sender, EventArgs e )
        {
            AbortStart();
        }
        private void WinCtrlPNL_Enter( object sender, EventArgs e )
        {
            _delayTimer.Enabled = false;
        }
        private void WinCtrlPNL_Leave( object sender, EventArgs e )
        {
            _delayTimer.Enabled = true;
        }
        private void AbortStart()
        {
            string cause = _hasUIerror ? "scrcpy execution error" : "scrcpy run prevented by user";
            DisposeDelayTimer();
            LogR.Norm( $"{cause}, calling Data Entry at {Xfer.RunTime}" );
            WinCtrl.Call_DataEntry( this );
        }
        protected override void OnFormClosed( FormClosedEventArgs e )
        {
            base.OnFormClosed( e );
            if (WinCtrl.IsRunningUnderUnitTests()) return;

            WinCtrl.SaveWinPos( this );
            WinCtrl.UnregisterWin( this );
        }
    }
}
