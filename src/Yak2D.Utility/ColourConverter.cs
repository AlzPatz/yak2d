using System.Numerics;
using Veldrid;

namespace Yak2D.Utility
{
    public static class ColourConverter
    {
        public static RgbaFloat ConvertToRgbaFloat(Colour colour)
        {
            return new RgbaFloat(colour.R, colour.G, colour.B, colour.A);
        }

        public static Vector4 ConvertToVec4(Colour colour)
        {
            return new Vector4(colour.R, colour.G, colour.B, colour.A);
        }
    }
}