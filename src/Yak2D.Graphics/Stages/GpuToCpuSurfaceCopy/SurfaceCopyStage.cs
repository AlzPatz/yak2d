namespace Yak2D.Graphics
{
    public class SurfaceCopyStage : ISurfaceCopyStage
    {
        public ulong Id { get; private set; }

        public SurfaceCopyStage(ulong id)
        {
            Id = id;
        }
    }
}