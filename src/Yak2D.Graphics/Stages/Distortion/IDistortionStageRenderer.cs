using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IDistortionStageRenderer
    {
        void Render(CommandList cl,
                    IDistortionStageModel stage,
                    GpuSurface source,
                    GpuSurface target,
                    ICameraModel2D camera);
    }
}