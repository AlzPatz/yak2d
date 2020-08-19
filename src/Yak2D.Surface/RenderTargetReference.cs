namespace Yak2D.Surface
{
    public class RenderTargetReference : IRenderTarget
    {
        public ulong Id { get; private set; }

        public RenderTargetReference(ulong id)
        {
            Id = id;
        }
    }
}
