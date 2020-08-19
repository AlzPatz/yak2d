using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IDistortionHeightRenderer
    {
        void Render(CommandList cl, IDistortionStageModel stage, GpuSurface target, ICameraModel2D camera);
        void ReInitialiseGpuResources();
    }
}