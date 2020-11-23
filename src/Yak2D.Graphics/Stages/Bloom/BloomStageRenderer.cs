using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class BloomStageRenderer : IBloomStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IBloomSamplingRenderer _bloomSamplingRenderer;
        private readonly ISinglePassGaussianBlurRenderer _singlePassGaussianBlurRenderer;
        private readonly IBloomResultMixingRenderer _bloomResultMixingRenderer;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcSpaceQuadVertexBuffer;

        public BloomStageRenderer(IFrameworkMessenger frameworkMessenger,
                                                                        IBloomSamplingRenderer bloomSamplingRenderer,
                                                                        ISinglePassGaussianBlurRenderer singlePassGaussianBlurRenderer,
                                                                        IBloomResultMixingRenderer bloomResultMixingRenderer,
                                                                        IFullNdcSpaceQuadVertexBuffer ndcSpaceQuadVertexBuffer)
        {
            _frameworkMessenger = frameworkMessenger;
            _bloomSamplingRenderer = bloomSamplingRenderer;
            _singlePassGaussianBlurRenderer = singlePassGaussianBlurRenderer;
            _bloomResultMixingRenderer = bloomResultMixingRenderer;
            _ndcSpaceQuadVertexBuffer = ndcSpaceQuadVertexBuffer;
        }

        public void Render(CommandList cl, IBloomStageModel stage, GpuSurface source, GpuSurface target)
        {
            if (cl == null || stage == null || source == null || target == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Bloom Stage Renderer null inputs, aborting");
                return;
            }

            //Downsample
            _bloomSamplingRenderer.Render(cl, stage.BrightnessThreshold, source, stage.LinearSampledSurface0, stage.SampleType);

            //Blur Horizontally
            _singlePassGaussianBlurRenderer.Render(cl, new Vector2(stage.TexelShiftSize.X, 0.0f), stage.NumberSamples, stage.LinearSampledSurface0, stage.LinearSampledSurface1);

            //Blur Vertically
            _singlePassGaussianBlurRenderer.Render(cl, new Vector2(0.0f, stage.TexelShiftSize.Y), stage.NumberSamples, stage.LinearSampledSurface1, stage.AnistropicallySampledSurface);

            //Mix
            _bloomResultMixingRenderer.Render(cl, stage, source, stage.AnistropicallySampledSurface, target);
        }
    }
}