using System;
using NeoVeldrid;

namespace Yak2D.Internal
{
    public static class RgbaBytes
    {
        /// <summary>
        /// Build an 8 bit per channel pixel from 0 to 1 float channels (values outside 0 to 1 are clamped)
        /// </summary>
        public static RgbaByte FromFloats(float r, float g, float b, float a)
        {
            return new RgbaByte(ToByte(r), ToByte(g), ToByte(b), ToByte(a));
        }

        private static byte ToByte(float value)
        {
            return (byte)MathF.Round(Math.Clamp(value, 0.0f, 1.0f) * 255.0f);
        }
    }
}
