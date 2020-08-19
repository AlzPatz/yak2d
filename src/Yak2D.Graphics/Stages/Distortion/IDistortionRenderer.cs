using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IDistortionRenderer
    {
        void Render(CommandList cl, IDistortionStageModel stage, GpuSurface source, GpuSurface shift, GpuSurface target);
        void ReInitialiseGpuResources();
    }
}