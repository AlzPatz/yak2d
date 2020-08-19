using Veldrid;

namespace Yak2D.Graphics
{
    public struct RenderCommandQueueItem
    {
        public RenderCommandType Type;
        public ulong Stage;
        public ulong Surface;
        public ulong Camera;
        public ulong Texture0;
        public ulong Texture1;
        public ulong SpareId0;
        public ulong SpareId1;
        public RgbaFloat Colour;
    }
}
