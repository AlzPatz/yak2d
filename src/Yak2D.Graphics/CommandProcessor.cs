using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IRenderStageManager _renderStageManager;
        private readonly IGpuSurfaceManager _surfaceManager;
        private readonly IViewportManager _viewportManager;
        private readonly IRenderStageVisitor _renderStageVisitor;
        private readonly ICopyStageRenderer _copyStageRenderer;
        private readonly ICameraManager _cameraManager;

        public CommandProcessor(IFrameworkMessenger frameworkMessenger,
                                                        IRenderStageManager renderStageManager,
                                                        IGpuSurfaceManager surfaceManager,
                                                        IViewportManager viewportManager,
                                                        IRenderStageVisitor renderStageVisitor,
                                                        ICopyStageRenderer copyStageRenderer,
                                                        ICameraManager cameraManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _renderStageManager = renderStageManager;
            _surfaceManager = surfaceManager;
            _viewportManager = viewportManager;
            _renderStageVisitor = renderStageVisitor;
            _copyStageRenderer = copyStageRenderer;
            _cameraManager = cameraManager;
        }

        public void Process(CommandList cl, RenderCommandQueueItem command)
        {
            switch (command.Type)
            {
                case RenderCommandType.ClearColourTarget:
                    ClearColourTarget(cl, command);
                    break;
                case RenderCommandType.ClearDepthTarget:
                    ClearDepthTarget(cl, command);
                    break;
                case RenderCommandType.SetViewport:
                    ProcessSetViewport(cl, command);
                    break;
                case RenderCommandType.ClearViewport:
                    ProcessClearViewport(cl, command);
                    break;
                case RenderCommandType.DrawStage:
                case RenderCommandType.ColourEffectStage:
                case RenderCommandType.BloomEffectStage:
                case RenderCommandType.Blur1DEffectStage:
                case RenderCommandType.Blur2DEffectStage:
                case RenderCommandType.StyleEffect:
                case RenderCommandType.MeshRender:
                case RenderCommandType.DistortionStage:
                case RenderCommandType.MixStage:
                case RenderCommandType.CustomShader:
                case RenderCommandType.CustomVeldrid:
                    ProcessRenderStage(cl, command);
                    break;
                case RenderCommandType.CopyStage:
                    ProcessCopyStage(cl, command);
                    break;
            }
        }

        private void ClearColourTarget(CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.RenderTarget | GpuSurfaceType.Internal });
            //Textures (not rendertargets) are fixed content
            //Cannot use internal render targets as reserved for framework use

            if (surface == null)
            {
                return;
            }

            cl.SetFramebuffer(surface.Framebuffer);
            cl.ClearColorTarget(0, command.Colour);
        }

        private void ClearDepthTarget(CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.RenderTarget | GpuSurfaceType.Internal });
            //Textures (not rendertargets) are fixed content and do not have depth
            //Cannot use internal render targets as reserved for framework use

            if (surface == null)
            {
                return;
            }

            cl.SetFramebuffer(surface.Framebuffer);
            cl.ClearDepthStencil(1.0f);
        }

        private void ProcessSetViewport(CommandList cl, RenderCommandQueueItem command)
        {
            _viewportManager.SetActiveViewport(command.SpareId0);
        }

        private void ProcessClearViewport(CommandList cl, RenderCommandQueueItem command)
        {
            _viewportManager.ClearActiveViewport();
        }

        private void ProcessCopyStage(CommandList cl, RenderCommandQueueItem command)
        {
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.Internal });
            var target = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            _copyStageRenderer.Render(cl, source, target);
        }

        private void ProcessRenderStage(CommandList cl, RenderCommandQueueItem command)
        {
            var renderStageModel = _renderStageManager.RetrieveStageModel(command.Stage);

            if (renderStageModel == null)
            {
                _frameworkMessenger.Report("Unable to process render stage command: error trying to locate model (null or wrong type");
                return;
            }

            renderStageModel.SendToRenderStage(_renderStageVisitor, cl, command);
        }
    }
}