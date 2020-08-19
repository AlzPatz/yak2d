namespace Yak2D.Graphics
{
    public class ViewportReference : IViewport
    {
        public ulong Id { get; private set; }

        public ViewportReference(ulong id)
        {
            Id = id;
        }
    }
}