using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBlur1DStageRenderer
    {
        void Render(CommandList cl, IBlur1DStageModel stage, GpuSurface source, GpuSurface target);
    }
}