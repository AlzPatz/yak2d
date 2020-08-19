using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface ICustomVeldridStageRenderer
    {
        void Render(CommandList cl, ICustomVeldridStageModel stage, GpuSurface t0, GpuSurface t1, GpuSurface t2, GpuSurface t3, GpuSurface target);
        void DisposeOfGpuResources();
        void ReInitialiseGpuResources();
    }
}