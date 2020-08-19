using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface IMeshRenderStageRenderer
    {
        void Render(CommandList cl, IMeshRenderStageModel stage, GpuSurface source, GpuSurface surface, ICameraModel3D camera);
        void ReInitialiseGpuResources();
    }
}