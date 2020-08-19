using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class GraphicsShutdownManager : IGraphicsShutdownManager
    {

        private readonly IGraphics _graphics;
        private readonly ICameraManager _cameraManager;
        private readonly IRenderStageManager _renderStageManager;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IViewportManager _viewportManager;
        private readonly IColourEffectsStageRenderer _colourEffectStageRenderer;
        private readonly IBloomSamplingRenderer _bloomSamplingRenderer;
        private readonly IBloomResultMixingRenderer _bloomResultMixingRenderer;
        private readonly IBlurResultMixingRenderer _blurResultMixingRenderer;
        private readonly IDownSamplingRenderer _downSamplingRenderer;
        private readonly ISinglePassGaussianBlurRenderer _singlePassGaussianBlurRenderer;
        private readonly ICopyStageRenderer _copyStageRenderer;
        private readonly IDistortionGraidentShiftRenderer _distortionGradientShiftRenderer;
        private readonly IDistortionHeightRenderer _distortionHeightRenderer;
        private readonly IStyleEffectsStageRenderer _styleEffectRenderer;
        private readonly IMeshRenderStageRenderer _meshStageRenderer;
        private readonly IDistortionRenderer _distortionRenderer;
        private readonly IMixStageRenderer _mixStageRenderer;


        public GraphicsShutdownManager(
                                        IGraphics graphics,
                                        ICameraManager cameraManager,
                                        IRenderStageManager renderStageManager,
                                        IPipelineFactory pipelineFactory,
                                        IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer,
                                        IViewportManager viewportManager,
                                        IColourEffectsStageRenderer colourEffectStageRenderer,
                                        IBloomSamplingRenderer bloomSamplingRenderer,
                                        IBloomResultMixingRenderer bloomResultMixingRenderer,
                                        IBlur1DStageRenderer blur1dEffectStageRenderer,
                                        IBlurResultMixingRenderer blurResultMixingRenderer,
                                        IDownSamplingRenderer downSamplingRenderer,
                                        ISinglePassGaussianBlurRenderer singlePassGaussianBlurRenderer,
                                        ICopyStageRenderer copyStageRenderer,
                                         IDistortionGraidentShiftRenderer distortionGradientShiftRenderer,
                                        IDistortionHeightRenderer distortionHeightRenderer,
                                        IStyleEffectsStageRenderer styleEffectRenderer,
                                        IMeshRenderStageRenderer meshStageRenderer,
                                        IDistortionRenderer distortionRenderer,
                                        IMixStageRenderer mixStageRenderer
        )
        {
            _graphics = graphics;
            _cameraManager = cameraManager;
            _renderStageManager = renderStageManager;
            _pipelineFactory = pipelineFactory;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;
            _viewportManager = viewportManager;
            _colourEffectStageRenderer = colourEffectStageRenderer;
            _bloomSamplingRenderer = bloomSamplingRenderer;
            _bloomResultMixingRenderer = bloomResultMixingRenderer;
            _blurResultMixingRenderer = blurResultMixingRenderer;
            _downSamplingRenderer = downSamplingRenderer;
            _singlePassGaussianBlurRenderer = singlePassGaussianBlurRenderer;
            _copyStageRenderer = copyStageRenderer;
            _distortionGradientShiftRenderer = distortionGradientShiftRenderer;
            _distortionHeightRenderer = distortionHeightRenderer;
            _styleEffectRenderer = styleEffectRenderer;
            _meshStageRenderer = meshStageRenderer;
            _distortionRenderer = distortionRenderer;
            _mixStageRenderer = mixStageRenderer;
        }

        public void Shutdown()
        {
            //Some resources will be doubly disposed, and veldrid's resource dispose collector 
            //factory would also ensure most resources are disposed of anyway. 
            //However, manually doing this for completeness, future usage and to catch non-veldrid resources
            //Some shutdowns also just call "destroyall" type methods that already exist. we could cut out the middle man

            _cameraManager.Shutdown();
            _renderStageManager.Shutdown();
            _viewportManager.Shutdown();
            _pipelineFactory.ClearPipelineList();
        }
    }
}