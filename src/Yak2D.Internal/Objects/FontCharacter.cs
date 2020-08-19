namespace Yak2D.Internal
{
    public class FontCharacter
    {
        public float X0 { get; }
        public float X1 { get; }
        public float Y0 { get; }
        public float Y1 { get; }
        public float XOffset { get; }
        public float YOffset { get; }
        public float Width { get; }
        public float Height { get; }
        public float XAdvance { get; }
        public int Page { get; }

        public FontCharacter(
            int pageWidth,
            int pageHeight,
            int x,
            int y,
            int width,
            int height,
            int xOffset,
            int yOffset,
            int xAdvance,
            int page)
        {
            var pw = (float)pageWidth;
            var ph = (float)pageHeight;

            X0 = x / pw;
            X1 = (x + width) / pw;
            Y0 = y / ph;
            Y1 = (y + height) / ph;
            XOffset = xOffset;
            YOffset = yOffset;
            Width = width;
            Height = height;
            XAdvance = xAdvance;
            Page = page;
        }
    }
}