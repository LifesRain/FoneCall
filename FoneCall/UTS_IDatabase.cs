// CSharp FoneCall\Services\IDatabase.cs
using System;

namespace FoneCall
{
    public interface UTS_IDatabase
    {
        string GetVer();
        ExeSpec GetExeSpec();
        bool CheckArg(string name);
        void SaveExeSpec(ExeSpec spec);
    }
}