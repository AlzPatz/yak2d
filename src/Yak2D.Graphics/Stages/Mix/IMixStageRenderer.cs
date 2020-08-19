using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IMixStageRenderer
    {
        void Render(CommandList cl, IMixStageModel stage, GpuSurface mix, GpuSurface t0, GpuSurface t1, GpuSurface t2, GpuSurface t3, GpuSurface target);
        void ReInitialiseGpuResources();
    }
}