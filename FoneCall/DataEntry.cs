using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FoneCall
{
    public partial class DataEntry : MsgBoxImplement
    {
        #region Declare fields
        private static bool _loading { get; set; } = false;
        private static bool _argsOK { get; set; } = false;
        private static string _fqnAtStart { get; set; }
        private static string _argsAtStart { get; set; }

        private static ToolTip _cttFQN;
        private static ToolTip _cttArgs;
        private static Control _ttCtrl { get; set; }
        //private const string _fqnStart = @"e.g.: D:\scrcpy-win64-v4.1\scrcpy.exe";
        private string _argStart { get; set; } = "e.g:  --always-on-top, --window-height 700";

        private ICaller _caller { get; set; }
        #endregion

        public DataEntry( ICaller caller )
        {
            _loading = true;
            InitializeComponent();

            WinCtrl.RestoreDBwinPos( this );
            _caller = caller;
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            this.FormBorderStyle = FormBorderStyle.None;

            I_form = this;
            WinCtrl.RegisterWin( this );
            VerLBL.Text = $"scrcpy {DBC.KVP_GetValu<string>( KVP_Keys.Version )}";

            AdjustForm();
            InitEventHandlers();

            _loading = false;
        }
        protected override void OnShown( EventArgs e )
        {
            base.OnShown( e );

            // only true if not started from ApplicationContext & if from Context ExeSpec is null
            if (_caller.I_form != null) 
            {
                _caller.I_form.Close();
                var spec = DBC.GetExeSpec();
                if (spec != null)
                {
                    FQN_TBX.Text = spec.ScrcpyFQN;
                    ArgsTBX.Text = spec.Arguments;
                    LogR.Norm( $"Execution values:\n\t" +
                               $"exe FQN  = {spec.ScrcpyFQN}\n\t" +
                               $"exe Args = {(spec.Arguments ?? "no arguments; running with defaults")}" );
                    foreach (var tbx in Controls.OfType<TextBox>()) InitTBX( tbx, truInit: false );
                    if (CanSave())
                    {
                        LogR.Norm( $"Execution values validated for running scrcpy" );
                        ExeBTN.Text = Xfer.Run;
                        CstmStyles.BTN_Enabled_Green( ExeBTN );
                    }
                    else cannotRun();
                }
                else cannotRun();
            }
            else cannotRun();

            _fqnAtStart = FQN_TBX.Text;
            _argsAtStart = ArgsTBX.Text;

            void cannotRun()
            {
                LogR.Norm( $"valid execution values required to run scrcpy" );
                ExeBTN.Text = Xfer.Save;
                CstmStyles.BTN_Disabled( ExeBTN );
            }
        }
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if (keyData == (Keys.Escape)) { Close(); return true; }
            return base.ProcessCmdKey( ref msg, keyData );
        }
        private void AdjustForm()
        {
            int margin = 20;
            ClientSize = new Size( FQN_TBX.Right + margin, ExeBTN.Bottom + 30 );
            CstmBorders.ClearBorder( this );
            CstmBorders.Vari_W( this, 2, Color.WhiteSmoke );
            foreach (var lbl in Controls.OfType<Label>()) if (lbl.Name.Contains( "bord_" )) lbl.BringToFront();

            VerLBL.Location = new Point( 5, ClientSize.Height - VerLBL.Height - 5 );
            WinCtrlPNL.Location = new Point( ClientSize.Width - WinCtrlPNL.Width, 0 );
            Application.DoEvents();
        }
        private void InitEventHandlers()
        {
            Click += HideTT_Click;
            FQNhdrLBL.Click += HideTT_Click;
            ArgHdrLBL.Click += HideTT_Click;
            FQN_TBX.Click += HideTT_Click;
            ArgsTBX.Click += HideTT_Click;

            UpdateArgsLINK.LinkClicked += UpdateArgs_LinkClicked;
            ExeBTN.Click += ExeBTN_Click;

            FQN_TBX.Enter += TBX_Enter;
            ArgsTBX.Enter += TBX_Enter;
            FQN_TBX.TextChanged += TBX_TextChanged;
            ArgsTBX.TextChanged += TBX_TextChanged;
            FQN_TBX.Leave += FQN_TBX_Leave;
            ArgsTBX.Leave += ArgsTBX_Leave;

            WinCtrl.MoveWin( MoveWinPB, Handle );
            WinCtrl.MinWin( MinWinPB );
            WinCtrl.CloseWin( CloseWinPB );
        }
        private void TBX_Enter( object sender, EventArgs e )
        {
            if (_loading) return;
            FQN_TBX.TabStop = true;
            ArgsTBX.TabStop = true;

            ExeBTN.Text = Xfer.Save;
            if (sender is TextBox tbx) InitTBX( tbx, truInit: true );
        }
        private void InitTBX( TextBox tbx, bool truInit )
        {
            tbx.Font = CstmStyles.Std_9_R;
            tbx.ForeColor = Color.Black;
            tbx.BackColor = Color.White;
            if (truInit) tbx.SelectAll();
        }
        private void TBX_TextChanged( object sender, EventArgs e )
        {
            if (_loading) return;

            if (sender is TextBox tbx)
            {
                bool fqnNu = false, argsNNu = false;
                InitTBX( tbx, truInit: false );
                if (tbx == FQN_TBX && tbx.Text != _fqnAtStart) fqnNu = true;
                else if (tbx == ArgsTBX && tbx.Text != _argsAtStart) argsNNu = true;

                if (fqnNu || argsNNu) ExeBTN.Text = Xfer.Save;
                else ExeBTN.Text = Xfer.Run;
            }

            if (CanSave()) { CstmStyles.BTN_Enabled_Green( ExeBTN ); HideTT(); }
            else CstmStyles.BTN_Disabled( ExeBTN );
        }
        private void FQN_TBX_Leave( object sender, EventArgs e )
        {
            if (sender is TextBox tbx)
            {
                tbx.Text = tbx.Text.Replace( "\"", "" ).Trim();
                if (CanSave()) CstmStyles.BTN_Enabled_Green( ExeBTN );
                else
                {
                    _ttCtrl = FQN_TBX;
                    string intro = !File.Exists( tbx.Text )
                            ? "File does not exist."
                            : "Invalid scrcpy.exe path.",
                            ttTxt = $"{intro}\nFix the error to enable Saving.\nClick app to remove message.";
                    _cttFQN = CstmStyles.Custom_tt( ttTxt, _ttCtrl, 30000, error: true );
                    CstmStyles.BTN_Disabled( ExeBTN );
                    tbx.BackColor = CstmStyles.PaleRed;
                    tbx.Select();
                }
            }
        }
        private void ArgsTBX_Leave( object sender, EventArgs e )
        {
            if (sender is TextBox tbx)
            {
                if (string.IsNullOrWhiteSpace( tbx.Text ) || tbx.Text.Contains("e.g."))
                { _argsOK = false; return; }

                if (tbx.Text.Contains( "-" ))
                {
                    int count = Regex.Matches( tbx.Text, "--" ).Count;

                    // validity checks only if comma separated args are detected, otherwise
                    // invalid args caught in ExeSCRPY() when scrcpy will fail to start
                    if (tbx.Text.Contains( "," ))
                    {
                        // valid args in DB were manually extracted from  scrcpy documentation
                        // this is a simple way to validate the args without having to
                        // parse the scrcpy documentation
                        _argsOK = true;
                        List<string> invaidArgs = new List<string>();
                        string[] args = ArgsTBX.Text.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                        foreach (var arg in args)
                        {
                            string argTrim = arg.Replace( "--", "" ).Trim();
                            argTrim = argTrim.Substring( 0, argTrim.IndexOf( " " ) > 0 ? argTrim.IndexOf( " " ) - 1 : argTrim.Length );
                            if (!DBC.CheckArg( argTrim ))
                            { _argsOK = false; invaidArgs.Add( argTrim.Trim( ',' ) ); }
                        }
                        if (_argsOK) tbx.Text = tbx.Text.Replace( ",", "" ).Trim();
                        else
                        {
                            _ttCtrl = ArgsTBX;

                            string intro = $"Invalid argument(s) detected:";
                            foreach (var arg in invaidArgs) intro += $"\n{arg}";

                            string ttTxt = $"{intro}\nFix the error to enable Saving.\nClick app to remove message.";
                            _cttArgs = CstmStyles.Custom_tt( ttTxt, _ttCtrl, 30000, error: true );
                            CstmStyles.BTN_Disabled( ExeBTN );
                            tbx.Select();
                        }
                    }
                    else _argsOK = true;
                    if (CanSave() && _argsOK) CstmStyles.BTN_Enabled_Default( ExeBTN );
                    else CstmStyles.BTN_Disabled( ExeBTN );
                }
            }
        }
        private bool CanSave()
        {
            string fqnText = FQN_TBX.Text?.Trim() ?? string.Empty;
            bool ok = File.Exists(fqnText) && fqnText.Contains("scrcpy.exe");
            VERIFIED_LBL.Visible = ok;
            return ok;
        }
        private void UpdateArgs_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            RunOptMngr opts = new RunOptMngr();
            opts.ShowDialog();
        }
        private void ExeBTN_Click( object sender, EventArgs e )
        {
            if (sender is Button btn)
            {
                if (btn.Text == Xfer.Save) SaveExeSpec();
                else if (btn.Text == Xfer.Run)
                {
                    LogR.Norm( $"no changes in Data Entry; returning to RunSCRCPY" );
                    WinCtrl.Call_RunSCRCPY( this, noDelay: true );
                }
                else
                {
                    LogR.Norm( $"scrcpy run failed -- exiting FoneCall at {Xfer.RunDate}" );
                    Close();
                }
            }
        }
        private void SaveExeSpec()
        {
            // logging of save detail done DBC.SaveExeSpec( t );
            ExeSpec t = new ExeSpec { ScrcpyFQN = FQN_TBX.Text };
            if (_argsOK && _argsOK) t.Arguments = ArgsTBX.Text;

            DBC.SaveExeSpec( t );

            if (DBC.GetExeSpec() != null)
            {
                DBC.UpdateKVP( KVP_Keys.FirstRun, "false" );
                WinCtrl.Call_RunSCRCPY( this, noDelay: true );
            }
            else
            {
                #region msgBox Params
                string msg = "Save to DB failed. If this message is a rare event the failure " +
                             "may be transitory.  Otherwise FoneCall may have become corrupted" +
                             "and reinstalling might solve the problem." +
                            $"{Xfer.Splitter2}" +
                            $"The FoneCall log is located in\n{Paths.AppData}\n\n" +
                            $"You can \"Retry\" saving or \"Exit\" FoneCall,";
                #endregion
                #region msgBox Params
                I_form = this;
                I_MBcaller = Name;
                I_MsgType = States.Warn;
                I_Msg = msg;
                I_Txt_Btn1 = "Retry";
                I_Txt_Btn2 = "Exit";
                #endregion
                string mbReturn = WinCtrl.CstmmMB_Call( this );
                if (mbReturn != null && mbReturn == I_Txt_Btn2) Close();
                else SaveExeSpec();
            }
        }

        private void HideTT_Click( object sender, EventArgs e )
        {
            HideTT();
        }
        private void HideTT()
        {
            if (_cttFQN != null && _ttCtrl == FQN_TBX) _cttFQN.Hide( _ttCtrl );
            else if (_cttArgs != null) _cttArgs.Hide( _ttCtrl );
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
