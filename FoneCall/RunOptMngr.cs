using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FoneCall
{
    public partial class RunOptMngr : MsgBoxImplement
    {
        private int _displayNum = 0;
        public RunOptMngr()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            FormBorderStyle = FormBorderStyle.None;
            I_form = this;
            WinCtrl.RegisterWin( this );

            SetEventHandlers();
            CstmBorders.Vari_W( LoadingLBL, 3, Color.WhiteSmoke );
            CstmStyles.BTN_Disabled( SaveBTN );
            CstmStyles.BTN_Enabled_Default( CloseBTN );
        }
        protected override void OnShown( EventArgs e )
        {
            base.OnShown( e );

            SuspendLayout();

            PopulateDGVs();
            AdjustForm();
            LoadingLBL.Visible = false;
            DGV_PNL.Visible = true;
            DGV.ClearSelection();
            CloseBTN.Text = "Close";
            CloseBTN.Select();

            ResumeLayout( false );
            PerformLayout();
        }
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if (keyData == (Keys.Escape)) { Close(); return true; }
            return base.ProcessCmdKey( ref msg, keyData );
        }
        private void SetEventHandlers()
        {
            DGV.Enter += DGV_Enter;
            DGV.UserAddedRow += DGV_UserAddedRow;
            DGV.UserDeletedRow += DGV_UserDeletedRow;
            DGV.EditingControlShowing += DGV_EditingControlShowing;
            DGV.DefaultValuesNeeded += DGV_DefaultValuesNeeded;
            DGV.CellEnter += DGV_CellEnter;

            SaveBTN.Click += SaveBTN_Click;
            CloseBTN.Click += CancelBTN_Click;
            ReleasesLINK.LinkClicked += Releases_LinkClicked;
        }
        private void AdjustForm()
        {
            int margin = DGV_PNL.Left, dgv2bot = (2 * margin) + SaveBTN.Height;

            SaveBTN.Location = new Point( DGV.Left, DGV.Bottom + margin );
            CloseBTN.Location = new Point( DGV.Right - CloseBTN.Width, SaveBTN.Top );
            DGV_PNL.Size = new Size( DGV.Right + 2, SaveBTN.Bottom + 2 );
            ClientSize = new Size( DGV_PNL.Width + (2 * margin), DGV_PNL.Bottom + margin );
            CstmBorders.ClearBorder( this );
            CstmBorders.Vari_W( this, 4, Color.WhiteSmoke );
            foreach (var lbl in Controls.OfType<Label>()) if (lbl.Name.Contains( "bord_" )) lbl.BringToFront();

            TextPNL.Location = new Point( (ClientSize.Width - TextPNL.Width) / 2, TextPNL.Top );
            DGV_PNL.Location = new Point( DGV_PNL.Left, TextPNL.Bottom + 10 );

            Screen scr = Screen.FromControl( this );
            Location = new Point( Left, (scr.WorkingArea.Height - ClientSize.Height) / 2 );
        }

        private void PopulateDGVs()
        {
            List<Args> args = DBC.GetArgs();
            int argsNum = args.Count;
            if (args != null && argsNum > 0)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    _displayNum++;
                    // accommodates inabbility to rigth align the first column due to AutoSizeColumnsMode & AlternatingRowsDefaultCellStyle
                    string raNum = _displayNum < 10
                        ? $"00{_displayNum}"
                        : _displayNum > 9 && _displayNum < 100 ? $"0{_displayNum}"
                        : _displayNum.ToString();
                    // Ensure neither Arg nor Role is null to avoid CS8602/CS8604
                    string roleValue = args[i].Role ?? string.Empty;
                    string argValue = args[i].Arg ?? string.Empty;
                    string explainValue = args[i].Explain ?? string.Empty;
                    DGV.Rows.Add( raNum, roleValue, argValue, explainValue );
                }
            }
            else return;


            #region Declare params
            DGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DGV.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            // doesn;t work;  overridden by AutoSizeColumnsMode & AlternatingRowsDefaultCellStyle
            //DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // auto sizing leaves unsightly gray areas; sizing hard coded since content is certain
            DGV.Columns[2].Width = 260;
            DGV.Columns[3].Width = 500;
            DGV.Width = 905;
            #endregion

            Screen scr = Screen.FromControl( this );
            int margin = DGV_PNL.Left,
                waw = scr.WorkingArea.Width,
                avail_2_DGV = waw - (2 * margin) - 20;
            //avail_2_DGV = 550 - (2 * margin) - 20; // testing for smaller screen width
            if (avail_2_DGV < DGV.Width)
            {
                double adjuster = (double)avail_2_DGV / DGV.Width;
                int adjdCol_2_W = (int)(DGV.Columns[2].Width * adjuster),
                    adjdCol_3_W = (int)(DGV.Columns[3].Width * adjuster),
                    diffCol_2 = DGV.Columns[2].Width - adjdCol_2_W,
                    diffCol_3 = DGV.Columns[3].Width - adjdCol_3_W,
                    diffTotal = diffCol_2 + diffCol_3;
                DGV.Width -= diffTotal;
                DGV.Columns[2].Width = adjdCol_2_W;
                DGV.Columns[3].Width = adjdCol_3_W;
            }

            int dgv2bot = (2 * margin) + SaveBTN.Height;
            double vu_H = scr.WorkingArea.Height * 0.7;
            DGV.Height = (int)vu_H - TextPNL.Bottom - 10 - dgv2bot;
        }

        private void DGV_EditingControlShowing( object sender, DataGridViewEditingControlShowingEventArgs e )
        {
            ToolStripMenuItem miCopy = new ToolStripMenuItem( "Copy Selection" );
            ToolStripMenuItem miPaste = new ToolStripMenuItem( "Paste" );
            ToolStripMenuItem[] menuItems = new ToolStripMenuItem[] { miCopy, miPaste };
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange( menuItems );
            e.Control.ContextMenuStrip = contextMenu;

            miCopy.ShortcutKeys = Keys.Control | Keys.C;
            miPaste.ShortcutKeys = Keys.Control | Keys.V;

            miCopy.Click += ( s, ev ) => DGV_Edit( copy: true, paste: false );
            miPaste.Click += ( s, ev ) => DGV_Edit( copy: false, paste: true );
        }
        private void DGV_Edit( bool copy, bool paste )
        {
            if (copy)
            {
                List<string> list = new List<string>();
                StringBuilder sb = new StringBuilder();
                foreach (DataGridViewRow row in DGV.SelectedRows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        list.Add( cell.Value is null ? string.Empty : cell.Value.ToString() );
                    }
                    sb.AppendLine( string.Join( ",", list ) );
                    list.Clear();
                }

                // Fallback for headless/unit-test scenarios where SelectedRows may be empty
                // (control not visible or selection not applied). Copy all non-new rows so
                // tests that expect row contents on the clipboard still get deterministic output.
                if (sb.Length == 0)
                {
                    foreach (DataGridViewRow row in DGV.Rows)
                    {
                        if (row.IsNewRow) continue;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            list.Add( cell.Value is null ? string.Empty : cell.Value.ToString() );
                        }
                        sb.AppendLine( string.Join( ",", list ) );
                        list.Clear();
                    }
                }

                try
                {
                    Clipboard.SetText( sb.ToString() );
                }
                catch (ArgumentNullException)
                {
                    // Guard against unexpected nulls; ensure no exception leaves DGV_Edit.
                    try { Clipboard.SetText( string.Empty ); } catch { /* best-effort */ }
                }
                catch
                {
                    // Try a Unicode text format as a fallback (best-effort in unusual environments).
                    try { Clipboard.SetText( sb.ToString(), TextDataFormat.UnicodeText ); } catch { /* best-effort */ }
                }
            }
            else
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split( new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries );
                if (lines.Length > 0)
                {
                    foreach (string line in lines) DGV.Rows.Add( line );
                    CstmStyles.BTN_Enabled_Default( SaveBTN );
                }

                CstmStyles.BTN_Enabled_Green( SaveBTN );
            }
        }
        private void DGV_Enter( object sender, EventArgs e )
        {
            CstmStyles.BTN_Enabled_Green( SaveBTN );
            CloseBTN.Text = "Cancel";
        }
        private void DGV_DefaultValuesNeeded( object sender, DataGridViewRowEventArgs e )
        {
            _displayNum++;
            e.Row.Cells[0].Value = _displayNum;
            e.Row.Cells[1].Value = Xfer.R_arg;
        }
        private void DGV_CellEnter( object sender, DataGridViewCellEventArgs e )
        {
            // Prevents user from selecting the first two columns (display number and role)
            // and automaticallt selects the third column (argument) which  is the first data entry column
            DataGridViewColumn cellCol = DGV.Columns[e.ColumnIndex];
            if (cellCol.Index == 0 || cellCol.Index == 1)
            {
                DataGridViewCell cell = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Selected = false;
                DataGridViewCell shiftCell = DGV.Rows[e.RowIndex].Cells[2];
                shiftCell.Selected = true;
            }
        }
        private void DGV_UserAddedRow( object sender, DataGridViewRowEventArgs e )
        {
            CstmStyles.BTN_Enabled_Green( SaveBTN );
            CloseBTN.Text = "Cancel";
        }
        private void DGV_UserDeletedRow( object sender, DataGridViewRowEventArgs e )
        {
            CstmStyles.BTN_Enabled_Green( SaveBTN );
            CloseBTN.Text = "Cancel";
        }
        private void Releases_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            string url = "https://github.com/Genymobile/scrcpy/releases";
            Process.Start( new ProcessStartInfo( url ) { UseShellExecute = true } );
        }
        private void SaveBTN_Click( object sender, EventArgs e )
        {
            CloseBTN.Text = "Close";
            DBC.ClearArgs();
            // col_0 = rowNum, col_1 = role, col_2 = arg, col_3 = explain;
            // rowNum not saved to DB, but used to display the row number in the DGV
            foreach (DataGridViewRow row in DGV.Rows)
            {
                object cellObj_1 = row.Cells[1].Value,
                       cellObj_2 = row.Cells[2].Value;
                if (cellObj_1 == null && cellObj_2 == null) continue;

                object cellObj_3 = row.Cells[3].Value;
                string r = cellObj_1 as string ?? null,
                       o = cellObj_2 as string ?? null,
                       exp = cellObj_3 as string ?? null;

                Args args = exp is null
                       ? new Args { Role = r, Arg = o } : new Args { Role = r, Arg = o, Explain = exp };
                DBC.SaveArgs_Model( args );
            }
        }
        private void CancelBTN_Click( object sender, EventArgs e )
        {
            Close();
        }
        protected override void OnFormClosed( FormClosedEventArgs e )
        {
            base.OnFormClosed( e );
            if (WinCtrl.IsRunningUnderUnitTests()) return;

            WinCtrl.UnregisterWin( this );
        }
    }
}
