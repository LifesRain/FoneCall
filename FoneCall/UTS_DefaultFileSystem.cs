// CSharp FoneCall\Services\DefaultFileSystem.cs
using System.IO;

namespace FoneCall
{
    public class UTS_DefaultFileSystem : UTS_IFileSystem
    {
        public bool Exists(string path) => File.Exists(path);
    }
}