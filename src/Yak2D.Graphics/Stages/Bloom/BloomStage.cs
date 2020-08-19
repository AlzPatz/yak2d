namespace Yak2D.Graphics
{
    public class BloomStage : IBloomStage
    {
        public ulong Id { get; private set; }

        public BloomStage(ulong id)
        {
            Id = id;
        }
    }
}