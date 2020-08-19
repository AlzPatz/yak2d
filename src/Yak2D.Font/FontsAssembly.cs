using System.Reflection;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class FontsAssembly : AssemblyBase, IFontsAssembly
    {
        protected override Assembly ProvideAssembly()
        {
            return this.GetType().Assembly;
        }
    }
}
