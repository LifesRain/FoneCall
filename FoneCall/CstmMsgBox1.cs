using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FoneCall

{
    public partial class CstmMsgBox : Form
    {
        private static bool _hasIntro, _hasExMsg, _hasEndMsg, _needsBtn2, _needsBtn3;
        private TableLayoutPanel _btnTLP { get; set; }
        private ICaller _mb { get; set; }

        public string MBreturn { get; set; } = string.Empty;
        public CstmMsgBox( ICaller caller )
        {
            InitializeComponent();
            _mb = caller;
            _needsBtn2 = _mb.I_Txt_Btn2 != null;
            _needsBtn3 = _mb.I_Txt_Btn3 != null;
            _btnTLP = new TableLayoutPanel();
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            this.FormBorderStyle = FormBorderStyle.None;
            LogR.Norm( $"\n=> Starting Custom Msg Box w/msg type {_mb.I_MsgType}" );
            PopulateMsgBox();
        }
        private void MsgBox_Shown( object sender, EventArgs e )
        {
            BringToFront();
            Application.DoEvents();
        }
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                CopyContent();
                return true;
            }
            return base.ProcessCmdKey( ref msg, keyData );
        }
        private void PopulateMsgBox()
        {
            LogR.Norm( $"caller {_mb.I_MBcaller}\n{_mb.I_Msg}" );
            SetupButtons();

            Font segoe = new Font( "Segoe UI", 9, FontStyle.Regular );
            Font courier = new Font( "Courier New", 9, FontStyle.Regular );

            string[] msgsArray = _mb.I_Msg.Split( new string[] { Xfer.Splitter }, StringSplitOptions.None );

            #region  Size & Position text labels
            _hasIntro = msgsArray[0] != "";
            _hasExMsg = msgsArray.Length > 1 && msgsArray[1] != "";
            _hasEndMsg = msgsArray.Length == 2 && (_hasIntro || _hasExMsg) || msgsArray.Length > 2 && msgsArray[2] != "";

            int szgRef_BTM = 0, max_R = 0, margin = IconPB.Left,
                iconLine_W = _btnTLP.Width - IconPB.Width + (margin / 2), stdLine_W = _btnTLP.Width;
            //iconLine_W = _btnTLP.Width - IconPB.Width + (margin / 2), stdLine_W = _btnTLP.Width;
            if (iconLine_W < MsgLBL.Width) iconLine_W = MsgLBL.Width;
            if (stdLine_W < ExMsgLBL.Width) iconLine_W = ExMsgLBL.Width;
            for (int i = 0; i < msgsArray.Length; i++)
            {
                if (i == 0 && _hasIntro) setLabelSpecs( MsgLBL, segoe, iconLine_W );
                else if (i == 1 && _hasExMsg)
                {
                    if (_hasIntro) setLabelSpecs( ExMsgLBL, courier, stdLine_W );
                    else setLabelSpecs( ExMsgLBL, courier, iconLine_W );
                }
                else if (i > 1)
                {
                    int endWidth = max_R > stdLine_W + margin ? max_R - margin : stdLine_W;
                    setLabelSpecs( EndLBL, segoe, endWidth );
                }

                void setLabelSpecs( Label lbl, Font lblFont, int startWidth )
                {
                    lbl.Font = lblFont;
                    //lbl.Text = M_Cache.FormatMBmsg( msgsArray[i], lblFont, startWidth );
                    lbl.Text = msgsArray[i];
                    lbl.AutoSize = true;

                    if (lbl.Right > max_R) max_R = lbl.Right;
                    //Console.WriteLine( $"max right = {max_R}" );

                    bool firstMsg = _hasIntro && i == 0 || !_hasIntro && i == 1; // assumes never only EndMsg
                    if (firstMsg)
                    {
                        int adjuster = lbl.Height < IconPB.Height ? (IconPB.Height - MsgLBL.Height) / 2 : 0;
                        lbl.Location = new Point( IconPB.Right + (margin / 2), IconPB.Top + adjuster );
                    }
                    else
                    {
                        lbl.Location = new Point( margin, szgRef_BTM );
                        lbl.Visible = true;
                    }
                    szgRef_BTM = i < msgsArray.Length - 1 ? lbl.Bottom + 13 : lbl.Bottom;
                    //Console.WriteLine( $"{lbl.Name} bounds {lbl.Bounds} max bottom {szgRef_BTM}" );
                }
            }
            #endregion
            _btnTLP.Location = new Point( max_R - _btnTLP.Width, szgRef_BTM + 15 );
            CopyMsgLBL.Location = new Point( margin, _btnTLP.Bottom + 15 );
            ClientSize = new Size( max_R + margin, CopyMsgLBL.Bottom + margin );
            //Click2CopyLBL.Location = new Point(max_R - _btnTLP.Width, _btnTLP.Bottom + 15);

            SetVisiCues();
        }
        private void SetupButtons()
        {
            Console.WriteLine();
            MB_BTN2.Visible = _needsBtn2;
            MB_BTN3.Visible = _needsBtn3;
            List<Button> btns = new List<Button>();

            MB_BTN1.Text = _mb.I_Txt_Btn1;
            btns.Add( MB_BTN1 );
            if (_needsBtn2)
            {
                MB_BTN2.Text = _mb.I_Txt_Btn2; btns.Add( MB_BTN2 );
                if (_needsBtn3) { MB_BTN3.Text = _mb.I_Txt_Btn3; btns.Add( MB_BTN3 ); }
            }

            if (!_needsBtn3) MB_BTN2.Margin = MB_BTN3.Margin;
            int b1_W = MB_BTN1.Width + MB_BTN1.Margin.Left,
                b2_W = MB_BTN2.Width + MB_BTN2.Margin.Left + MB_BTN2.Margin.Right,
                b3_W = MB_BTN3.Width + MB_BTN3.Margin.Right,
                tlp_W = _needsBtn3 ? b1_W + b2_W + b3_W : _needsBtn2 ? b1_W + b2_W : b1_W,
                tlp_H = MB_BTN1.Height + MB_BTN1.Margin.Top + MB_BTN1.Margin.Bottom;

            for (int i = btns.Count - 1; i >= 0; i--)
            {
                _btnTLP.ColumnStyles.Add( new ColumnStyle( SizeType.AutoSize ) );
                _btnTLP.Controls.Add( btns[i], btns.Count - 1 - i, 0 );
                btns[i].Click += MB_BTN_Click;
                CstmStyles.BTN_Enabled_Default( btns[i] );
            }

            _btnTLP.Size = new Size( tlp_W + 1, tlp_H + 1 );
            Controls.Add( _btnTLP );
        }
        private void SetVisiCues()
        {
            if (_mb.I_MsgType == States.Fail)
            {
                IconPB.BackgroundImage = Properties.Resources.StopIcon;
                IconPB.BackgroundImageLayout = ImageLayout.Stretch;
                BackColor = CstmStyles.PaleRed;
                CstmBorders.Vari_W( this, 4, CstmStyles.Red );
            }
            else if (_mb.I_MsgType == States.Warn)
            {
                IconPB.BackgroundImage = Properties.Resources.Warning;
                IconPB.BackgroundImageLayout = ImageLayout.Stretch;
                BackColor = CstmStyles.PaleYellow;
                CstmBorders.Vari_W( this, 4, CstmStyles.Yellow );
            }
            else
            {
                IconPB.BackgroundImage = Properties.Resources.info;
                IconPB.BackgroundImageLayout = ImageLayout.Stretch;
                BackColor = CstmStyles.PaleBlue;
                CstmBorders.Vari_W( this, 4, CstmStyles.Blue );
            }
            foreach (var lbl in Controls.OfType<Label>()) if (lbl.Name.Contains( "bord_" )) lbl.BringToFront();
        }
        private void CopyContent()
        {
            int txtLen;
            string txtBrd;
            StringBuilder sb = new StringBuilder();
            txtLen = _mb.I_MsgType.Length;
            txtBrd = new string( '-', txtLen );
            sb.AppendLine( txtBrd );
            sb.AppendLine( _mb.I_MsgType.ToUpper() );
            sb.AppendLine( txtBrd );
            if (_hasIntro) sb.AppendLine( MsgLBL.Text );
            if (_hasExMsg) sb.AppendLine( ExMsgLBL.Text );
            if (_hasEndMsg) sb.AppendLine( EndLBL.Text );
            string opts = _mb.I_Txt_Btn3 != null ? $"Options: {MB_BTN3.Text} {MB_BTN2.Text} {MB_BTN1.Text}"
                          : _mb.I_Txt_Btn2 != null ? $"Options: {MB_BTN2.Text} {MB_BTN1.Text}" : $"Option: {MB_BTN1.Text}";
            txtLen = opts.Length;
            txtBrd = new string( '-', txtLen );
            sb.AppendLine( txtBrd );
            sb.AppendLine( opts );
            sb.AppendLine( txtBrd );
            Clipboard.SetText( sb.ToString() );
        }
        private void MB_BTN_Click( object sender, EventArgs e )
        {
            Button btn = (Button)sender;
            MBreturn = btn.Text;
            Close();
        }
        protected override void OnFormClosed( FormClosedEventArgs e )
        {
            base.OnFormClosed( e );
        }
    }
}
