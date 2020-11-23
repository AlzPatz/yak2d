using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class BlurStageRenderer : IBlurStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDownSamplingRenderer _blurSamplingRenderer;
        private readonly ISinglePassGaussianBlurRenderer _singlePassGaussianBlurRenderer;
        private readonly IBlurResultMixingRenderer _blur2DResultMixingRenderer;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcSpaceQuadVertexBuffer;

        public BlurStageRenderer(IFrameworkMessenger frameworkMessenger,
                                                                        IDownSamplingRenderer blurSamplingRenderer,
                                                                        ISinglePassGaussianBlurRenderer singlePassGaussianBlurRenderer,
                                                                        IBlurResultMixingRenderer blur2DResultMixingRenderer,
                                                                        IFullNdcSpaceQuadVertexBuffer ndcSpaceQuadVertexBuffer)
        {
            _frameworkMessenger = frameworkMessenger;
            _blurSamplingRenderer = blurSamplingRenderer;
            _singlePassGaussianBlurRenderer = singlePassGaussianBlurRenderer;
            _blur2DResultMixingRenderer = blur2DResultMixingRenderer;
            _ndcSpaceQuadVertexBuffer = ndcSpaceQuadVertexBuffer;
        }

        public void Render(CommandList cl, IBlurStageModel stage, GpuSurface source, GpuSurface target)
        {
            if (cl == null || stage == null || source == null || target == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Blur2D Stage Renderer null inputs, aborting");
                return;
            }

            //Downsample
            _blurSamplingRenderer.Render(cl, source, stage.LinearSampledSurface0, stage.SampleType);

            //Blur Horizontally
            _singlePassGaussianBlurRenderer.Render(cl, new Vector2(stage.TexelShiftSize.X, 0.0f), stage.NumberSamples, stage.LinearSampledSurface0, stage.LinearSampledSurface1);

            //Blur Vertically
            _singlePassGaussianBlurRenderer.Render(cl, new Vector2(0.0f, stage.TexelShiftSize.Y), stage.NumberSamples, stage.LinearSampledSurface1, stage.AnistropicallySampledSurface);

            //Mix
            _blur2DResultMixingRenderer.Render(cl, stage.MixAmount, source, stage.AnistropicallySampledSurface, target);
        }
    }
}