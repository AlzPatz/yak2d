using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class ApplicationAssembly : IApplicationAssembly
    {
        public Assembly Assembly { get; private set; }

        public string Name => Assembly.GetName().Name;

        public ApplicationAssembly(Assembly applicationAssembly)
        {
            Assembly = applicationAssembly;
        }

        public List<string> GetManifestResourceNames()
        {
            return Assembly.GetManifestResourceNames().ToList();
        }

        public Stream GetManifestResourceStream(string resourceName)
        {
            if (GetManifestResourceNames().Contains(resourceName))
            {
                return Assembly.GetManifestResourceStream(resourceName);
            }

            return null;
        }
    }
}