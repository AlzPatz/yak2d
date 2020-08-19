using Veldrid;

namespace Yak2D.Graphics
{
    public interface IRenderStageVisitor
    {
        IDrawStageModel CachedDrawStageModel { get; }
        IBloomStageModel CachedBloomEffectStageModel { get; }
        IBlur1DStageModel CachedBlur1dEffectModel { get; }
        IBlurStageModel CachedBlur2dEffectModel { get; }
        IColourEffectsStageModel CachedColourEffectModel { get; }
        IDistortionStageModel CachedDistortionEffectStageModel { get; }
        IMeshRenderStageModel CachedMeshRenderStageModel { get; }
        IMixStageModel CachedMixStageModel { get; }
        IStyleEffectsStageModel CachedStyleEffectStageModel { get; }
        ICustomShaderStageModel CachedCustomShaderModel { get; }
        ICustomVeldridStageModel CachedCustomVeldridModel { get; }

        void DispatchToRenderStage(IDrawStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IBloomStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IBlur1DStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IBlurStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IColourEffectsStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IDistortionStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IMeshRenderStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IMixStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(IStyleEffectsStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(ICustomShaderStageModel model, CommandList cl, RenderCommandQueueItem command);
        void DispatchToRenderStage(ICustomVeldridStageModel model, CommandList cl, RenderCommandQueueItem command);

        void CacheStageModel(IDrawStageModel model);
        void CacheStageModel(IBloomStageModel model);
        void CacheStageModel(IBlur1DStageModel model);
        void CacheStageModel(IBlurStageModel model);
        void CacheStageModel(IColourEffectsStageModel model);
        void CacheStageModel(IDistortionStageModel model);
        void CacheStageModel(IMeshRenderStageModel model);
        void CacheStageModel(IMixStageModel model);
        void CacheStageModel(IStyleEffectsStageModel model);
        void ClearCachedDrawingModels();
        void CacheStageModel(ICustomShaderStageModel model);
        void CacheStageModel(ICustomVeldridStageModel model);
    }
}