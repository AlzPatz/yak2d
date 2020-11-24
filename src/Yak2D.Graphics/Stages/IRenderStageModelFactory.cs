using System;

namespace Yak2D.Graphics
{
    public interface IRenderStageModelFactory
    {
        IDrawStageModel CreateDrawStageModel(BlendState blendState);
        IColourEffectsStageModel CreateColourEffectStageModel();
        IBloomStageModel CreateBloomEffectStageModel(uint sampleSurfaceWidth, uint sampleSurfaceHeight);
        IBlurStageModel CreateBlur2DEffectModel(uint sampleSurfaceWidth, uint sampleSurfaceHeight);
        IBlur1DStageModel CreateBlur1DEffectModel(uint sampleSurfaceWidth, uint sampleSurfaceHeight);
        IStyleEffectsStageModel CreateStyleEffectModel();
        IMeshRenderStageModel CreateMeshRenderModel();
        IDistortionStageModel CreateDistortionEffectStageModel(uint internalSurfaceWidth, uint internalSurfaceHeight);
        IMixStageModel CreateMixStageModel();
        ICustomShaderStageModel CreateCustomStageModel(string fragmentShaderFilename, AssetSourceEnum assetType, ShaderUniformDescription[] uniformDescriptions, BlendState blendState, bool useSpirvCompile);
        ICustomVeldridStageModel CreateCustomVeldridStage(CustomVeldridBase stage);
        ISurfaceCopyStageModel CreateSurfaceCopyDataStage(uint stagingTextureWidth, uint stagingTextureHeight, Action<TextureData> callback, bool useFloat32PixelFormat);
    }
}