using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IDownSamplingRenderer
    {
        void Render(CommandList cl, GpuSurface source, GpuSurface target, ResizeSamplerType downSamplerType);
        void ReInitialiseGpuResources();
    }
}