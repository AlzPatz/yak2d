using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Yak2D.Internal
{
    public abstract class AssemblyBase : IAssembly
    {
        public string Name { get; private set; }

        private Assembly _assembly;
        private List<string> _resourceNames;

        protected abstract Assembly ProvideAssembly();

        public AssemblyBase()
        {
            _assembly = ProvideAssembly(); //Not great practice, but given specific use case can be confident it works

            if (_assembly == null)
            {
                throw new Yak2DException("Internal Fatal Error: null assembly passed to AssemblyBase");
            }

            Name = _assembly.GetName().Name;

            _resourceNames = _assembly.GetManifestResourceNames().ToList();
        }

        public List<string> GetManifestResourceNames()
        {
            return _resourceNames;
        }

        public Stream GetManifestResourceStream(string resourceName)
        {
            if (_resourceNames.Contains(resourceName))
            {
                return _assembly.GetManifestResourceStream(resourceName);
            }

            return null;
        }
    }
}