using Veldrid;

namespace Yak2D.Graphics
{
    public interface IViewportManager
    {
        int ViewportCount { get; }
        IViewport CreateViewport(uint minx, uint miny, uint width, uint height);
        void ClearActiveViewport();
        void SetActiveViewport(ulong id);
        void ConfigureViewportForActiveFramebuffer(CommandList cl);
        void DestroyViewport(ulong viewport);
        void DestroyAllViewports();
        void Shutdown();
        void ReInitialise();
    }
}