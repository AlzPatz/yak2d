using System.Numerics;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBloomStageModel : IRenderStageModel
    {
        float BrightnessThreshold { get; }
        float MixAmount { get; }

        Vector2 TexelShiftSize { get; }
        int NumberSamples { get; }
        ResizeSamplerType SampleType { get; }

        GpuSurface LinearSampledSurface0 { get; }
        GpuSurface LinearSampledSurface1 { get; }
        GpuSurface AnistropicallySampledSurface { get; }

        void SetEffectTransition(ref BloomEffectConfiguration config, ref float transitionSeconds);
    }
}