namespace Yak2D.Graphics
{
    public class MixStage : IMixStage
    {
        public ulong Id { get; private set; }

        public MixStage(ulong id)
        {
            Id = id;
        }
    }
}