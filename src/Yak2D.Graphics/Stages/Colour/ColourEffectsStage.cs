namespace Yak2D.Graphics
{
    public class ColourEffectsStage : IColourEffectsStage
    {
        public ulong Id { get; private set; }

        public ColourEffectsStage(ulong id)
        {
            Id = id;
        }
    }
}