using System.Numerics;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBlurStageModel : IRenderStageModel
    {
        float MixAmount { get; }

        Vector2 TexelShiftSize { get; }
        int NumberSamples { get; }
        ResizeSamplerType SampleType { get; }

        GpuSurface LinearSampledSurface0 { get; }
        GpuSurface LinearSampledSurface1 { get; }
        GpuSurface AnistropicallySampledSurface { get; }

        void SetEffectTransition(ref BlurEffectConfiguration config, ref float transitionSeconds);
    }
}