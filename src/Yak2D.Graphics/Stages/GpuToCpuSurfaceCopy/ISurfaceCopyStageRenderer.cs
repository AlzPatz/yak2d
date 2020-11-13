using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface ISurfaceCopyStageRenderer
    {
        void Render(CommandList cl, ISurfaceCopyStageModel stage, GpuSurface source);
    }
}