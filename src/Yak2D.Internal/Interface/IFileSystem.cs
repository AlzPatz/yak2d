using System.IO;

namespace Yak2D.Internal
{
    public interface IFileSystem
    {
        bool Exists(string path);
        FileStream OpenRead(string path);
        string[] GetFilesInDirectory(string path, string searchPattern, SearchOption searchOptions);
        byte[] ReadAllBytes(string filePath);
    }
}
