using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IBloomSamplingRenderer
    {
        void Render(CommandList cl, float brightnessThreshold, GpuSurface source, GpuSurface target, ResizeSamplerType samplerType);
        void ReInitialiseGpuResources();
    }
}