using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface ISinglePassGaussianBlurRenderer
    {
        void Render(CommandList cl, Vector2 texelShiftSize, int numberSamplesPerSide, GpuSurface source, GpuSurface target);
        void ReInitialiseGpuResources();
    }
}