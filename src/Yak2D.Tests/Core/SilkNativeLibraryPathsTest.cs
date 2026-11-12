using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Silk.NET.Core.Loader;
using Xunit;
using Yak2D.Core;

namespace Yak2D.Tests
{
    public class SilkNativeLibraryPathsTest
    {
        private static bool IsLinuxX64 => OperatingSystem.IsLinux() && RuntimeInformation.ProcessArchitecture == Architecture.X64;

        [Theory]
        [InlineData("libshaderc_shared.so")]
        [InlineData("libspirv-cross.so")]
        [InlineData("libSDL2-2.0.so")]
        public void SilkNativeLibraryPaths_Register_AddsPortableRuntimesFolderCandidate(string library)
        {
            Assert.SkipUnless(IsLinuxX64, "Only checked on linux-x64");

            SilkNativeLibraryPaths.Register();

            var expected = Path.Combine(AppContext.BaseDirectory, "runtimes", "linux-x64", "native", library);

            Assert.Contains(expected, PathResolver.Default.EnumeratePossibleLibraryLoadTargets(library).ToList());
        }

        [Fact]
        public void SilkNativeLibraryPaths_RegisterTwice_AddsResolverOnlyOnce()
        {
            var resolver = Assert.IsAssignableFrom<DefaultPathResolver>(PathResolver.Default);

            SilkNativeLibraryPaths.Register();
            var count = resolver.Resolvers.Count;
            SilkNativeLibraryPaths.Register();

            Assert.Equal(count, resolver.Resolvers.Count);
        }
    }
}
