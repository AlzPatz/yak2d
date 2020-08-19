using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IStyleEffectsStageRenderer
    {
        void Render(CommandList cl, IStyleEffectsStageModel stage, GpuSurface source, GpuSurface target);
        void ReInitialiseGpuResources();
    }
}