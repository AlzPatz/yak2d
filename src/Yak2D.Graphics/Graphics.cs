using System;
using System.Threading.Tasks;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class Graphics : IGraphics
    {
        public bool RenderingComplete { get; private set; }

        private readonly ISystemComponents _systemComponents;
        private readonly IStartupPropertiesCache _startUpPropertiesCache;
        private readonly IRenderCommandQueue _renderCommandQueue;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IRenderStageManager _renderStageManager;
        private readonly IRenderStageVisitor _renderStageVisitor;
        private readonly IGpuSurfaceManager _surfaceManager;
        private readonly IViewportManager _viewportManager;
        private readonly IFontManager _fontManager;
        private readonly ICameraManager _cameraManager;
        private readonly IFrameworkDebugOverlay _debugOverlay;

        private CommandList _cl;

        public Graphics(IStartupPropertiesCache startUpPropertiesCache,
                            ISystemComponents systemComponents,
                            IRenderCommandQueue renderCommandQueue,
                            ICommandProcessor commandProcessor,
                            IRenderStageManager renderStageManager,
                            IRenderStageVisitor renderStageVisitor,
                            IGpuSurfaceManager surfaceManager,
                            IViewportManager viewportManager,
                            IFontManager fontManager,
                            ICameraManager cameraManager,
                            IFrameworkDebugOverlay debugOverlay)
        {
            _startUpPropertiesCache = startUpPropertiesCache;
            _systemComponents = systemComponents;
            _renderCommandQueue = renderCommandQueue;
            _commandProcessor = commandProcessor;
            _renderStageManager = renderStageManager;
            _renderStageVisitor = renderStageVisitor;
            _surfaceManager = surfaceManager;
            _viewportManager = viewportManager;
            _fontManager = fontManager;
            _cameraManager = cameraManager;
            _debugOverlay = debugOverlay;

            Initialise();
        }

        private void Initialise()
        {
            _cl = _systemComponents.Factory.CreateCommandList();

            RenderingComplete = true;
        }

        public void PrepareForDrawing()
        {
            RenderingComplete = false;
            _renderCommandQueue.Reset();
            _renderStageManager.PrepareForDrawing();
            QueueAutoClearSurfaces();
        }

        private void QueueAutoClearSurfaces()
        {
            //Auto clear colour and depth for required render surfaces

            if(_surfaceManager.AutoClearMainWindowDepth)
            {
                   _renderCommandQueue.Add(
                            RenderCommandType.ClearDepthTarget,
                            0UL,
                            _surfaceManager.MainSwapChainFrameBufferKey,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear); //Note - this does not represent clear colour
            }

            if(_surfaceManager.AutoClearMainWindowColour)
            {
                 _renderCommandQueue.Add(
                            RenderCommandType.ClearColourTarget,
                            0UL,
                            _surfaceManager.MainSwapChainFrameBufferKey,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear);
            }

            var autoClearDepthIds = _surfaceManager.GetAutoClearDepthSurfaceIds();

            autoClearDepthIds?.ForEach(id =>
            {
                _renderCommandQueue.Add(
                            RenderCommandType.ClearDepthTarget,
                            0UL,
                            id,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear); //Note - this does not represent clear colour
            });

            var autoClearColourIds = _surfaceManager.GetAutoClearColourSurfaceIds();

            autoClearColourIds?.ForEach(id =>
            {
                _renderCommandQueue.Add(
                            RenderCommandType.ClearColourTarget,
                            0UL,
                            id,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear);
            });
        }

        public void Render(float timeSinceLastDraw)
        {
            ProcessRenderStageUpdates(timeSinceLastDraw);

            _cl.Begin();

            if (_renderCommandQueue.CommandQueueSize == 0)
            {
                ClearWindowWhenNoRenderCommandsQueued(_startUpPropertiesCache.Internal.DefaultWindowClearColourOnNoRenderCommandsQueued);
            }
            else
            {
                ProcessRenderCommandQueue();
            }

            if (_debugOverlay.Visible)
            {
                _debugOverlay.Render(_cl, _systemComponents);
            }

            _cl.End();

            _systemComponents.Device.SubmitCommands(_cl);

            _renderStageVisitor.ClearCachedDrawingModels();

            Task.Run(() =>
            {
                _systemComponents.Device.WaitForIdle();
                _systemComponents.Device.SwapBuffers();
                ExecutePostRenderFunctions();
                RenderingComplete = true;
            });
        }

        private void ExecutePostRenderFunctions()
        {
            //Back on the main thread we can now safely pass data back to the user
            InvokePostRenderingSurfaceCopyCallbacks();

            //Process queued framework item destruction
            _surfaceManager.ProcessPendingDestruction();
            _renderStageManager.ProcessPendingDestruction();
            _viewportManager.ProcessPendingDestruction();
            _fontManager.ProcessPendingDestruction();
            _cameraManager.ProcessPendingDestruction();
        }

        private void InvokePostRenderingSurfaceCopyCallbacks()
        {
            foreach (var id in _renderCommandQueue.FlushCallbackStageIds())
            {
                var stage = _renderStageManager.RetrieveStageModel(id) as ISurfaceCopyStageModel;

                if (stage != null)
                {
                    stage.CopyDataFromStagingTextureAndPassToUser();
                }
            }
        }

        private void ProcessRenderStageUpdates(float timeSinceLastDraw)
        {
            _renderStageManager.ProcessRenderStageUpdates(timeSinceLastDraw);
        }

        private void ClearWindowWhenNoRenderCommandsQueued(RgbaFloat colour)
        {
            _cl.SetFramebuffer(_systemComponents.Device.SwapchainFramebuffer);

            _cl.ClearColorTarget(0, colour);
        }

        private void ProcessRenderCommandQueue()
        {
            foreach (var command in _renderCommandQueue.FlushCommands())
            {
                _commandProcessor.Process(_cl, command);
            }
        }

        public void ReInitalise()
        {
            Initialise();
        }
    }
}