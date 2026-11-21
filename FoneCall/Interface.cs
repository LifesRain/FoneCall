// FoneCall, a scrcpy UI. Copyright (c) 2026 Lifesain
// Licensed under the PolyForm Noncommercial License 1.0.0.
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//     <https://polyformproject.org/licenses/noncommercial/1.0.0>
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

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
        string I_LinkIntroTxt { get; set; }
        string I_LinkTxt { get; set; }
        string I_LinkLastTxt { get; set; }
        string I_LinkURL { get; set; }
        bool I_migrated { get; set; }
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
        public string I_LinkIntroTxt { get; set; }
        public string I_LinkTxt { get; set; }
        public string I_LinkLastTxt { get; set; }
        public string I_LinkURL { get; set; }
        public bool I_migrated { get; set; } = false;
    }
}
