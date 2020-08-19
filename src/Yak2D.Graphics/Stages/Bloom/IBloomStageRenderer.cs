using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBloomStageRenderer
    {
        void Render(CommandList cl, IBloomStageModel stage, GpuSurface source, GpuSurface target);
    }
}