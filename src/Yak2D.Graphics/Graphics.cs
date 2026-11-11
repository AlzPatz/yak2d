using System;
using System.Threading.Tasks;
using NeoVeldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class Graphics : IGraphics
    {
        public bool RenderingComplete { get; private set; }

        private readonly IFrameworkMessenger _frameworkMessenger;
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
        private Task _renderTask;

        public Graphics(IFrameworkMessenger frameworkMessenger,
                            IStartupPropertiesCache startUpPropertiesCache,
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
            _frameworkMessenger = frameworkMessenger;
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

            //On (re)initialisation the device has been replaced, so any present still in flight belongs to the old device.
            //It is abandoned here so it cannot keep the draw gate shut: its outcome is reported from inside the task, and
            //any destruction it left queued is picked up by the next frame's completion (TryCompleteFrame)
            _renderTask = null;
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

            StartPresent();
        }

        internal void StartPresent()
        {
            //Only the GPU wait and the swap happen off the main thread. Post-render work is done by TryCompleteFrame
            _renderTask = Task.Run(() =>
            {
                try
                {
                    _systemComponents.Device.WaitForIdle();
                    if (_systemComponents.Window.Exists)
                    {
                        _systemComponents.Device.SwapBuffers();
                    }
                }
                catch (Exception e)
                {
                    //Observed here so the task never faults and the frame still completes on the main thread
                    _frameworkMessenger.Report(string.Concat("Present failed (frame will still complete): ", e));
                }
            });
        }

        public bool TryCompleteFrame()
        {
            if (RenderingComplete)
            {
                return true;
            }

            var task = _renderTask;
            if (task != null && !task.IsCompleted)
            {
                return false;
            }

            //A null task while not complete means the frame was abandoned (or never presented), so it is treated as complete
            try
            {
                ExecutePostRenderFunctions();
            }
            catch (Exception e)
            {
                _frameworkMessenger.Report(string.Concat("Post-render work failed (frame marked complete): ", e));
            }
            finally
            {
                _renderTask = null;
                RenderingComplete = true;
            }

            return true;
        }

        public bool WaitForRenderingToComplete(TimeSpan timeout)
        {
            var task = _renderTask;
            if (task == null)
            {
                return true;
            }

            try
            {
                return task.Wait(timeout);
            }
            catch (AggregateException)
            {
                return true; //Task finished (faulted); nothing left to wait for
            }
        }

        private void ExecutePostRenderFunctions()
        {
            //Runs on the main thread from TryCompleteFrame, after the present task (WaitForIdle + SwapBuffers) has ended,
            //so this frame's GPU work is idle and the managers' pending lists are only ever touched by the main thread
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