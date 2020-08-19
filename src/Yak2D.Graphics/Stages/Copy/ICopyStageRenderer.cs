using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface ICopyStageRenderer
    {
        void Render(CommandList cl, GpuSurface source, GpuSurface surface);
        void ReInitialiseGpuResources();
    }
}