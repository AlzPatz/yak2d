using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBloomResultMixingRenderer
    {
        void Render(CommandList cl, IBloomStageModel stage, GpuSurface original, GpuSurface bloom, GpuSurface target);
        void ReInitialiseGpuResources();
    }
}