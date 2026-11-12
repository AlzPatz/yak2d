using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Silk.NET.Core.Loader;

namespace Yak2D.Core
{
    /// <summary>
    /// Silk.NET 2.23's DefaultPathResolver only searches runtimes/{rid}/native once it can map the RID it reads from
    /// /etc/os-release (e.g. ubuntu.24.04-x64) down to a portable one (linux-x64). Neither its hard-coded distro list
    /// (DefaultPathResolver._linuxRiDs, no "ubuntu") nor the shared framework's RID graph (no ubuntu.24.04) does that,
    /// so on Ubuntu the SDL2, shaderc and spirv-cross natives shipped beside the app are never found.
    /// This adds a resolver that tries runtimes/{os}-{arch}/native under AppContext.BaseDirectory.
    /// Call before anything touches Silk (SDL window creation, NeoVeldrid.SPIRV).
    /// </summary>
    internal static class SilkNativeLibraryPaths
    {
        private static bool _registered;
        private static readonly object _lock = new object();

        public static void Register()
        {
            lock (_lock)
            {
                if (_registered) return;
                _registered = true;
                if (PathResolver.Default is DefaultPathResolver resolver)
                {
                    resolver.Resolvers.Add(PortableRuntimesFolder);
                }
            }
        }

        private static IEnumerable<string> PortableRuntimesFolder(string name)
        {
            var file = Path.GetFileName(name);
            var rid = PortableRid();
            if (string.IsNullOrEmpty(file) || rid == null || string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                return Enumerable.Empty<string>();
            }
            var path = Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native", file);
            return File.Exists(path) ? new[] { path } : Enumerable.Empty<string>();
        }

        private static string PortableRid()
        {
            var os = OperatingSystem.IsWindows() ? "win"
                   : OperatingSystem.IsMacOS() ? "osx"
                   : OperatingSystem.IsLinux() ? "linux"
                   : null;
            if (os == null) return null;
            var arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm64 => "arm64",
                Architecture.Arm => "arm",
                _ => null
            };
            return arch == null ? null : os + "-" + arch;
        }
    }
}
