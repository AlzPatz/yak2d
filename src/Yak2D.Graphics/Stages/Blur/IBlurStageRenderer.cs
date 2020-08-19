using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBlurStageRenderer
    {
        void Render(CommandList cl, IBlurStageModel stage, GpuSurface source, GpuSurface target);
    }
}