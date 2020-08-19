using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBlurResultMixingRenderer
    {
        void Render(CommandList cl, float mixAmount, GpuSurface original, GpuSurface blurred, GpuSurface target);
        void ReInitialiseGpuResources();
    }
}