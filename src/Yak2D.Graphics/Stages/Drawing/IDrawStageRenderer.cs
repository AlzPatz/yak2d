using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IDrawStageRenderer
    {
        void Render(CommandList cl, IDrawStageModel stage, GpuSurface surface, ICameraModel2D camera);
        void ReInitialiseGpuResources();
    }
}