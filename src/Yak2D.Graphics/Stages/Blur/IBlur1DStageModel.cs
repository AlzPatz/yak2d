using System.Numerics;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBlur1DStageModel : IRenderStageModel
    {
        float MixAmount { get; }

        Vector2 TexelShiftSize { get; }
        int NumberSamples { get; }
        ResizeSamplerType SampleType { get; }

        GpuSurface LinearSampledSurface { get; }

        GpuSurface AnistropicallySampledSurface { get; }

        void SetEffectTransition(ref Blur1DEffectConfiguration config, ref float transitionSeconds);
    }
}