using Veldrid;

namespace Yak2D.Graphics
{
    public interface IFullNdcSpaceQuadVertexBuffer
    {
        DeviceBuffer Buffer { get; }

        void ReInitialise();
    }
}