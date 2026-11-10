// CSharp FoneCall\Services\DefaultDatabase.cs
namespace FoneCall
{
    internal class DefaultDatabase : UTS_IDatabase
    {
        public string GetVer() => DBC.GetVer();
        public ExeSpec GetExeSpec() => DBC.GetExeSpec();
        public bool CheckArg( string name ) => DBC.CheckArg( name );
        public void SaveExeSpec( ExeSpec spec ) => DBC.SaveExeSpec( spec );
    }
}