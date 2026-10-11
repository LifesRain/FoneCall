// CSharp FoneCall\Services\DefaultWinController.cs
using System;
using System.Windows.Forms;

namespace FoneCall.Services
{
    public class DefaultWinController : IWinController
    {
        public void RestoreDBwinPos(Form f) => WinCtrl.RestoreDBwinPos(f);
        public void RegisterWin(ICaller caller) => WinCtrl.RegisterWin(caller);
        public void MoveWin(Control ctrl, IntPtr handle) => WinCtrl.MoveWin(ctrl, handle);
        public void MinWin(Control ctrl) => WinCtrl.MinWin(ctrl);
        public void CloseWin(Control ctrl) => WinCtrl.CloseWin(ctrl);
        public void Call_RunSCRCPY(ICaller passThru, bool noDelay) => WinCtrl.Call_RunSCRCPY(passThru, noDelay);
        public string CstmmMB_Call(ICaller passThru) => WinCtrl.CstmmMB_Call(passThru);
        public void SaveWinPos(Form f) => WinCtrl.SaveWinPos(f);
        public void UnregisterWin(ICaller caller) => WinCtrl.UnregisterWin(caller);
        public bool IsRunningUnderUnitTests() => WinCtrl.IsRunningUnderUnitTests();
    }
}