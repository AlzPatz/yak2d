namespace Yak2D.Font
{
    public class FontReference : IFont
    {
        public ulong Id { get; private set; }

        public FontReference(ulong id)
        {
            Id = id;
        }
    }
}
