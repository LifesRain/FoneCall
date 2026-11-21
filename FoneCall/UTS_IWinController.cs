// FoneCall, a scrcpy UI. Copyright (c) 2026 Lifesain
// Licensed under the PolyForm Noncommercial License 1.0.0.
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//     <https://polyformproject.org/licenses/noncommercial/1.0.0>
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

using System;
using System.Windows.Forms;

namespace FoneCall
{
    internal interface UTS_IWinController
    {
        void RestoreDBwinPos( Form f );
        void RegisterWin( ICaller caller );
        void MoveWin( Control ctrl, IntPtr handle );
        void MinWin( Control ctrl );
        void CloseWin( Control ctrl );
        void Call_RunSCRCPY( ICaller passThru, bool noDelay );
        string CstmmMB_Call( ICaller passThru );
        void SaveWinPos( Form f );
        void UnregisterWin( ICaller caller );
        bool IsRunningUnderUnitTests();
    }
}