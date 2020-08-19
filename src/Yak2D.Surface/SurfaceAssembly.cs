using System.Reflection;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public class SurfaceAssembly : AssemblyBase, ISurfaceAssembly
    {
        protected override Assembly ProvideAssembly()
        {
            return this.GetType().Assembly;
        }
    }
}