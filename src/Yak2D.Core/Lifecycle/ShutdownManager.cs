using System;
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
        private readonly IGraphics _graphics;

        public ShutdownManager(IFrameworkMessenger frameworkMessenger,
                                ISystemComponents veldridComponents,
                                IGraphicsShutdownManager graphicsShutdownManager,
                                IGpuSurfaceManager gpuSurfaceManager,
                                IFontManager fontManager,
                                ISdl2EventProcessor sdl2EventProcessor,
                                IGraphics graphics
                                )
        {
            _frameworkMessenger = frameworkMessenger;
            _veldridComponents = veldridComponents;
            _graphicsShutdownManager = graphicsShutdownManager;
            _gpuSurfaceManager = gpuSurfaceManager;
            _fontManager = fontManager;
            _sdl2EventProcessor = sdl2EventProcessor;
            _graphics = graphics;
        }

        public void Shutdown()
        {
            if (!_graphics.WaitForRenderingToComplete(TimeSpan.FromSeconds(1)))
            {
                _frameworkMessenger.Report("Render task did not complete within 1s; continuing shutdown");
            }

            _veldridComponents.ReleaseResources();

            _frameworkMessenger.Shutdown();
            _fontManager.Shutdown();
            _graphicsShutdownManager.Shutdown();
            _gpuSurfaceManager.Shutdown();
            _sdl2EventProcessor.Shutdown();
        }
    }
}