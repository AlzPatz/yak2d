namespace Yak2D.Graphics
{
    public class DrawStage : IDrawStage
    {
        public ulong Id { get; private set; }

        public DrawStage(ulong id)
        {
            Id = id;
        }
    }
}