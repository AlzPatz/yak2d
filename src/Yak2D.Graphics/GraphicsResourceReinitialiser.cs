using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class GraphicsResourceReinitialiser : IGraphicsResourceReinitialiser
    {
        private readonly IGraphics _graphics;
        private readonly ICameraManager _cameraManager;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IRenderStageManager _renderStageManager;
        private readonly IViewportManager _viewportManager;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcSpaceQuadVertexBuffer;
        private readonly IBloomResultMixingRenderer _bloomResultMixingRenderer;
        private readonly IBloomSamplingRenderer _bloomSamplingRender;
        private readonly IBlurResultMixingRenderer _blurResultMixingRenderer;
        private readonly IColourEffectsStageRenderer _colourEffectStageRenderer;
        private readonly ICopyStageRenderer _copyStageRenderer;
        private readonly IDistortionGraidentShiftRenderer _distortionGradientShiftRenderer;
        private readonly IDistortionHeightRenderer _distortionHeightRenderer;
        private readonly IDistortionRenderer _distortionRenderer;
        private readonly IDownSamplingRenderer _downSamplingRenderer;
        private readonly IDrawStageRenderer _drawStageRenderer;
        private readonly IMeshRenderStageRenderer _meshStageRenderer;
        private readonly IMixStageRenderer _mixStageRenderer;
        private readonly ISinglePassGaussianBlurRenderer _singlePassGaussianBlurRenderer;
        private readonly IStyleEffectsStageRenderer _styleEffectStageRenderer;

        public GraphicsResourceReinitialiser(
            IGraphics graphics,
            ICameraManager cameraManager,
            IPipelineFactory pipelineFactory,
            IRenderStageManager renderStageManager,
            IViewportManager viewportManager,
            IFullNdcSpaceQuadVertexBuffer ndcSpaceQuadVertexBuffer,
            IBloomResultMixingRenderer bloomResultMixingRenderer,
            IBloomSamplingRenderer bloomSamplingRender,
            IBlurResultMixingRenderer blurResultMixingRenderer,
            IColourEffectsStageRenderer colourEffectStageRenderer,
            ICopyStageRenderer copyStageRenderer,
            IDistortionGraidentShiftRenderer distortionGradientShiftRenderer,
            IDistortionHeightRenderer distortionHeightRenderer,
            IDistortionRenderer distortionRenderer,
            IDownSamplingRenderer downSamplingRenderer,
            IDrawStageRenderer drawStageRenderer,
            IMeshRenderStageRenderer meshStageRenderer,
            IMixStageRenderer mixStageRenderer,
            ISinglePassGaussianBlurRenderer singlePassGaussianBlurRenderer,
            IStyleEffectsStageRenderer styleEffectStageRenderer
        )
        {
            _graphics = graphics;
            _cameraManager = cameraManager;
            _pipelineFactory = pipelineFactory;
            _renderStageManager = renderStageManager;
            _viewportManager = viewportManager;

            _ndcSpaceQuadVertexBuffer = ndcSpaceQuadVertexBuffer;

            _bloomResultMixingRenderer = bloomResultMixingRenderer;
            _bloomSamplingRender = bloomSamplingRender;
            _blurResultMixingRenderer = blurResultMixingRenderer;
            _colourEffectStageRenderer = colourEffectStageRenderer;
            _copyStageRenderer = copyStageRenderer;
            _distortionGradientShiftRenderer = distortionGradientShiftRenderer;
            _distortionHeightRenderer = distortionHeightRenderer;
            _distortionRenderer = distortionRenderer;
            _downSamplingRenderer = downSamplingRenderer;
            _drawStageRenderer = drawStageRenderer;
            _meshStageRenderer = meshStageRenderer;
            _mixStageRenderer = mixStageRenderer;
            _singlePassGaussianBlurRenderer = singlePassGaussianBlurRenderer;
            _styleEffectStageRenderer = styleEffectStageRenderer;
        }

        public void ReInitialise()
        {
            //Many resources will already be lost as the GpuDevice has been changed. But we still call Dispose/Shutdown routines for them
            //before reinitialisation. Shouldn't do any harm
            _graphics.ReInitalise();

            _cameraManager.DestroyAllCameras();
            _pipelineFactory.ReInitialise();
            _renderStageManager.ReInitialise();
            _viewportManager.ReInitialise();

            _ndcSpaceQuadVertexBuffer.ReInitialise();

            _bloomResultMixingRenderer.ReInitialiseGpuResources();
            _bloomSamplingRender.ReInitialiseGpuResources();
            _blurResultMixingRenderer.ReInitialiseGpuResources();
            _colourEffectStageRenderer.ReInitialiseGpuResources();
            _copyStageRenderer.ReInitialiseGpuResources();
            _distortionGradientShiftRenderer.ReInitialiseGpuResources();
            _distortionHeightRenderer.ReInitialiseGpuResources();
            _distortionRenderer.ReInitialiseGpuResources();
            _downSamplingRenderer.ReInitialiseGpuResources();
            _drawStageRenderer.ReInitialiseGpuResources();
            _meshStageRenderer.ReInitialiseGpuResources();
            _mixStageRenderer.ReInitialiseGpuResources();
            _singlePassGaussianBlurRenderer.ReInitialiseGpuResources();
            _styleEffectStageRenderer.ReInitialiseGpuResources();
        }
    }
}
