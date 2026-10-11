// CSharp FoneCall\Services\DefaultDatabase.cs
using System;

namespace FoneCall
{
    public class DefaultDatabase : UTS_IDatabase
    {
        public string GetVer() => DBC.GetVer();
        public ExeSpec GetExeSpec() => DBC.GetExeSpec();
        public bool CheckArg(string name) => DBC.CheckArg(name);
        public void SaveExeSpec(ExeSpec spec) => DBC.SaveExeSpec(spec);
    }
}