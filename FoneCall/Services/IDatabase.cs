// CSharp FoneCall\Services\IDatabase.cs
using System;

namespace FoneCall.Services
{
    public interface IDatabase
    {
        string GetVer();
        ExeSpec GetExeSpec();
        bool CheckArg(string name);
        void SaveExeSpec(ExeSpec spec);
    }
}