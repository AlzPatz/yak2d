using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class ViewportModel : IViewportModel
    {
        public Viewport Viewport { get; private set; }

        public uint MinX { get; private set; }
        public uint MinY { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        public ViewportModel(uint minx, uint miny, uint width, uint height)
        {
            MinX = minx;
            MinY = miny;
            Width = width;
            Height = height;

            Viewport = new Viewport(minx, miny, width, height, 0.0f, 1.0f);
        }
    }
}