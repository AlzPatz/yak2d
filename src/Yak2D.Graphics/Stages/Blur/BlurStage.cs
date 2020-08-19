namespace Yak2D.Graphics
{
    public class BlurStage : IBlurStage
    {
        public ulong Id { get; private set; }

        public BlurStage(ulong id)
        {
            Id = id;
        }
    }
}