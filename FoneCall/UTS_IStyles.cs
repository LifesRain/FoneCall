// FoneCall, a scrcpy UI. Copyright (c) 2026 Lifesain
// Licensed under the PolyForm Noncommercial License 1.0.0.
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//     <https://polyformproject.org/licenses/noncommercial/1.0.0>
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

using System.Drawing;
using System.Windows.Forms;

namespace FoneCall
{
    internal interface UTS_IStyles
    {
        Font Std_9_R { get; }
        Color PaleRed { get; }
        ToolTip Custom_tt( string txt, Control ctrl, int dur, bool error );
        void BTN_Disabled( Button btn );
        void BTN_Enabled_Green( Button btn );
        void BTN_Enabled_Default( Button btn );
    }
}