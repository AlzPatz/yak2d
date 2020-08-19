using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IDistortionGraidentShiftRenderer
    {
        void Render(CommandList cl, IDistortionStageModel stage, GpuSurface source, GpuSurface target);
        void ReInitialiseGpuResources();
    }
}