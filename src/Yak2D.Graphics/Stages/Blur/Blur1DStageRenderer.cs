using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class Blur1DStageRenderer : IBlur1DStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDownSamplingRenderer _blurSamplingRenderer;
        private readonly ISinglePassGaussianBlurRenderer _singlePassGaussianBlurRenderer;
        private readonly IBlurResultMixingRenderer _blurResultMixingRenderer;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcSpaceQuadVertexBuffer;

        public Blur1DStageRenderer(IFrameworkMessenger frameworkMessenger,
                                                                        IDownSamplingRenderer blurSamplingRenderer,
                                                                        ISinglePassGaussianBlurRenderer singlePassGaussianBlurRenderer,
                                                                        IBlurResultMixingRenderer blur2DResultMixingRenderer,
                                                                        IFullNdcSpaceQuadVertexBuffer ndcSpaceQuadVertexBuffer)
        {
            _frameworkMessenger = frameworkMessenger;
            _blurSamplingRenderer = blurSamplingRenderer;
            _singlePassGaussianBlurRenderer = singlePassGaussianBlurRenderer;
            _blurResultMixingRenderer = blur2DResultMixingRenderer;
            _ndcSpaceQuadVertexBuffer = ndcSpaceQuadVertexBuffer;
        }

        public void Render(CommandList cl, IBlur1DStageModel stage, GpuSurface source, GpuSurface target)
        {
            if (cl == null || stage == null || source == null || target == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Blur1D Stage Renderer null inputs, aborting");
                return;
            }

            //Downsample
            _blurSamplingRenderer.Render(cl, source, stage.LinearSampledSurface, stage.SampleType);

            //Blur 
            _singlePassGaussianBlurRenderer.Render(cl, stage.TexelShiftSize, stage.NumberSamples, stage.LinearSampledSurface, stage.AnistropicallySampledSurface);

            //Mix
            _blurResultMixingRenderer.Render(cl, stage.MixAmount, source, stage.AnistropicallySampledSurface, target);
        }
    }
}