using System.Reflection;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class GraphicsAssembly : AssemblyBase, IGraphicsAssembly
    {
        protected override Assembly ProvideAssembly()
        {
            return this.GetType().Assembly;
        }
    }
}
