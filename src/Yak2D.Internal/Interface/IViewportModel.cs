using Veldrid;

namespace Yak2D.Internal
{
    public interface IViewportModel
    {
        Viewport Viewport { get; }
        uint MinX { get; }
        uint MinY { get; }
        uint Width { get; }
        uint Height { get; }
    }
}