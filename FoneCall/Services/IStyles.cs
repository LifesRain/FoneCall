// CSharp FoneCall\Services\IStyles.cs
using System.Drawing;
using System.Windows.Forms;

namespace FoneCall.Services
{
    public interface IStyles
    {
        Font Std_9_R { get; }
        Color PaleRed { get; }
        ToolTip Custom_tt(string txt, Control ctrl, int dur, bool error);
        void BTN_Disabled(Button btn);
        void BTN_Enabled_Green(Button btn);
        void BTN_Enabled_Default(Button btn);
    }
}