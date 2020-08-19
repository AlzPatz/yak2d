using Veldrid;

namespace Yak2D.Graphics
{
    public class ViewportModel : IViewportModel
    {
        public Viewport Viewport { get; private set; }

        public ViewportModel(uint minx, uint miny, uint width, uint height)
        {
            Viewport = new Viewport(minx, miny, width, height, 0.0f, 1.0f);
        }
    }
}