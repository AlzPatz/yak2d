using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class RenderStageModelFactory : IRenderStageModelFactory
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IStartupPropertiesCache _startUpPropertiesCache;
        private readonly ISystemComponents _systemComponents;
        private readonly IVeldridWindowUpdater _windowUpdater;
        private readonly IDrawQueueGroupFactory _drawQueueGroupFactory;
        private readonly IDrawStageBatcherFactory _drawStageBatcherFactory;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IGaussianBlurWeightsAndOffsetsCache _gaussianWeightsAndOffsetsCache;
        private readonly IQuadMeshBuilder _quadMeshBuilder;
        private readonly IBlendStateConverter _blendStateConverter;
        private readonly IShaderLoader _shaderTools;

        public RenderStageModelFactory(IFrameworkMessenger frameworkMessenger,
                                        IStartupPropertiesCache startUpPropertiesCache,
                                        ISystemComponents veldridComponents,
                                        IVeldridWindowUpdater windowUpdater,
                                        IDrawQueueGroupFactory drawQueueGroupFactory,
                                        IDrawStageBatcherFactory drawStageBatcherFactory,
                                        IGpuSurfaceManager gpuSurfaceManager,
                                        IPipelineFactory pipelineFactory,
                                        IGaussianBlurWeightsAndOffsetsCache gaussianWeightsAndOffsetsCache,
                                        IQuadMeshBuilder quadMeshBuilder,
                                        IBlendStateConverter blendStateConverter,
                                        IShaderLoader shaderTools)
        {
            _frameworkMessenger = frameworkMessenger;
            _startUpPropertiesCache = startUpPropertiesCache;
            _systemComponents = veldridComponents;
            _windowUpdater = windowUpdater;
            _drawQueueGroupFactory = drawQueueGroupFactory;
            _drawStageBatcherFactory = drawStageBatcherFactory;
            _gpuSurfaceManager = gpuSurfaceManager;
            _pipelineFactory = pipelineFactory;
            _gaussianWeightsAndOffsetsCache = gaussianWeightsAndOffsetsCache;
            _quadMeshBuilder = quadMeshBuilder;
            _blendStateConverter = blendStateConverter;
            _shaderTools = shaderTools;
        }

        public IDrawStageModel CreateDrawStageModel(BlendState blendState)
        {
            return new DrawStageModel(_drawStageBatcherFactory.Create(),
                                      _drawQueueGroupFactory.Create(false),
                                      blendState);
        }

        public IColourEffectsStageModel CreateColourEffectStageModel()
        {
            return new ColourEffectsStageModel(_frameworkMessenger, _systemComponents);
        }

        public IBloomStageModel CreateBloomEffectStageModel(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            return new BloomStageModel(_frameworkMessenger,
                                                                                _startUpPropertiesCache,
                                                                                _systemComponents,
                                                                                _gpuSurfaceManager,
                                                                                _gaussianWeightsAndOffsetsCache,
                                                                                sampleSurfaceWidth,
                                                                                sampleSurfaceHeight
                                                                            );
        }

        public IBlurStageModel CreateBlur2DEffectModel(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            return new BlurStageModel(_frameworkMessenger,
                                                                            _startUpPropertiesCache,
                                                                            _systemComponents,
                                                                            _gpuSurfaceManager,
                                                                            _gaussianWeightsAndOffsetsCache,
                                                                            sampleSurfaceWidth,
                                                                            sampleSurfaceHeight
                                                                        );
        }

        public IBlur1DStageModel CreateBlur1DEffectModel(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            return new Blur1DStageModel(_frameworkMessenger,
                                                                                _startUpPropertiesCache,
                                                                                _systemComponents,
                                                                                _gpuSurfaceManager,
                                                                                _gaussianWeightsAndOffsetsCache,
                                                                                sampleSurfaceWidth,
                                                                                sampleSurfaceHeight
                                                                            );
        }

        public IStyleEffectsStageModel CreateStyleEffectModel()
        {
            return new StyleEffectsStageModel(_frameworkMessenger, _systemComponents);
        }

        public IMeshRenderStageModel CreateMeshRenderModel()
        {
            return new MeshRenderStageModel(_frameworkMessenger, _systemComponents, _quadMeshBuilder);
        }

        public IDistortionStageModel CreateDistortionEffectStageModel(uint internalSurfaceWidth, uint internalSurfaceHeight)
        {
            return new DistortionStageModel(_drawQueueGroupFactory.Create(true),
                                                  _drawStageBatcherFactory.Create(),
                                                  _systemComponents,
                                                  _gpuSurfaceManager,
                                                  internalSurfaceWidth,
                                                  internalSurfaceHeight);
        }

        public IMixStageModel CreateMixStageModel()
        {
            return new MixStageModel(_frameworkMessenger);
        }

        public ICustomShaderStageModel CreateCustomStageModel(string fragmentShaderFilename, AssetSourceEnum assetType, ShaderUniformDescription[] uniformDescriptions, BlendState blendState)
        {
            return new CustomShaderStageModel(_frameworkMessenger, _systemComponents, _shaderTools, _pipelineFactory, _blendStateConverter, fragmentShaderFilename, assetType, uniformDescriptions, blendState);
        }

        public ICustomVeldridStageModel CreateCustomVeldridStage(CustomVeldridBase stage)
        {
            return new CustomVeldridStageModel(_frameworkMessenger, _systemComponents, _windowUpdater, stage);
        }
    }
}