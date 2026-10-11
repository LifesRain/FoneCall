using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FoneCall
{
    // Ensure FoneCallUI inherits from System.Windows.Forms.Form
    public partial class Welcome : MsgBoxImplement
    {

        public Welcome()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            VerLBL.Text = $"scrcpy {DBC.GetVer()}";
            LogLBL.Text = Path.GetDirectoryName( Paths.AppLog );

            PopulateRTBX();
            AdjustForm();
            WinCtrl.RestoreDBwinPos( this );
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            I_form = this;
            WinCtrl.RegisterWin( this );

            InitEventHandlers();
        }///
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if (keyData == (Keys.Escape)) { Close(); return true; }
            return base.ProcessCmdKey( ref msg, keyData );
        }
        private void PopulateRTBX()
        {
            string rtfCode = DBC.KVP_GetValu<string>( KVP_Keys.Intro ).Replace( "##", "'" );

            // HYPERLINK inn rtf code is a "placeholder" with desired text
            // result of clicking dictated by LinkClicked evvent handfer
            MemoryStream stream = new MemoryStream( Encoding.UTF8.GetBytes( rtfCode ) );
            RTBX.LoadFile( stream, RichTextBoxStreamType.RichText );
            LogR.Norm( "One-time display of usage instructions" );
            string[] rtbxTxt = RTBX.Lines;
            for (int i = 0; i < rtbxTxt.Length; i++) LogR.Norm( $"\t{rtbxTxt[i]}" );
            LogR.Norm( $"\t{LogLBL.Text}" );
        }
        private void AdjustForm()
        {
            int margin = 20;
            LogLBL.Location = new Point( RTBX.Left, RTBX.Bottom );
            CloseBTN.Location = new Point( margin + (RTBX.Width - CloseBTN.Width) / 2, LogLBL.Bottom + 15 );
            ClientSize = new Size( RTBX.Right + margin, CloseBTN.Bottom + 20 );
            CstmBorders.ClearBorder( this );
            CstmBorders.Vari_W( this, 2, Color.WhiteSmoke );
            foreach (var lbl in Controls.OfType<Label>()) if (lbl.Name.Contains( "bord_" )) lbl.BringToFront();

            VerLBL.Location = new Point( 5, ClientSize.Height - VerLBL.Height - 5 );
            WinCtrlPNL.Location = new Point( ClientSize.Width - WinCtrlPNL.Width, 0 );
            Application.DoEvents();
        }
        private void InitEventHandlers()
        {
            RTBX.LinkClicked += RTBX_LinkClicked;
            CloseBTN.Click += CloseIntroBTN_Click;

            WinCtrl.MoveWin( MoveWinPB, Handle );
            WinCtrl.MinWin( MinWinPB );
            WinCtrl.CloseWin( CloseWinPB );
        }
        private void RTBX_LinkClicked( object sender, LinkClickedEventArgs e )
        {
            RunOptMngr opts = new RunOptMngr();
            opts.ShowDialog();
        }
        private void CloseIntroBTN_Click( object sender, EventArgs e )
        {
            DBC.UpdateKVP( KVP_Keys.FirstRun, false );
            WinCtrl.Call_DataEntry( this );
            Close();
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
