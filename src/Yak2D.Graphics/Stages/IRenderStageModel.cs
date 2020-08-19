using Veldrid;

namespace Yak2D.Graphics
{
    public interface IRenderStageModel
    {
        void Update(float seconds);
        void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command);
        void CacheInstanceInVisitor(IRenderStageVisitor visitor);
        void DestroyResources();
    }
}
