using System.Numerics;

namespace Yak2D.Graphics
{
    public class Stages : IStages
    {
        private const int MAX_BLUR_SAMPLE_STEPS_FROM_CENTRE = 7;

        private readonly IRenderStageManager _renderStageManager;
        private readonly IViewportManager _viewportManager;
        private readonly IRenderStageVisitor _renderStageVisitor;

        public int CountRenderStages { get { return _renderStageManager.StageCount; } }
        public int CountViewports { get { return _viewportManager.ViewportCount; } }

        public Stages(IRenderStageManager renderStageManager,
                      IViewportManager viewportManager,
                      IRenderStageVisitor renderStageVisitor)
        {
            _renderStageManager = renderStageManager;
            _viewportManager = viewportManager;
            _renderStageVisitor = renderStageVisitor;
        }

        public IDrawStage CreateDrawStage(bool clearDynamicRequestQueueEachFrame = true,
                                          BlendState blendState = BlendState.Alpha) => _renderStageManager.CreateDrawStage(clearDynamicRequestQueueEachFrame, blendState);

        public IColourEffectsStage CreateColourEffectsStage() => _renderStageManager.CreateColourEffectStage();

        public IBloomStage CreateBloomStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            return _renderStageManager.CreateBloomEffectStage(sampleSurfaceWidth, sampleSurfaceHeight);
        }

        public IBlurStage CreateBlurStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            return _renderStageManager.CreateBlurEffect2DStage(sampleSurfaceWidth, sampleSurfaceHeight);
        }

        public IBlur1DStage CreateBlur1DStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            return _renderStageManager.CreateBlurEffect1DStage(sampleSurfaceWidth, sampleSurfaceHeight);
        }

        public IStyleEffectsStage CreateStyleEffectsStage() => _renderStageManager.CreateStyleEffectStage();

        public IMeshRenderStage CreateMeshRenderStage() => _renderStageManager.CreateMeshRenderStage();

        public IDistortionStage CreateDistortionStage(uint internalSurfaceWidth, uint internalSurfaceHeight, bool clearDynamicRequestQueueEachFrame = true)
        {
            return _renderStageManager.CreateDistortionEffectStage(clearDynamicRequestQueueEachFrame, internalSurfaceWidth, internalSurfaceHeight);
        }

        public IMixStage CreateMixStage()
        {
            return _renderStageManager.CreateMixStage();
        }

        public ICustomShaderStage CreateCustomShaderStage(string fragmentShaderPathName, AssetSourceEnum assetType, ShaderUniformDescription[] uniformDescriptions, BlendState blendState)
        {
            return _renderStageManager.CreateCustomShaderStage(fragmentShaderPathName, assetType, uniformDescriptions, blendState);
        }

        public ICustomVeldridStage CreateCustomVeldridStage(CustomVeldridBase stage)
        {
            return _renderStageManager.CreateCustomVeldridStage(stage);
        }

        public void DestroyStage(IRenderStage renderStage)
        {
            if (renderStage == null)
            {
                throw new Yak2DException("Unable to destroy render stage as null passed");
            }

            _renderStageManager.DestroyStage(renderStage.Id);
        }

        public void DestroyStage(ulong renderStage)
        {
            _renderStageManager.DestroyStage(renderStage);
        }

        public void DestroyAllStages() => _renderStageManager.DestroyAllStages(false);

        public IViewport CreateViewport(uint minx, uint miny, uint width, uint height)
        {
            return _viewportManager.CreateViewport(minx, miny, width, height);
        }

        public void DestroyViewport(IViewport viewport)
        {
            if (viewport == null)
            {
                throw new Yak2DException("Unable to destroy viewport as null passed");
            }

            _viewportManager.DestroyViewport(viewport.Id);
        }

        public void DestroyViewport(ulong viewport)
        {
            _viewportManager.DestroyViewport(viewport);
        }

        public void DestroyAllViewports() => _viewportManager.DestroyAllViewports();

        private void CacheRenderStageModelInVisitor(ulong id)
        {
            var model = _renderStageManager.RetrieveStageModel(id);

            if (model == null)
            {
                throw new Yak2DException("Unable to set effects as requested stage does not exist (invalid stage id)");
            }

            model?.CacheInstanceInVisitor(_renderStageVisitor);
        }

        public void SetColourEffectsConfig(IColourEffectsStage effect, ColourEffectConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set colour effect as stage passed is null");
            }

            SetColourEffectsConfig(effect.Id, config, transitionSeconds);
        }

        public void SetColourEffectsConfig(ulong effect, ColourEffectConfiguration config, float transitionSeconds)
        {
            config.SingleColour = Utility.Clamper.Clamp(config.SingleColour, 0.0f, 1.0f);
            config.GrayScale = Utility.Clamper.Clamp(config.GrayScale, 0.0f, 1.0f);
            config.Colourise = Utility.Clamper.Clamp(config.Colourise, 0.0f, 1.0f);
            config.Negative = Utility.Clamper.Clamp(config.Negative, 0.0f, 1.0f);
            config.Opacity = Utility.Clamper.Clamp(config.Opacity, 0.0f, 1.0f);
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedColourEffectModel?.SetEffectTransition(ref config, ref transitionSeconds);
        }

        public void SetBloomConfig(IBloomStage effect, BloomEffectConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set bloom effect as stage passed is null");
            }

            SetBloomConfig(effect.Id, config, transitionSeconds);
        }

        public void SetBloomConfig(ulong effect, BloomEffectConfiguration config, float transitionSeconds)
        {
            config.BrightnessThreshold = Utility.Clamper.Clamp(config.BrightnessThreshold, 0.0f, 1.0f);
            config.AdditiveMixAmount = Utility.Clamper.Clamp(config.AdditiveMixAmount, 0.0f, float.MaxValue);

            config.NumberOfBlurSamples = Utility.Clamper.Clamp(config.NumberOfBlurSamples, 0, MAX_BLUR_SAMPLE_STEPS_FROM_CENTRE);

            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedBloomEffectStageModel?.SetEffectTransition(ref config, ref transitionSeconds);
        }

        public void SetBlurConfig(IBlurStage effect, BlurEffectConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set blur2d effect as stage passed is null");
            }

            SetBlurConfig(effect.Id, config, transitionSeconds);
        }

        public void SetBlurConfig(ulong effect, BlurEffectConfiguration config, float transitionSeconds)
        {
            config.MixAmount = Utility.Clamper.Clamp(config.MixAmount, 0.0f, float.MaxValue);

            config.NumberOfBlurSamples = Utility.Clamper.Clamp(config.NumberOfBlurSamples, 0, MAX_BLUR_SAMPLE_STEPS_FROM_CENTRE);

            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedBlur2dEffectModel?.SetEffectTransition(ref config, ref transitionSeconds);
        }

        public void SetBlur1DConfig(IBlur1DStage effect, Blur1DEffectConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set blur1d effect as stage passed is null");
            }

            SetBlur1DConfig(effect.Id, config, transitionSeconds);
        }

        public void SetBlur1DConfig(ulong effect, Blur1DEffectConfiguration config, float transitionSeconds)
        {
            config.MixAmount = Utility.Clamper.Clamp(config.MixAmount, 0.0f, float.MaxValue); //i think move these checks deeper and closer to where get used..

            config.NumberOfBlurSamples = Utility.Clamper.Clamp(config.NumberOfBlurSamples, 0, MAX_BLUR_SAMPLE_STEPS_FROM_CENTRE);

            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedBlur1dEffectModel?.SetEffectTransition(ref config, ref transitionSeconds);
        }

        public void SetStyleEffectsGroupConfig(IStyleEffectsStage effect, StyleEffectGroupConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set style effect as stage passed is null");
            }

            SetStyleEffectsGroupConfig(effect.Id, config, transitionSeconds);
        }

        public void SetStyleEffectsGroupConfig(ulong effect, StyleEffectGroupConfiguration config, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedStyleEffectStageModel?.SetEffectGroupTransition(ref config, transitionSeconds);
        }

        public void SetStyleEffectsPixellateConfig(IStyleEffectsStage effect, PixellateConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set style effect as stage passed is null");
            }

            SetStyleEffectsPixellateConfig(effect.Id, config, transitionSeconds);
        }

        public void SetStyleEffectsPixellateConfig(ulong effect, PixellateConfiguration config, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedStyleEffectStageModel?.SetPixellateTransition(ref config, transitionSeconds);
        }

        public void SetStyleEffectsEdgeDetectionConfig(IStyleEffectsStage effect, EdgeDetectionConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set style effect as stage passed is null");
            }

            SetStyleEffectsEdgeDetectionConfig(effect.Id, config, transitionSeconds);
        }

        public void SetStyleEffectsEdgeDetectionConfig(ulong effect, EdgeDetectionConfiguration config, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedStyleEffectStageModel?.SetEdgeDetectionTransition(ref config, transitionSeconds);
        }

        public void SetStyleEffectsStaticConfig(IStyleEffectsStage effect, StaticConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set colour style as stage passed is null");
            }

            SetStyleEffectsStaticConfig(effect.Id, config, transitionSeconds);
        }

        public void SetStyleEffectsStaticConfig(ulong effect, StaticConfiguration config, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedStyleEffectStageModel?.SetStaticTransition(ref config, transitionSeconds);
        }

        public void SetStyleEffectsOldMovieConfig(IStyleEffectsStage effect, OldMovieConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set colour style as stage passed is null");
            }

            SetStyleEffectsOldMovieConfig(effect.Id, config, transitionSeconds);
        }

        public void SetStyleEffectsOldMovieConfig(ulong effect, OldMovieConfiguration config, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedStyleEffectStageModel?.SetOldMovieTransition(ref config, transitionSeconds);
        }

        public void SetStyleEffectsCrtConfig(IStyleEffectsStage effect, CrtEffectConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set style effect as stage passed is null");
            }

            SetStyleEffectsCrtConfig(effect.Id, config, transitionSeconds);
        }

        public void SetStyleEffectsCrtConfig(ulong effect, CrtEffectConfiguration config, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedStyleEffectStageModel?.SetCrtEffectTransition(ref config, transitionSeconds);
        }

        public void SetMeshRenderLightingProperties(IMeshRenderStage effect, MeshRenderLightingPropertiesConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set mesh effect as stage passed is null");
            }

            SetMeshRenderLightingProperties(effect.Id, config, transitionSeconds);
        }

        public void SetMeshRenderLightingProperties(ulong effect, MeshRenderLightingPropertiesConfiguration config, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedMeshRenderStageModel?.SetLightingProperties(ref config, transitionSeconds);
        }

        public void SetMeshRenderLights(IMeshRenderStage effect, MeshRenderLightConfiguration[] lightConfigurations, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set mesh effect as stage passed is null");
            }

            SetMeshRenderLights(effect.Id, lightConfigurations, transitionSeconds);
        }

        public void SetMeshRenderLights(ulong effect, MeshRenderLightConfiguration[] lightConfigurations, float transitionSeconds)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedMeshRenderStageModel?.SetLights(lightConfigurations, transitionSeconds);
        }

        public void SetMeshRenderMesh(IMeshRenderStage effect, Vertex3D[] mesh)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set mesh effect as stage passed is null");
            }

            SetMeshRenderMesh(effect.Id, mesh);
        }

        public void SetMeshRenderMesh(ulong effect, Vertex3D[] mesh)
        {
            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedMeshRenderStageModel?.SetMesh(mesh);
        }

        public void SetDistortionConfig(IDistortionStage effect, DistortionEffectConfiguration config, float transitionSeconds)
        {
            if (effect == null)
            {
                throw new Yak2DException("Unable to set distortion effect as stage passed is null");
            }

            SetDistortionConfig(effect.Id, config, transitionSeconds);
        }

        public void SetDistortionConfig(ulong effect, DistortionEffectConfiguration config, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            CacheRenderStageModelInVisitor(effect);
            _renderStageVisitor.CachedDistortionEffectStageModel?.SetEffectTransition(ref config, ref transitionSeconds);
        }

        public void SetMixStageProperties(IMixStage stage, Vector4 amounts, float transitionSeconds, bool normalise)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to set mix effect as stage passed is null");
            }

            SetMixStageProperties(stage.Id, amounts, transitionSeconds, normalise);
        }

        public void SetMixStageProperties(ulong stage, Vector4 amounts, float transitionSeconds, bool normalise)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            CacheRenderStageModelInVisitor(stage);
            _renderStageVisitor.CachedMixStageModel?.SetEffectTransition(ref amounts, ref transitionSeconds, normalise);
        }


        public void SetCustomShaderUniformValues<T>(ICustomShaderStage stage, string uniformName, T data) where T : struct
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to set custom shader effect as stage passed is null");
            }

            SetCustomShaderUniformValues(stage.Id, uniformName, data);
        }

        public void SetCustomShaderUniformValues<T>(ulong stage, string uniformName, T data) where T : struct
        {
            CacheRenderStageModelInVisitor(stage);
            _renderStageVisitor.CachedCustomShaderModel?.SetUniformValue<T>(uniformName, data);
        }

        public void SetCustomShaderUniformValues<T>(ICustomShaderStage stage, string uniformName, T[] dataArray) where T : struct
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to set custom shader effect as stage passed is null");
            }
            SetCustomShaderUniformValues(stage.Id, uniformName, dataArray);
        }

        public void SetCustomShaderUniformValues<T>(ulong stage, string uniformName, T[] dataArray) where T : struct
        {
            CacheRenderStageModelInVisitor(stage);
            _renderStageVisitor.CachedCustomShaderModel?.SetUniformValue<T>(uniformName, dataArray);
        }
    }
}