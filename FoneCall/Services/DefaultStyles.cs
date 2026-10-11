// CSharp FoneCall\Services\DefaultStyles.cs
using System.Drawing;
using System.Windows.Forms;

namespace FoneCall.Services
{
    public class DefaultStyles : IStyles
    {
        public Font Std_9_R => CstmStyles.Std_9_R;
        public Color PaleRed => CstmStyles.PaleRed;
        public ToolTip Custom_tt(string txt, Control ctrl, int dur, bool error) => CstmStyles.Custom_tt(txt, ctrl, dur, error);
        public void BTN_Disabled(Button btn) => CstmStyles.BTN_Disabled(btn);
        public void BTN_Enabled_Green(Button btn) => CstmStyles.BTN_Enabled_Green(btn);
        public void BTN_Enabled_Default(Button btn) => CstmStyles.BTN_Enabled_Default(btn);
    }
}