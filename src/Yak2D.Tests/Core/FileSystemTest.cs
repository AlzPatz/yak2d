using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class FileSystemTest
    {
        [Fact]
        public void FileSystem_Exists_CatchesAndReturnsCleanlyOnEmptyInput()
        {
            IFileSystem file = new FileSystem();

            Assert.False(file.Exists(""));
        }

        [Fact]
        public void FileSystem_Exists_CatchesAndReturnsCleanlyOnNullInput()
        {
            IFileSystem file = new FileSystem();

            Assert.False(file.Exists(null));
        }

        [Fact]
        public void FileSystem_GetFilesInDirectory_CatchesAndReturnsCleanlyOnEmptyInput()
        {
            IFileSystem file = new FileSystem();

            Assert.Null(file.GetFilesInDirectory("", "*", System.IO.SearchOption.AllDirectories));
        }

        [Fact]
        public void FileSystem_GetFilesInDirectory_CatchesAndReturnsCleanlyOnNullInput()
        {
            IFileSystem file = new FileSystem();

            Assert.Null(file.GetFilesInDirectory(null, "*", System.IO.SearchOption.AllDirectories));
        }

        [Fact]
        public void FileSystem_OpenRead_CatchesAndReturnsCleanlyOnEmptyInput()
        {
            IFileSystem file = new FileSystem();

            Assert.Null(file.OpenRead(""));
        }

        [Fact]
        public void FileSystem_OpenRead_CatchesAndReturnsCleanlyOnNullInput()
        {
            IFileSystem file = new FileSystem();

            Assert.Null(file.OpenRead(null));
        }

        [Fact]
        public void FileSystem_ReadAllBytes_CatchesAndReturnsCleanlyOnEmptyInput()
        {
            IFileSystem file = new FileSystem();

            Assert.Null(file.ReadAllBytes(""));
        }

        [Fact]
        public void FileSystem_ReadAllBytes_CatchesAndReturnsCleanlyOnNullInput()
        {
            IFileSystem file = new FileSystem();

            Assert.Null(file.ReadAllBytes(null));
        }
    }
}