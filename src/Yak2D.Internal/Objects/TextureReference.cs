namespace Yak2D.Internal
{
    public class TextureReference : ITexture
    {
        public ulong Id { get; private set; }

        public TextureReference(ulong id)
        {
            Id = id;
        }
    }
}