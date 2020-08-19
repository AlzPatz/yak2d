using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IColourEffectsStageRenderer
    {
        void Render(CommandList cl, IColourEffectsStageModel stage, GpuSurface surface, GpuSurface source);
        void ReInitialiseGpuResources();
    }
}