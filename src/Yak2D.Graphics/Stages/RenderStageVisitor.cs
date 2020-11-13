using System;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class RenderStageVisitor : IRenderStageVisitor
    {
        public IDrawStageModel CachedDrawStageModel { get; private set; }
        public IBloomStageModel CachedBloomEffectStageModel { get; private set; }
        public IBlur1DStageModel CachedBlur1dEffectModel { get; private set; }
        public IBlurStageModel CachedBlur2dEffectModel { get; private set; }
        public IColourEffectsStageModel CachedColourEffectModel { get; private set; }
        public IDistortionStageModel CachedDistortionEffectStageModel { get; private set; }
        public IMeshRenderStageModel CachedMeshRenderStageModel { get; private set; }
        public IMixStageModel CachedMixStageModel { get; private set; }
        public IStyleEffectsStageModel CachedStyleEffectStageModel { get; private set; }
        public ICustomShaderStageModel CachedCustomShaderModel { get; private set; }
        public ICustomVeldridStageModel CachedCustomVeldridModel { get; private set; }
        public ISurfaceCopyStageModel CachedSurfaceCopyStageModel { get; private set; }

        public void CacheStageModel(IDrawStageModel model) => CachedDrawStageModel = model;
        public void CacheStageModel(IBloomStageModel model) => CachedBloomEffectStageModel = model;
        public void CacheStageModel(IBlur1DStageModel model) => CachedBlur1dEffectModel = model;
        public void CacheStageModel(IBlurStageModel model) => CachedBlur2dEffectModel = model;
        public void CacheStageModel(IColourEffectsStageModel model) => CachedColourEffectModel = model;
        public void CacheStageModel(IDistortionStageModel model) => CachedDistortionEffectStageModel = model;
        public void CacheStageModel(IMeshRenderStageModel model) => CachedMeshRenderStageModel = model;
        public void CacheStageModel(IMixStageModel model) => CachedMixStageModel = model;
        public void CacheStageModel(IStyleEffectsStageModel model) => CachedStyleEffectStageModel = model;
        public void CacheStageModel(ICustomShaderStageModel model) => CachedCustomShaderModel = model;
        public void CacheStageModel(ICustomVeldridStageModel model) => CachedCustomVeldridModel = model;
        public void CacheStageModel(ISurfaceCopyStageModel model) => CachedSurfaceCopyStageModel = model;

        private readonly IGpuSurfaceManager _surfaceManager;
        private readonly ICameraManager _cameraManager;
        private readonly IDrawStageRenderer _drawStageRenderer;
        private readonly IColourEffectsStageRenderer _colourEffectStageRenderer;
        private readonly IBloomStageRenderer _bloomEffectStageRenderer;
        private readonly IBlur1DStageRenderer _blur1DEffectStageRenderer;
        private readonly IBlurStageRenderer _blur2DEffectStageRenderer;
        private readonly IStyleEffectsStageRenderer _styleEffectRenderer;
        private readonly IMeshRenderStageRenderer _meshRenderer;
        private readonly IDistortionStageRenderer _distortionEffectStageRenderer;
        private readonly IMixStageRenderer _mixRenderer;
        private readonly ICustomShaderStageRenderer _customShaderRenderer;
        private readonly ICustomVeldridStageRenderer _customVeldridRenderer;
        private readonly ISurfaceCopyStageRenderer _surfaceCopyStageRenderer;

        public RenderStageVisitor(
                                                        IGpuSurfaceManager surfaceManager,
                                                        ICameraManager cameraManager,
                                                        IDrawStageRenderer drawStageRenderer,
                                                        IColourEffectsStageRenderer colourEffectStageRenderer,
                                                        IBloomStageRenderer bloomEffectStageRenderer,
                                                        IBlurStageRenderer blur2DEffectStageRenderer,
                                                        IBlur1DStageRenderer blur1DEffectStageRenderer,
                                                        IStyleEffectsStageRenderer styleEffectRenderer,
                                                        IMeshRenderStageRenderer meshRenderer,
                                                        IDistortionStageRenderer distortionEffectStageRenderer,
                                                        IMixStageRenderer mixRenderer,
                                                        ICustomShaderStageRenderer customShaderRenderer,
                                                        ICustomVeldridStageRenderer customVeldridRenderer,
                                                        ISurfaceCopyStageRenderer surfaceCopyStageRenderer)
        {
            _surfaceManager = surfaceManager;
            _cameraManager = cameraManager;
            _drawStageRenderer = drawStageRenderer;
            _colourEffectStageRenderer = colourEffectStageRenderer;
            _bloomEffectStageRenderer = bloomEffectStageRenderer;
            _blur1DEffectStageRenderer = blur1DEffectStageRenderer;
            _blur2DEffectStageRenderer = blur2DEffectStageRenderer;
            _styleEffectRenderer = styleEffectRenderer;
            _meshRenderer = meshRenderer;
            _distortionEffectStageRenderer = distortionEffectStageRenderer;
            _mixRenderer = mixRenderer;
            _customShaderRenderer = customShaderRenderer;
            _customVeldridRenderer = customVeldridRenderer;
            _surfaceCopyStageRenderer = surfaceCopyStageRenderer;
        }

        public void DispatchToRenderStage(IDrawStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            stage.Process();
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var camera = _cameraManager.RetrieveCameraModel2D(command.Camera);
            _drawStageRenderer.Render(cl, stage, surface, camera);
        }

        public void DispatchToRenderStage(IDistortionStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            stage.Process();
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var camera = _cameraManager.RetrieveCameraModel2D(command.Camera);
            _distortionEffectStageRenderer.Render(cl, stage, source, surface, camera);
        }

        public void DispatchToRenderStage(IBloomStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            _bloomEffectStageRenderer.Render(cl, stage, source, surface);
        }

        public void DispatchToRenderStage(IBlur1DStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal }); ;
            _blur1DEffectStageRenderer.Render(cl, stage, source, surface);
        }

        public void DispatchToRenderStage(IBlurStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            _blur2DEffectStageRenderer.Render(cl, stage, source, surface);
        }

        public void DispatchToRenderStage(IColourEffectsStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            _colourEffectStageRenderer.Render(cl, stage, surface, source);
        }

        public void DispatchToRenderStage(IMeshRenderStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var camera = _cameraManager.RetrieveCameraModel3D(command.Camera);
            _meshRenderer.Render(cl, stage, source, surface, camera);
        }

        public void DispatchToRenderStage(IMixStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var t0 = command.Camera == 0UL ? null : _surfaceManager.RetrieveSurface(command.Camera, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t1 = command.Texture0 == 0UL ? null : _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t2 = command.Texture1 == 0UL ? null : _surfaceManager.RetrieveSurface(command.Texture1, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t3 = command.SpareId0 == 0UL ? null : _surfaceManager.RetrieveSurface(command.SpareId0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var mix = command.SpareId1 == 0UL ? null : _surfaceManager.RetrieveSurface(command.SpareId1, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            _mixRenderer.Render(cl, stage, mix, t0, t1, t2, t3, surface);
        }

        public void DispatchToRenderStage(IStyleEffectsStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var source = _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            _styleEffectRenderer.Render(cl, stage, source, surface);
        }

        public void DispatchToRenderStage(ICustomShaderStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var t0 = command.Camera == 0UL ? null : _surfaceManager.RetrieveSurface(command.Camera, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t1 = command.Texture0 == 0UL ? null : _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t2 = command.Texture1 == 0UL ? null : _surfaceManager.RetrieveSurface(command.Texture1, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t3 = command.SpareId0 == 0UL ? null : _surfaceManager.RetrieveSurface(command.SpareId0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            _customShaderRenderer.Render(cl, stage, t0, t1, t2, t3, surface);
        }

        public void DispatchToRenderStage(ICustomVeldridStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var surface = command.Surface == 0UL ? null : _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            var t0 = command.Camera == 0UL ? null : _surfaceManager.RetrieveSurface(command.Camera, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t1 = command.Texture0 == 0UL ? null : _surfaceManager.RetrieveSurface(command.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t2 = command.Texture1 == 0UL ? null : _surfaceManager.RetrieveSurface(command.Texture1, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            var t3 = command.SpareId0 == 0UL ? null : _surfaceManager.RetrieveSurface(command.SpareId0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });
            _customVeldridRenderer.Render(cl, stage, t0, t1, t2, t3, surface);
        }

        public void DispatchToRenderStage(ISurfaceCopyStageModel stage, CommandList cl, RenderCommandQueueItem command)
        {
            var source = command.Surface == 0UL ? null : _surfaceManager.RetrieveSurface(command.Surface, new GpuSurfaceType[] { GpuSurfaceType.Texture, GpuSurfaceType.Internal });
            _surfaceCopyStageRenderer.Render(cl, stage, source);
        }

        public void ClearCachedDrawingModels()
        {
            //Very defensive. This is called after each command list is submitted
            //As Drawing uses the cache, the clear is to avoid the event of a null
            //or invalid cached object is used at the start of a new drawing frame
            //And to avoid invalid references hanging around to destroyed draw stage modles
            //Perhaps the other cached objects need to be analysed for this persisting reference issue

            CachedDrawStageModel = null;
            CachedDistortionEffectStageModel = null;
        }
    }
}