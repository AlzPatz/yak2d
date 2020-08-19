namespace Yak2D.Graphics
{
    public interface IRenderStageManager
    {
        int StageCount { get; }
        void ProcessRenderStageUpdates(float timeSinceLastDraw);

        IDrawStage CreateDrawStage(bool clearDynamicRequestQueueEachFrame, BlendState blendState);
        IColourEffectsStage CreateColourEffectStage();
        IBloomStage CreateBloomEffectStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight);
        IBlur1DStage CreateBlurEffect1DStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight);
        IBlurStage CreateBlurEffect2DStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight);
        IStyleEffectsStage CreateStyleEffectStage();
        IMeshRenderStage CreateMeshRenderStage();
        IDistortionStage CreateDistortionEffectStage(bool clearDynamicRequestQueueEachFrame, uint internalSurfaceWidth, uint internalSurfaceHeight);
        IMixStage CreateMixStage();
        ICustomShaderStage CreateCustomShaderStage(string fragmentShaderFilename, AssetSourceEnum assetType, ShaderUniformDescription[] uniformDescriptions, BlendState blendState);
        ICustomVeldridStage CreateCustomVeldridStage(CustomVeldridBase stage);

        IRenderStageModel RetrieveStageModel(ulong id);

        void PrepareForDrawing();

        bool DestroyStage(ulong stage);
        void DestroyAllStages(bool haveResourcesAlreadyBeenDisposed);
        void Shutdown();
        void ReInitialise();
    }
}
