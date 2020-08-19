namespace Yak2D.Graphics
{
    public class StyleEffectsStage : IStyleEffectsStage
    {
        public ulong Id { get; private set; }

        public StyleEffectsStage(ulong id)
        {
            Id = id;
        }
    }
}