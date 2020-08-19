namespace Yak2D.Graphics
{
    public class MeshRenderStage : IMeshRenderStage
    {
        public ulong Id { get; private set; }

        public MeshRenderStage(ulong id)
        {
            Id = id;
        }
    }
}