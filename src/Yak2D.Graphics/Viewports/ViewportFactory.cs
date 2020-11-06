using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class ViewportFactory : IViewportFactory
    {
        public IViewportModel CreateViewport(uint minx, uint miny, uint width, uint height)
        {
            return new ViewportModel(minx, miny, width, height);
        }
    }
}