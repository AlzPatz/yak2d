using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IDistortionStageModel : IDrawStageModel
    {
        GpuSurface HeightMapSurface { get; }
        GpuSurface GradientShiftSurface { get; }
        ResourceSet InternalSurfacePixelShiftUniform { get; }
        float DistortionScalar { get; }

        void SetEffectTransition(ref DistortionEffectConfiguration config, ref float transitionSeconds);
    }
}