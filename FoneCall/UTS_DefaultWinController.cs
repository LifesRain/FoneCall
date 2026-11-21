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
    internal class DefaultWinController : UTS_IWinController
    {
        public void RestoreDBwinPos( Form f ) => WinCtrl.RestoreDBwinPos( f );
        public void RegisterWin( ICaller caller ) => WinCtrl.RegisterWin( caller );
        public void MoveWin( Control ctrl, IntPtr handle ) => WinCtrl.MoveWin( ctrl, handle );
        public void MinWin( Control ctrl ) => WinCtrl.MinWin( ctrl );
        public void CloseWin( Control ctrl ) => WinCtrl.CloseWin( ctrl );
        public void Call_RunSCRCPY( ICaller passThru, bool noDelay ) => WinCtrl.Call_RunSCRCPY( passThru, noDelay );
        public string CstmmMB_Call( ICaller passThru ) => WinCtrl.CstmmMB_Call( passThru );
        public void SaveWinPos( Form f ) => WinCtrl.SaveWinPos( f );
        public void UnregisterWin( ICaller caller ) => WinCtrl.UnregisterWin( caller );
        public bool IsRunningUnderUnitTests() => WinCtrl.IsTesting();
    }
}