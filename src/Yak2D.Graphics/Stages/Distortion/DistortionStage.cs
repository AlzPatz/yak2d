namespace Yak2D.Graphics
{
    public class DistortionStage : IDistortionStage
    {
        public ulong Id { get; private set; }

        public DistortionStage(ulong id)
        {
            Id = id;
        }
    }
}