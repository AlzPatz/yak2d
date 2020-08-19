using Yak2D.Internal;

namespace Yak2D.Core
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _veldridComponents;
        private readonly IGraphicsShutdownManager _graphicsShutdownManager;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly IFontManager _fontManager;
        private readonly ISdl2EventProcessor _sdl2EventProcessor;

        public ShutdownManager(IFrameworkMessenger frameworkMessenger,
                                ISystemComponents veldridComponents,
                                IGraphicsShutdownManager graphicsShutdownManager,
                                IGpuSurfaceManager gpuSurfaceManager,
                                IFontManager fontManager,
                                ISdl2EventProcessor sdl2EventProcessor
                                )
        {
            _frameworkMessenger = frameworkMessenger;
            _veldridComponents = veldridComponents;
            _graphicsShutdownManager = graphicsShutdownManager;
            _gpuSurfaceManager = gpuSurfaceManager;
            _fontManager = fontManager;
            _sdl2EventProcessor = sdl2EventProcessor;
        }

        public void Shutdown()
        {
            _veldridComponents.ReleaseResources();
            
            _frameworkMessenger.Shutdown();
            _fontManager.Shutdown();
            _graphicsShutdownManager.Shutdown();
            _gpuSurfaceManager.Shutdown();
            _sdl2EventProcessor.Shutdown();
        }
    }
}