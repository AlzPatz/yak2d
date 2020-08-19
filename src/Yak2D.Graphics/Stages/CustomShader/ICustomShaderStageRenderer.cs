using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface ICustomShaderStageRenderer
    {
        void Render(CommandList cl, ICustomShaderStageModel stage, GpuSurface t0, GpuSurface t1, GpuSurface t2, GpuSurface t3, GpuSurface target);
        void DisposeOfGpuResources();
        void ReInitialiseGpuResources();
    }
}