using System.Collections.Generic;
using System.IO;

namespace Yak2D.Internal
{
    public interface IAssembly
    {
        string Name { get; }
        List<string> GetManifestResourceNames();
        Stream GetManifestResourceStream(string resourceName);
    }
}
