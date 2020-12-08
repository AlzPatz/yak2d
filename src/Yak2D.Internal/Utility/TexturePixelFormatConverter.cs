using Veldrid;

namespace Yak2D.Internal
{
    public static class TexturePixelFormatConverter
    {
        public static PixelFormat ConvertYakToVeldrid(TexturePixelFormat pixelFormat)
        {
            return (PixelFormat)pixelFormat;
        }

        public static TexturePixelFormat ConvertVeldridToYak(PixelFormat pixelFormat)
        {
            return (TexturePixelFormat)pixelFormat;
        }
    }
}