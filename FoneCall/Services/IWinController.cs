// CSharp FoneCall\Services\IWinController.cs
using System;
using System.Windows.Forms;

namespace FoneCall.Services
{
    public interface IWinController
    {
        void RestoreDBwinPos(Form f);
        void RegisterWin(ICaller caller);
        void MoveWin(Control ctrl, IntPtr handle);
        void MinWin(Control ctrl);
        void CloseWin(Control ctrl);
        void Call_RunSCRCPY(ICaller passThru, bool noDelay);
        string CstmmMB_Call(ICaller passThru);
        void SaveWinPos(Form f);
        void UnregisterWin(ICaller caller);
        bool IsRunningUnderUnitTests();
    }
}