// CSharp FoneCall\Services\DefaultFileSystem.cs
using System.IO;

namespace FoneCall.Services
{
    public class DefaultFileSystem : IFileSystem
    {
        public bool Exists(string path) => File.Exists(path);
    }
}