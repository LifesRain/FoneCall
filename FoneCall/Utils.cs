using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Dapper;


namespace FoneCall
{
    public class Paths
    {
        // gets the app's exe directory
        public static string AppExeDir { get; } = AppContext.BaseDirectory;
        public static string AppExeFQN { get; } = Application.ExecutablePath;

        // apparently user only has access rights to CommonApplicationData if they & app run as Admin
        static string PD  = Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData );
        static string ULD = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
        public static string AppData 
        {
            get
            {
                if (AppExeFQN.ToLower().Contains("debug")) return Path.Combine( PD, @"FoneCall\" );
                else return Path.Combine( ULD, @"FoneCall\" );
            }
        }

        /********  FOR UNIT TESTS 
        public static string AppData { get; internal set; } = Application.ExecutablePath;
         /compute these at runtime from AppData so tests can change AppData and have
        // AppDB/AppLog reflect that immediately
        public static string AppDB => Path.Combine( AppData, "FoneCall.db" );
        public static string AppLog => Path.Combine( AppData, "FoneCall.log" );
        */

        public static string AppDB { get; } = Path.Combine( AppData, "FoneCall.db" );
        public static string AppLog { get; } = Path.Combine( AppData, "FoneCall.log" );

        public static string Help { get; } = @"E:\FoneCall\Data\scrcpy help.txt"; // used in dev to populate DB
    }
    public static class KVP_Keys
    {
        public const string FirstRun = "FirstRun";
        public const string Intro = "Intro";
        public const string Version = "Version";
    }
    public static class TBL
    {
        public const string Args = "Args";
        public const string KVP = "KVP";
        public const string StartPos = "StartPos";
        public const string ExeSpec = "ExeSpec";
    }
    public static class Xfer
    {
        public const string R_arg = "ARG";
        public const string R_shrt = "SCT";
        public const string R_environ = "ENV";
        public const string R_exit = "ES";
        public const string R_ver = "VER";

        public const string DBaMod = "GetMod";
        public const string Exit = "Exit";
        public const string Run = "RUN scrcpy";
        public const string Save = "Save to DB";
        public const string Splitter = "|||";
        public const string Splitter2 = "||||||";
        public const string ErrChks = "Please check the following:\n" +
                "- Ensure that the scrcpy executable exists at the specified path.\n" +
                "- Ensure that all requested Arguments are valid.\n" +
                "- Verify that you have the necessary permissions to execute the file.\n" +
                "- Check if any antivirus or security software is blocking the execution.\n\n" +
                "If the issue persists after checking the above\nFoneCall may need to be reinstalled.";

        public static List<ICaller> OpenWins { get; set; }
        public static string RunDate { get; } = DateTime.Now.ToString( "yyyy MMM dd HH:mm" );
        public static string RunTime { get; } = DateTime.Now.ToString( "HH:mm" );
    }
    public static class DBC
    {
        public static SQLiteConnection SQLyt()
        {
            var csb = new SQLiteConnectionStringBuilder
            {
                DataSource = Paths.AppDB,
                Version = 3,
                FailIfMissing = true,
            };
            return new SQLiteConnection( csb.ConnectionString );
        }
        public static Task<bool> CheckDB()
        {
            bool dbOK = false;
            if (!File.Exists( Paths.AppDB )) return Task.FromResult( dbOK );
            dbOK = OneT_Valu<string>( "PRAGMA integrity_check" ) != null;

            return Task.FromResult( dbOK );
        }
        private static void JustSQL( string sql )
        {
            try { using (var dbc = new SQLiteConnection( SQLyt() )) dbc.Execute( sql ); }
            catch (SqlException ex) { LogR.Norm( $"{ex}" ); }
            catch (Exception ex) { LogR.Norm( $"{ex}" ); }
        }
        public static void TypedModelSave<T>( string sql, object model )
        {
            try
            {
                using (var dbc = new SQLiteConnection( SQLyt() ))
                    dbc.Execute( sql, (T)model );
            }
            catch (SqlException ex) { LogR.Norm( $"{ex}" ); throw;  }
            catch (Exception ex) { LogR.Norm( $"{ex}" ); }
        }
        public static T TypedModel<T>( string sql )
        {
            T rtrn = default( T );
            try
            {
                using (var dbc = new SQLiteConnection( SQLyt() ))
                    rtrn = dbc.QuerySingleOrDefault<T>( sql );

            }
            catch (SqlException ex) { LogR.Norm( $"{ex}" ); }
            catch (Exception ex) { LogR.Norm( $"{ex}" ); }

            return rtrn;
        }
        public static List<T> TypedList<T>( string sql )
        {
            List<T> listRtrn = null;
            try
            {
                using (var dbc = new SQLiteConnection( SQLyt() ))
                    listRtrn = dbc.Query<T>( sql ).ToList();
            }
            catch (SqlException ex) { LogR.Norm( $"{ex}" ); }
            catch (Exception ex) { LogR.Norm( $"{ex}" ); }

            return listRtrn ?? new List<T>();
        }
        private static T OneT_Valu<T>( string sql )
        {
            T scalarValue = default( T );
            try
            {
                using (var dbc = new SQLiteConnection( SQLyt() ))
                    scalarValue = dbc.ExecuteScalar<T>( sql );

                // Fix: Only check ToLower if scalarValue is string, and use default(T) instead of null for generic type
                if (scalarValue is string strValue && !string.IsNullOrEmpty( strValue ) && strValue.ToLower().Contains( "null" ))
                    scalarValue = default( T );
            }
            catch (SqlException ex) { LogR.Norm( $"{ex}" ); }
            catch (Exception ex) { LogR.Norm( $"{ex}" ); }

            return scalarValue;
        }
        private static bool BoolValue( string sql )
        {
            bool rtrn = false;
            try
            {
                string scalarValue = "";
                using (var dbc = new SQLiteConnection( SQLyt() ))
                    scalarValue = dbc.ExecuteScalar( sql ) as string;

                if (scalarValue != null && (scalarValue.Contains( "1" )
                    || scalarValue.ToLower().Contains( "true" ))) rtrn = true;
            }
            catch (SqlException ex) { LogR.Norm( $"{ex}" ); }
            catch (Exception ex) { LogR.Norm( $"{ex}" ); }

            return rtrn;
        }

        public static string GetSQLiteVersion() { return OneT_Valu<string>( "SELECT sqlite_version() " ); }
        public static bool SaveKVP( string key, string valu )
        {
            string sql = $"INSERT INTO {TBL.KVP} ( KVP_key,KVP_valu ) " +
                         $"  VALUES( '{key}', '{valu}' ) ";
            return BoolValue( sql );
        }
        public static T KVP_GetValu<T>( string key )
        {
            string sql = $"SELECT KVP_valu FROM {TBL.KVP} WHERE KVP_KEY='{key}' ";
            var t_Valu = OneT_Valu<T>( sql );
            return t_Valu;
        }
        public static void UpdateKVP( string key, string state )
        {
            string sql = $"UPDATE {TBL.KVP} SET KVP_valu='{state}'" +
                         $" WHERE KVP_key='{key}' ";
            JustSQL( sql );
        }
        public static void DeleteKVPrecord( string key )
        {
            string sql = $"DELETE FROM {TBL.KVP}  WHERE KVP_key='{key}' ";
            JustSQL( sql );
        }
        public static void SaveExeSpec( ExeSpec t )
        {
            ClearExeSpec();
            string args = t.Arguments is null ? "no args; run defaults" : t.Arguments.ToString();
            LogR.Norm( $"|=DB=| saving scrcpy.exe FQN: {t.ScrcpyFQN}\nexe args: {args}" );
            string sql = $"INSERT INTO {TBL.ExeSpec} ( ScrcpyFQN,Arguments ) " +
                         $"  VALUES( '{t.ScrcpyFQN}', '{t.Arguments}' ) ";
            JustSQL( sql );
        }
        public static ExeSpec GetExeSpec()
        {
            string sql = $"SELECT * FROM {TBL.ExeSpec} WHERE RecID='{Xfer.DBaMod}' ";
            return TypedModel<ExeSpec>( sql );
        }
        public static void ClearExeSpec()
        {
            LogR.Norm( $"|=DB=| {TBL.ExeSpec} clear" );
            string sql = $"DELETE FROM {TBL.ExeSpec}  ";
            JustSQL( sql );
        }
        public static void SaveArgs_Model( Args args )
        {
            LogR.Norm( $"|=DB=| {TBL.Args} save {args.Arg}\n{args.Explain}" );
            string sql = $"INSERT INTO {TBL.Args}  ( Role,Arg,Explain ) VALUES(  @Role,@Arg,@Explain ) ";
            TypedModelSave<Args>( sql, args );
        }
        public static void SaveArgs( string role, string arg, string explain )
        {
            LogR.Norm( $"|=DB=| {TBL.Args} save {arg}\n{explain}" );
            string sql;
            if (role == Xfer.R_arg) sql = $"INSERT INTO {TBL.Args} ( Arg,Explain ) VALUES( '{arg}', '{explain}' ) ";
            else sql = $"INSERT INTO {TBL.Args}  ( Role,Arg,Explain ) VALUES( '{role}', '{arg}', '{explain}' ) ";
            JustSQL( sql );
        }
        public static bool CheckArg( string name )
        {
            string sql = $"SELECT Arg FROM {TBL.Args}" +
                         $" WHERE Arg LIKE '%{name}%' ";
            return OneT_Valu<string>( sql ) != null;
        }
        public static string GetVer()
        {
            string name = "VER";
            string sql = $"SELECT Arg FROM {TBL.Args}" +
                         $" WHERE Role='{name}' ";
            return OneT_Valu<string>( sql ) ?? string.Empty;
        }
        public static List<Args> GetArgs()
        {
            string sql = $"SELECT * FROM {TBL.Args}";
            return TypedList<Args>( sql );
        }
        public static void ClearArgs()
        {
            LogR.Norm( $"|=DB=| {TBL.Args} clear" );
            string sql = $"DELETE FROM {TBL.Args}  ";
            JustSQL( sql );
        }
        public static void StartPos_Save( StartPos model )
        {
            string sql = $"INSERT INTO {TBL.StartPos} " +
                                   "( ScreenName, FormName, Form_L, Form_T ) " +
                          "  VALUES( @ScreenName,@FormName,@Form_L,@Form_T ) ";
            TypedModelSave<StartPos>( sql, model );
        }
        public static StartPos StartPos_Load( string scr, string form )
        {
            string sql = $"SELECT * FROM {TBL.StartPos} WHERE ScreenName='{scr}' AND FormName='{form}' ";
            return TypedModel<StartPos>( sql );
        }
        public static void StartPos_Delete( string scr, string form )
        {
            string sql = $"DELETE FROM {TBL.StartPos} WHERE ScreenName='{scr}' AND FormName='{form}' ";
            JustSQL( sql );
        }
    }
    public class StartPos
    {
        public string ScreenName { get; set; }
        public string FormName { get; set; }
        public int Form_L { get; set; }
        public int Form_T { get; set; }
    }
    public class ExeSpec
    {
        public string RecID { get; set; }
        public string ScrcpyFQN { get; set; }
        public string Arguments { get; set; }
    }
    public class Args
    {
        public string Role { get; set; }
        public string Arg { get; set; }
        public string Explain { get; set; }
    }
    public class States
    {
        public const string Fail = "Error";
        public const string Info = "Info";
        public const string OK = "OK";
        public const string Warn = "Warn";
    }

    [DebuggerStepThrough()]
    public static class LogR
    {
        private static StringBuilder _SB4log = new StringBuilder();

        public static void Norm( string logTxt )
        {
            // Ensure directory and file exist before trying to open
            try
            {
                var logDir = Path.GetDirectoryName( Paths.AppLog );
                if (!string.IsNullOrEmpty( logDir )) Directory.CreateDirectory( logDir );
                if (!File.Exists( Paths.AppLog )) File.WriteAllText( Paths.AppLog, string.Empty );
            }
            catch { /* best-effort only */ }

            int retries = 0, maxRetries = 10;
            while (retries < maxRetries)
            {
                try
                {
                    // Source - https://stackoverflow.com/a/876513
                    // ensure file is available
                    using (FileStream fs = File.Open( Paths.AppLog, FileMode.Open, FileAccess.Read, FileShare.None ))
                    { fs.Close(); }
                    Console.WriteLine( $"{logTxt}" );
                    _SB4log.AppendLine( $"{logTxt}" );
                    File.AppendAllText( Paths.AppLog, $"\n{logTxt}" );
                    _SB4log.Clear();
                    retries = maxRetries;
                }
                catch (Exception)
                {
                    retries++;
                    if (retries < maxRetries) Thread.Sleep( 150 );
                }
            }
        }
    }

    [DebuggerStepThrough()]
    public class CstmBorders
    {
        static Label _bord_L;
        static Label _bord_T;
        static Label _bord_R;
        static Label _bord_B;

        public static void Vari_W( Control ctrl, int bordW, Color color )
        {
            Color bordColor = color;
            _bord_L = new Label
            {
                BackColor = bordColor,
                Size = new Size( bordW, ctrl.Height ),
                Location = new Point( 0, 0 ),
                Name = "bord_Left"
            };
            ctrl.Controls.Add( _bord_L );

            _bord_T = new Label
            {
                BackColor = bordColor,
                Size = new Size( ctrl.Width, bordW ),
                Location = new Point( 0, 0 ),
                Name = "bord_Top"
            };
            ctrl.Controls.Add( _bord_T );

            _bord_R = new Label
            {
                BackColor = bordColor,
                Size = new Size( bordW, ctrl.Height ),
                Location = new Point( ctrl.Width - bordW, 0 ),
                Name = "bord_Right"
            };
            ctrl.Controls.Add( _bord_R );

            _bord_B = new Label
            {
                BackColor = bordColor,
                Size = new Size( ctrl.Width, bordW ),
                Location = new Point( 0, ctrl.Height - bordW ),
                Name = "bord_Bottom"
            };
            ctrl.Controls.Add( _bord_B );
        }

        public static void ClearBorder( Control bordCaller )
        {
            List<Label> bordLBL = new List<Label>();
            foreach (var lbl in bordCaller.Controls.OfType<Label>())
                if (lbl.Name.Contains( "bord_" )) bordLBL.Add( lbl );
            foreach (var lbl in bordLBL)
            {
                bordCaller.Controls.Remove( lbl );
                lbl.Dispose();
            }
        }
    }

    [DebuggerStepThrough()]
    public class CstmStyles
    {
        public static Font Std_9_R { get; } = new Font( "Segoe UI", 9, FontStyle.Regular );
        public static Font Std_9_B { get; } = new Font( "Segoe UI", 9, FontStyle.Bold );
        public static Font Std_9_I { get; } = new Font( "Segoe UI", 9, FontStyle.Italic );

        #region Colors
        public static Color Blue { get; } = Color.FromArgb( 0, 52, 196 );
        public static Color Brown { get; } = Color.FromArgb( 99, 82, 7 );
        public static Color Gray { get; } = Color.FromArgb( 100, 98, 108 );
        public static Color Green { get; } = Color.FromArgb( 3, 99, 3 );
        public static Color Red { get; } = Color.FromArgb( 150, 0, 0 );
        public static Color Yellow { get; } = Color.FromArgb( 203, 163, 1 );

        public static Color PaleBlue { get; } = Color.FromArgb( 216, 216, 243 );

        public static Color PaleGray { get; } = Color.FromArgb( 223, 223, 225 );
        public static Color PaleGreen { get; } = Color.FromArgb( 225, 254, 225 );
        public static Color PaleRed { get; } = Color.FromArgb( 255, 240, 240 );
        public static Color PaleYellow { get; } = Color.FromArgb( 250, 248, 239 );
        #endregion

        #region BUTTONS
        public static void BTN_Disabled( Button btnName )
        {
            btnName.BackColor = Color.DarkGray;
            btnName.Font = Std_9_R;
            btnName.FlatStyle = FlatStyle.Popup;
            btnName.Enabled = false;
            btnName.Cursor = Cursors.Default;
            Application.DoEvents();
        }
        public static void BTN_Enabled_Default( Button btnName )
        {
            btnName.BackColor = Color.FromArgb( 250, 246, 240 );
            btnName.BackColor = Color.WhiteSmoke;
            btnName.Font = Std_9_R;
            btnName.Enabled = true;
            btnName.Cursor = Cursors.Hand;
            btnName.MouseEnter += BTN_MouseEnter_Default;
            btnName.MouseDown += BTN_MouseDown_Default;
            btnName.MouseUp += BTN_MouseUp_Default;
            btnName.MouseLeave += BTN_MouseLeave_Default;
        }
        private static void BTN_MouseEnter_Default( object sender, EventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb( 198, 186, 129 );
                btn.Font = Std_9_B;
            }
        }
        private static void BTN_MouseDown_Default( object sender, EventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb( 229, 224, 199 );
                btn.Font = Std_9_B;
            }
        }
        private static void BTN_MouseUp_Default( object sender, EventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb( 214, 206, 165 );
                btn.Font = Std_9_B;
            }
        }
        private static void BTN_MouseLeave_Default( object sender, EventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb( 214, 206, 165 );
                btn.Font = Std_9_R;
            }
        }

        public static void BTN_Enabled_Green( Button btnName )
        {
            btnName.BackColor = PaleGreen;
            btnName.Font = Std_9_R;
            btnName.Enabled = true;
            btnName.FlatStyle = FlatStyle.Popup;
            btnName.Cursor = Cursors.Hand;
            btnName.MouseEnter += BTN_MouseEnter_Green;
            btnName.MouseDown += BTN_MouseDown_Green;
            btnName.MouseUp += BTN_MouseUp_Green;
            btnName.MouseLeave += BTN_MouseLeave_Green;
        }
        private static void BTN_MouseDown_Green( object sender, MouseEventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb( 240, 249, 240 );
                btn.Font = Std_9_B;
                btn.Cursor = Cursors.Hand;
            }
        }
        private static void BTN_MouseEnter_Green( object sender, EventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb( 186, 227, 186 );
                btn.Font = Std_9_B;
                btn.Cursor = Cursors.Hand;
            }
        }
        private static void BTN_MouseUp_Green( object sender, MouseEventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = PaleGreen;
                btn.Font = Std_9_B;
                btn.Cursor = Cursors.Hand;
            }
        }
        private static void BTN_MouseLeave_Green( object sender, EventArgs e )
        {
            if (sender is Button btn)
            {
                btn.BackColor = PaleGreen;
                btn.Font = Std_9_R;
                btn.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region Leveraged Tooltip
        private static Font _txtFont = new Font( "Tahoma", 10 );
        private static Color _ttColor = default;
        private static ToolTip _customTT;
        public static ToolTip Custom_tt( string txt, Control ctrl, int dur, bool error )
        {
            _ttColor = error ? PaleRed : PaleBlue;

            _customTT = new ToolTip();
            _customTT.OwnerDraw = true;
            _customTT.Popup += Custom_tt_Popup;
            _customTT.Draw += Custom_tt_Draw;
            _customTT.UseAnimation = true;
            _customTT.UseFading = true;
            _customTT.ShowAlways = false;

            Size txtSize = TextRenderer.MeasureText( txt, _txtFont );

            // postioniing is relative to ctrl rectangle NOT ctrl Bounds on parent form
            int pos_X, pos_Y;
            pos_X = (ctrl.Width - txtSize.Width) / 2;
            pos_Y = -(txtSize.Height + 5);

            LogR.Norm( $"in Custom_tt\n tt control {ctrl.Name}, {ctrl.Bounds}\ntt position pos_X {pos_X} pos_Y {pos_Y}." );

            // Avoid creating window handles or calling ToolTip.Show() when running under unit tests
            // or when the control does not have a valid handle. In those cases return the ToolTip
            // instance without calling Show so callers can still call Hide/Dispose safely.
            if (ctrl == null || !ctrl.IsHandleCreated || WinCtrl.IsRunningUnderUnitTests())
            {
                return _customTT;
            }

            _customTT.Show( txt, ctrl, pos_X, pos_Y, dur );

            return _customTT;
        }
        private static void Custom_tt_Popup( object sender, PopupEventArgs e )
        {
            using (_txtFont)
            {
                string toolTipText = _customTT?.GetToolTip( e.AssociatedControl ) ?? string.Empty;
                e.ToolTipSize = TextRenderer.MeasureText( toolTipText, _txtFont );
            }
        }
        private static void Custom_tt_Draw( object sender, DrawToolTipEventArgs e )
        {
            e.DrawBorder();

            Color bgFill = _ttColor; ;
            SolidBrush _tt_copiedBrush = new SolidBrush( bgFill );
            e.Graphics.FillRectangle( _tt_copiedBrush, e.Bounds );

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                using (Font f = new Font( "Tahoma", 9 ))
                {
                    e.Graphics.DrawString( e.ToolTipText, f,
                        SystemBrushes.ActiveCaptionText, e.Bounds, sf );
                }
            }
        }
        #endregion

    }
    public class WinCtrl
    {
        #region Window Manipulation Events
        #region move window prerequisites
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport( "user32.dll" )]
        public static extern int SendMessage( IntPtr hWnd, int msg, int wParam, int lParam );
        [DllImport( "user32.dll" )]
        public static extern bool ReleaseCapture();
        #endregion

        #region move window 
        public static void MoveWin( Control ctrl, IntPtr Handle )
        {
            ctrl.MouseMove += ( sender, ea ) => MoveWinPB_MouseMove( sender, ea, Handle );
            ctrl.MouseEnter += MoveWinPB_MouseEnter;
            ctrl.MouseLeave += MoveWinPB_MouseLeave;
        }
        public static void MoveWinPB_MouseMove( object sender, MouseEventArgs e, IntPtr Handle )
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage( Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0 );
            }
        }
        public static void MoveWinPB_MouseEnter( object sender, EventArgs e )
        {
            PictureBox pb = (PictureBox)sender;
            pb.Cursor = Cursors.Hand;
            pb.BackgroundImage = Properties.Resources.moveWin_MO;
            pb.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public static void MoveWinPB_MouseLeave( object sender, EventArgs e )
        {
            PictureBox pb = (PictureBox)sender;
            pb.Cursor = Cursors.Default;
            pb.BackgroundImage = Properties.Resources.moveWin;
            pb.BackgroundImageLayout = ImageLayout.Stretch;
        }
        #endregion

        #region minimize window 
        public static void MinWin( Control ctrl )
        {
            ctrl.Click += ( sender, ea ) => MinWinPB_Click( sender, ea );
            ctrl.MouseEnter += MinWinPB_MouseEnter;
            ctrl.MouseLeave += MinWinPB_MouseLeave;
        }
        public static void MinWinPB_Click( object sender, EventArgs e )
        {
            Form parentForm = ((PictureBox)sender).FindForm();
            parentForm.WindowState = FormWindowState.Minimized;
        }
        public static void MinWinPB_MouseEnter( object sender, EventArgs e )
        {
            PictureBox pb = (PictureBox)sender;
            pb.Cursor = Cursors.Hand;
            pb.BackgroundImage = Properties.Resources.Minimize_MO;
            pb.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public static void MinWinPB_MouseLeave( object sender, EventArgs e )
        {
            PictureBox pb = (PictureBox)sender;
            pb.Cursor = Cursors.Default;
            pb.BackgroundImage = Properties.Resources.Minimize;
            pb.BackgroundImageLayout = ImageLayout.Stretch;
        }
        #endregion

        #region close window 
        public static void CloseWin( Control ctrl )
        {
            ctrl.Click += ( sender, ea ) => CloseWinPB_Click( sender, ea );
            ctrl.MouseEnter += CloseWinPB_MouseEnter;
            ctrl.MouseLeave += CloseWinPB_MouseLeave;
        }
        public static void CloseWinPB_Click( object sender, EventArgs e )
        {
            Form parentForm = ((PictureBox)sender).FindForm();
            parentForm.Close();
        }
        public static void CloseWinPB_MouseEnter( object sender, EventArgs e )
        {
            PictureBox pb = (PictureBox)sender;
            pb.Cursor = Cursors.Hand;
            pb.BackgroundImage = Properties.Resources.Close_MO;
            pb.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public static void CloseWinPB_MouseLeave( object sender, EventArgs e )
        {
            PictureBox pb = (PictureBox)sender;
            pb.Cursor = Cursors.Default;
            pb.BackgroundImage = Properties.Resources.Close;
            pb.BackgroundImageLayout = ImageLayout.Stretch;
        }
        #endregion
        #endregion

        public static void Call_Welcome( ICaller passThru )
        {
            Welcome frm = new Welcome();
            frm.Show();
        }
        public static void Call_DataEntry( ICaller passThru )
        {
            DataEntry frm = new DataEntry( caller: passThru );
            frm.Show();
        }
        public static void Call_RunSCRCPY( ICaller passThru, bool noDelay )
        {
            RunSCRCPY frm = new RunSCRCPY( passThru, noDelay );
            frm.Show();
        }
        public static string CstmmMB_Call( ICaller passThru )
        {
            string cmbReturn = null;
            #region Ensure ShowDialog runs on UI thread of the owner form
            if (passThru.I_form != null && passThru.I_form.InvokeRequired)
            {
                passThru.I_form.Invoke( (Action)(() =>
                {
                    using (var cmb = new CstmMsgBox( passThru )) { cmb.ShowDialog(); cmbReturn = cmb.MBreturn; }
                }) );
            }
            else using (var cmb = new CstmMsgBox( passThru )) { cmb.ShowDialog(); cmbReturn = cmb.MBreturn; }
            #endregion

            return cmbReturn;
        }

        public static void RegisterWin( ICaller regCaller )
        {
            if (regCaller == null || regCaller.I_form == null) return;
            if (Xfer.OpenWins == null) Xfer.OpenWins = new List<ICaller>();

            Xfer.OpenWins.Add( regCaller );
            string openforms = string.Join( ", ", Xfer.OpenWins.Select( x => x.I_form.Name ) );
            LogR.Norm( $"\n==> \"{regCaller.I_form.Name}\" opened {Xfer.RunTime}" +
                         $" - OpenForms={Xfer.OpenWins.Count}: {openforms}" );
        }
        public static void UnregisterWin( ICaller caller )
        {
            if (caller == null || caller.I_form == null) return;
            if (Xfer.OpenWins == null) return;

            try
            {
                string openforms = string.Join( ", ", Xfer.OpenWins.Select( x => x.I_form.Name ) );
                LogR.Norm( $"close {caller.I_form.Name} requested; current Open Forms={Xfer.OpenWins.Count}:{openforms}" );

                Xfer.OpenWins.Remove( caller );

                openforms = string.Join( ", ", Xfer.OpenWins.Select( x => x.I_form.Name ) );
                LogR.Norm( $"=> \"{caller.I_form.Name}\" closed  {Xfer.RunTime}" +
                           $" - remaining Open Forms={Xfer.OpenWins.Count}: {openforms}" );

                if (Xfer.OpenWins.Count == 0)
                {
                    LogR.Norm( "all windows closed -- exiting FoneCall" );
                    if (!IsRunningUnderUnitTests()) Application.Exit();
                }
            }
            catch (Exception ex) { LogR.Norm( $"error in UnregisterWin {ex}" ); }
        }

        // Detect common test runners so we can avoid calling Application.Exit() during unit tests.
        // Not foolproof but prevents test runs from inadvertently terminating the test host.
        public static bool IsRunningUnderUnitTests()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                return assemblies.Any(a =>
                {
                    var n = a.FullName ?? string.Empty;
                    return n.StartsWith( "Microsoft.VisualStudio.TestPlatform", StringComparison.OrdinalIgnoreCase )
                        || n.IndexOf( "nunit", StringComparison.OrdinalIgnoreCase ) >= 0
                        || n.IndexOf( "xunit", StringComparison.OrdinalIgnoreCase ) >= 0
                        || n.IndexOf( "Microsoft.VisualStudio.QualityTools", StringComparison.OrdinalIgnoreCase ) >= 0;
                } );
            }
            catch
            {
                return false;
            }
        }

        public static void SaveWinPos( Form form )
        {
            Screen scr = Screen.FromControl( form );
            StartPos sp = DBC.StartPos_Load( scr.DeviceName, form.Name );
            if (sp != null) DBC.StartPos_Delete( scr.DeviceName, form.Name );

            StartPos model = new StartPos
            {
                ScreenName = scr.DeviceName,
                FormName = form.Name,
                Form_L = form.Left,
                Form_T = form.Top,
            };
            DBC.StartPos_Save( model );
        }
        public static void RestoreDBwinPos( Form form )
        {
            Screen scr = Screen.FromControl( form );
            StartPos sp = DBC.StartPos_Load( scr.DeviceName, form.Name );
            if (sp is null)
            {
                int left = (scr.WorkingArea.Width - form.Width) / 2;
                int top = (scr.WorkingArea.Height - form.Height) / 2;
                form.Location = new Point( left, top );
            }
            else form.Location = new Point( sp.Form_L, sp.Form_T );
        }

    }
}
