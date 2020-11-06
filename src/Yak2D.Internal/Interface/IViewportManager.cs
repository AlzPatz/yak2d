using Veldrid;

namespace Yak2D.Internal
{
    public interface IViewportManager
    {
        int ViewportCount { get; }
        IViewport CreateViewport(uint minx, uint miny, uint width, uint height);
        IViewportModel RetrieveViewportModel(ulong key);
        void ClearActiveViewport();
        void SetActiveViewport(ulong id);
        void ConfigureViewportForActiveFramebuffer(CommandList cl);
        void DestroyViewport(ulong viewport);
        void DestroyAllViewports();
        void Shutdown();
        void ReInitialise();
    }
}