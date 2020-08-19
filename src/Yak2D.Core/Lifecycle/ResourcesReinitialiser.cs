using Yak2D.Internal;

namespace Yak2D.Core
{
    public class ResourcesReinitialiser : IResourceReinitialiser
    {
        private readonly IGraphicsResourceReinitialiser _graphicsResourceReinitialiser;
        private readonly IFontManager _fontManager;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly IFrameworkDebugOverlay _frameworkDebugOverlay;

        public ResourcesReinitialiser(
            IGraphicsResourceReinitialiser graphicsResourceReinitialiser,
            IFontManager fontManager,
            IGpuSurfaceManager gpuSurfaceManager,
            IFrameworkDebugOverlay frameworkDebugOverlay
        )
        {
            _graphicsResourceReinitialiser = graphicsResourceReinitialiser;
            _fontManager = fontManager;
            _gpuSurfaceManager = gpuSurfaceManager;
            _frameworkDebugOverlay = frameworkDebugOverlay;
        }

        public void ReInitialise()
        {
            _gpuSurfaceManager.ReInitialise();
            _graphicsResourceReinitialiser.ReInitialise();
            _fontManager.ReInitialise();
            _frameworkDebugOverlay.ReInitialise();
        }
    }
}