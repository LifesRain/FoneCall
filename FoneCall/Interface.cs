using System.Windows.Forms;

namespace FoneCall
{
    public interface ICaller
    {
        Form I_form { get; set; }
        string I_MBcaller { get; set; }
        string I_MsgType { get; set; }
        string I_Msg { get; set; }
        string I_Txt_Btn1 { get; set; }
        string I_Txt_Btn2 { get; set; }
        string I_Txt_Btn3 { get; set; }
    }
    public class MsgBoxImplement : Form, ICaller
    {
        public Form I_form { get; set; }
        public string I_MBcaller { get; set; }
        public string I_MsgType { get; set; }
        public string I_Msg { get; set; }
        public string I_Txt_Btn1 { get; set; }
        public string I_Txt_Btn2 { get; set; }
        public string I_Txt_Btn3 { get; set; }
    }
}
