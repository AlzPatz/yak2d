using System.IO;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class FileSystem : IFileSystem
    {
        public bool Exists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return File.Exists(path);
        }

        public string[] GetFilesInDirectory(string path, string searchPattern, SearchOption searchOptions)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if (searchPattern == null)
            {
                searchPattern = "";
            }

            return Directory.GetFiles(path, searchPattern, searchOptions);
        }

        public FileStream OpenRead(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return File.OpenRead(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return File.ReadAllBytes(path);
        }
    }
}