namespace Yak2D.Graphics
{
    public class Blur1DStage : IBlur1DStage
    {
        public ulong Id { get; private set; }

        public Blur1DStage(ulong id)
        {
            Id = id;
        }
    }
}