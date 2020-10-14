using System;

namespace Yak2D
{
    /// <summary>
    /// A four component colour, with RGBA values each within 0 to 1 range
    /// </summary>
    public readonly struct Colour : IEquatable<Colour>
    {
        public Colour(float r, float g, float b, float a)
        {
            R = Clamp(r);
            G = Clamp(g);
            B = Clamp(b);
            A = Clamp(a);
        }

        public float R { get; }
        public float G { get; }
        public float B { get; }
        public float A { get; }

        private static float Clamp(float f)
        {
            if (f < 0.0f)
                return 0.0f;
            if (f > 1.0f)
                return 1.0f;
            return f;
        }

        public static Colour operator +(Colour c0, Colour c1)
        {
            return new Colour(Clamp(c0.R + c1.R), Clamp(c0.G + c1.G), Clamp(c0.B + c1.B), Clamp(c0.A + c1.A));
        }

        public static Colour operator -(Colour c0, Colour c1)
        {
            return new Colour(Clamp(c0.R - c1.R), Clamp(c0.G - c1.G), Clamp(c0.B - c1.B), Clamp(c0.A - c1.A));
        }

        public static Colour operator *(Colour c0, Colour c1)
        {
            return new Colour(Clamp(c0.R * c1.R), Clamp(c0.G * c1.G), Clamp(c0.B * c1.B), Clamp(c0.A * c1.A));
        }

        public static Colour operator *(float f, Colour c)
        {
            return new Colour(Clamp(f * c.R), Clamp(f * c.G), Clamp(f * c.B), Clamp(f * c.A));
        }

        public static Colour operator *(Colour c, float f)
        {
            return new Colour(Clamp(f * c.R), Clamp(f * c.G), Clamp(f * c.B), Clamp(f * c.A));
        }

        public static Colour Clear { get { return new Colour(0.0f, 0.0f, 0.0f, 0.0f); } }

        //X11 Colour Names
        public static Colour AliceBlue { get { return new Colour(0.94f, 0.97f, 1.00f, 1.00f); } }
        public static Colour AntiqueWhite { get { return new Colour(0.98f, 0.92f, 0.84f, 1.00f); } }
        public static Colour Aqua { get { return new Colour(0.00f, 1.00f, 1.00f, 1.00f); } }
        public static Colour Aquamarine { get { return new Colour(0.50f, 1.00f, 0.83f, 1.00f); } }
        public static Colour Azure { get { return new Colour(0.94f, 1.00f, 1.00f, 1.00f); } }
        public static Colour Beige { get { return new Colour(0.96f, 0.96f, 0.86f, 1.00f); } }
        public static Colour Bisque { get { return new Colour(1.00f, 0.89f, 0.77f, 1.00f); } }
        public static Colour Black { get { return new Colour(0.00f, 0.00f, 0.00f, 1.00f); } }
        public static Colour BlanchedAlmond { get { return new Colour(1.00f, 0.92f, 0.80f, 1.00f); } }
        public static Colour Blue { get { return new Colour(0.00f, 0.00f, 1.00f, 1.00f); } }
        public static Colour BlueViolet { get { return new Colour(0.54f, 0.17f, 0.89f, 1.00f); } }
        public static Colour Brown { get { return new Colour(0.65f, 0.16f, 0.16f, 1.00f); } }
        public static Colour Burlywood { get { return new Colour(0.87f, 0.72f, 0.53f, 1.00f); } }
        public static Colour CadetBlue { get { return new Colour(0.37f, 0.62f, 0.63f, 1.00f); } }
        public static Colour Chartreuse { get { return new Colour(0.50f, 1.00f, 0.00f, 1.00f); } }
        public static Colour Chocolate { get { return new Colour(0.82f, 0.41f, 0.12f, 1.00f); } }
        public static Colour Coral { get { return new Colour(1.00f, 0.50f, 0.31f, 1.00f); } }
        public static Colour CornflowerBlue { get { return new Colour(0.39f, 0.58f, 0.93f, 1.00f); } }
        public static Colour Cornsilk { get { return new Colour(1.00f, 0.97f, 0.86f, 1.00f); } }
        public static Colour Crimson { get { return new Colour(0.86f, 0.08f, 0.24f, 1.00f); } }
        public static Colour Cyan { get { return new Colour(0.00f, 1.00f, 1.00f, 1.00f); } }
        public static Colour DarkBlue { get { return new Colour(0.00f, 0.00f, 0.55f, 1.00f); } }
        public static Colour DarkCyan { get { return new Colour(0.00f, 0.55f, 0.55f, 1.00f); } }
        public static Colour DarkGoldenrod { get { return new Colour(0.72f, 0.53f, 0.04f, 1.00f); } }
        public static Colour DarkGray { get { return new Colour(0.66f, 0.66f, 0.66f, 1.00f); } }
        public static Colour DarkGreen { get { return new Colour(0.00f, 0.39f, 0.00f, 1.00f); } }
        public static Colour DarkKhaki { get { return new Colour(0.74f, 0.72f, 0.42f, 1.00f); } }
        public static Colour DarkMagenta { get { return new Colour(0.55f, 0.00f, 0.55f, 1.00f); } }
        public static Colour DarkOliveGreen { get { return new Colour(0.33f, 0.42f, 0.18f, 1.00f); } }
        public static Colour DarkOrange { get { return new Colour(1.00f, 0.55f, 0.00f, 1.00f); } }
        public static Colour DarkOrchid { get { return new Colour(0.60f, 0.20f, 0.80f, 1.00f); } }
        public static Colour DarkRed { get { return new Colour(0.55f, 0.00f, 0.00f, 1.00f); } }
        public static Colour DarkSalmon { get { return new Colour(0.91f, 0.59f, 0.48f, 1.00f); } }
        public static Colour DarkSeaGreen { get { return new Colour(0.56f, 0.74f, 0.56f, 1.00f); } }
        public static Colour DarkSlateBlue { get { return new Colour(0.28f, 0.24f, 0.55f, 1.00f); } }
        public static Colour DarkSlateGray { get { return new Colour(0.18f, 0.31f, 0.31f, 1.00f); } }
        public static Colour DarkTurquoise { get { return new Colour(0.00f, 0.81f, 0.82f, 1.00f); } }
        public static Colour DarkViolet { get { return new Colour(0.58f, 0.00f, 0.83f, 1.00f); } }
        public static Colour DeepPink { get { return new Colour(1.00f, 0.08f, 0.58f, 1.00f); } }
        public static Colour DeepSkyBlue { get { return new Colour(0.00f, 0.75f, 1.00f, 1.00f); } }
        public static Colour DimGray { get { return new Colour(0.41f, 0.41f, 0.41f, 1.00f); } }
        public static Colour DodgerBlue { get { return new Colour(0.12f, 0.56f, 1.00f, 1.00f); } }
        public static Colour Firebrick { get { return new Colour(0.70f, 0.13f, 0.13f, 1.00f); } }
        public static Colour FloralWhite { get { return new Colour(1.00f, 0.98f, 0.94f, 1.00f); } }
        public static Colour ForestGreen { get { return new Colour(0.13f, 0.55f, 0.13f, 1.00f); } }
        public static Colour Fuchsia { get { return new Colour(1.00f, 0.00f, 1.00f, 1.00f); } }
        public static Colour Gainsboro { get { return new Colour(0.86f, 0.86f, 0.86f, 1.00f); } }
        public static Colour GhostWhite { get { return new Colour(0.97f, 0.97f, 1.00f, 1.00f); } }
        public static Colour Gold { get { return new Colour(1.00f, 0.84f, 0.00f, 1.00f); } }
        public static Colour Goldenrod { get { return new Colour(0.85f, 0.65f, 0.13f, 1.00f); } }
        public static Colour Gray { get { return new Colour(0.75f, 0.75f, 0.75f, 1.00f); } }
        public static Colour WebGray { get { return new Colour(0.50f, 0.50f, 0.50f, 1.00f); } }
        public static Colour Green { get { return new Colour(0.00f, 1.00f, 0.00f, 1.00f); } }
        public static Colour WebGreen { get { return new Colour(0.00f, 0.50f, 0.00f, 1.00f); } }
        public static Colour GreenYellow { get { return new Colour(0.68f, 1.00f, 0.18f, 1.00f); } }
        public static Colour Honeydew { get { return new Colour(0.94f, 1.00f, 0.94f, 1.00f); } }
        public static Colour HotPink { get { return new Colour(1.00f, 0.41f, 0.71f, 1.00f); } }
        public static Colour IndianRed { get { return new Colour(0.80f, 0.36f, 0.36f, 1.00f); } }
        public static Colour Indigo { get { return new Colour(0.29f, 0.00f, 0.51f, 1.00f); } }
        public static Colour Ivory { get { return new Colour(1.00f, 1.00f, 0.94f, 1.00f); } }
        public static Colour Khaki { get { return new Colour(0.94f, 0.90f, 0.55f, 1.00f); } }
        public static Colour Lavender { get { return new Colour(0.90f, 0.90f, 0.98f, 1.00f); } }
        public static Colour LavenderBlush { get { return new Colour(1.00f, 0.94f, 0.96f, 1.00f); } }
        public static Colour LawnGreen { get { return new Colour(0.49f, 0.99f, 0.00f, 1.00f); } }
        public static Colour LemonChiffon { get { return new Colour(1.00f, 0.98f, 0.80f, 1.00f); } }
        public static Colour LightBlue { get { return new Colour(0.68f, 0.85f, 0.90f, 1.00f); } }
        public static Colour LightCoral { get { return new Colour(0.94f, 0.50f, 0.50f, 1.00f); } }
        public static Colour LightCyan { get { return new Colour(0.88f, 1.00f, 1.00f, 1.00f); } }
        public static Colour LightGoldenrod { get { return new Colour(0.98f, 0.98f, 0.82f, 1.00f); } }
        public static Colour LightGray { get { return new Colour(0.83f, 0.83f, 0.83f, 1.00f); } }
        public static Colour LightGreen { get { return new Colour(0.56f, 0.93f, 0.56f, 1.00f); } }
        public static Colour LightPink { get { return new Colour(1.00f, 0.71f, 0.76f, 1.00f); } }
        public static Colour LightSalmon { get { return new Colour(1.00f, 0.63f, 0.48f, 1.00f); } }
        public static Colour LightSeaGreen { get { return new Colour(0.13f, 0.70f, 0.67f, 1.00f); } }
        public static Colour LightSkyBlue { get { return new Colour(0.53f, 0.81f, 0.98f, 1.00f); } }
        public static Colour LightSlateGray { get { return new Colour(0.47f, 0.53f, 0.60f, 1.00f); } }
        public static Colour LightSteelBlue { get { return new Colour(0.69f, 0.77f, 0.87f, 1.00f); } }
        public static Colour LightYellow { get { return new Colour(1.00f, 1.00f, 0.88f, 1.00f); } }
        public static Colour Lime { get { return new Colour(0.00f, 1.00f, 0.00f, 1.00f); } }
        public static Colour LimeGreen { get { return new Colour(0.20f, 0.80f, 0.20f, 1.00f); } }
        public static Colour Linen { get { return new Colour(0.98f, 0.94f, 0.90f, 1.00f); } }
        public static Colour Magenta { get { return new Colour(1.00f, 0.00f, 1.00f, 1.00f); } }
        public static Colour Maroon { get { return new Colour(0.69f, 0.19f, 0.38f, 1.00f); } }
        public static Colour WebMaroon { get { return new Colour(0.50f, 0.00f, 0.00f, 1.00f); } }
        public static Colour MediumAquamarine { get { return new Colour(0.40f, 0.80f, 0.67f, 1.00f); } }
        public static Colour MediumBlue { get { return new Colour(0.00f, 0.00f, 0.80f, 1.00f); } }
        public static Colour MediumOrchid { get { return new Colour(0.73f, 0.33f, 0.83f, 1.00f); } }
        public static Colour MediumPurple { get { return new Colour(0.58f, 0.44f, 0.86f, 1.00f); } }
        public static Colour MediumSeaGreen { get { return new Colour(0.24f, 0.70f, 0.44f, 1.00f); } }
        public static Colour MediumSlateBlue { get { return new Colour(0.48f, 0.41f, 0.93f, 1.00f); } }
        public static Colour MediumSpringGreen { get { return new Colour(0.00f, 0.98f, 0.60f, 1.00f); } }
        public static Colour MediumTurquoise { get { return new Colour(0.28f, 0.82f, 0.80f, 1.00f); } }
        public static Colour MediumVioletRed { get { return new Colour(0.78f, 0.08f, 0.52f, 1.00f); } }
        public static Colour MidnightBlue { get { return new Colour(0.10f, 0.10f, 0.44f, 1.00f); } }
        public static Colour MintCream { get { return new Colour(0.96f, 1.00f, 0.98f, 1.00f); } }
        public static Colour MistyRose { get { return new Colour(1.00f, 0.89f, 0.88f, 1.00f); } }
        public static Colour Moccasin { get { return new Colour(1.00f, 0.89f, 0.71f, 1.00f); } }
        public static Colour NavajoWhite { get { return new Colour(1.00f, 0.87f, 0.68f, 1.00f); } }
        public static Colour NavyBlue { get { return new Colour(0.00f, 0.00f, 0.50f, 1.00f); } }
        public static Colour OldLace { get { return new Colour(0.99f, 0.96f, 0.90f, 1.00f); } }
        public static Colour Olive { get { return new Colour(0.50f, 0.50f, 0.00f, 1.00f); } }
        public static Colour OliveDrab { get { return new Colour(0.42f, 0.56f, 0.14f, 1.00f); } }
        public static Colour Orange { get { return new Colour(1.00f, 0.65f, 0.00f, 1.00f); } }
        public static Colour OrangeRed { get { return new Colour(1.00f, 0.27f, 0.00f, 1.00f); } }
        public static Colour Orchid { get { return new Colour(0.85f, 0.44f, 0.84f, 1.00f); } }
        public static Colour PaleGoldenrod { get { return new Colour(0.93f, 0.91f, 0.67f, 1.00f); } }
        public static Colour PaleGreen { get { return new Colour(0.60f, 0.98f, 0.60f, 1.00f); } }
        public static Colour PaleTurquoise { get { return new Colour(0.69f, 0.93f, 0.93f, 1.00f); } }
        public static Colour PaleVioletRed { get { return new Colour(0.86f, 0.44f, 0.58f, 1.00f); } }
        public static Colour PapayaWhip { get { return new Colour(1.00f, 0.94f, 0.84f, 1.00f); } }
        public static Colour PeachPuff { get { return new Colour(1.00f, 0.85f, 0.73f, 1.00f); } }
        public static Colour Peru { get { return new Colour(0.80f, 0.52f, 0.25f, 1.00f); } }
        public static Colour Pink { get { return new Colour(1.00f, 0.75f, 0.80f, 1.00f); } }
        public static Colour Plum { get { return new Colour(0.87f, 0.63f, 0.87f, 1.00f); } }
        public static Colour PowderBlue { get { return new Colour(0.69f, 0.88f, 0.90f, 1.00f); } }
        public static Colour Purple { get { return new Colour(0.63f, 0.13f, 0.94f, 1.00f); } }
        public static Colour WebPurple { get { return new Colour(0.50f, 0.00f, 0.50f, 1.00f); } }
        public static Colour RebeccaPurple { get { return new Colour(0.40f, 0.20f, 0.60f, 1.00f); } }
        public static Colour Red { get { return new Colour(1.00f, 0.00f, 0.00f, 1.00f); } }
        public static Colour RosyBrown { get { return new Colour(0.74f, 0.56f, 0.56f, 1.00f); } }
        public static Colour RoyalBlue { get { return new Colour(0.25f, 0.41f, 0.88f, 1.00f); } }
        public static Colour SaddleBrown { get { return new Colour(0.55f, 0.27f, 0.07f, 1.00f); } }
        public static Colour Salmon { get { return new Colour(0.98f, 0.50f, 0.45f, 1.00f); } }
        public static Colour SandyBrown { get { return new Colour(0.96f, 0.64f, 0.38f, 1.00f); } }
        public static Colour SeaGreen { get { return new Colour(0.18f, 0.55f, 0.34f, 1.00f); } }
        public static Colour Seashell { get { return new Colour(1.00f, 0.96f, 0.93f, 1.00f); } }
        public static Colour Sienna { get { return new Colour(0.63f, 0.32f, 0.18f, 1.00f); } }
        public static Colour Silver { get { return new Colour(0.75f, 0.75f, 0.75f, 1.00f); } }
        public static Colour SkyBlue { get { return new Colour(0.53f, 0.81f, 0.92f, 1.00f); } }
        public static Colour SlateBlue { get { return new Colour(0.42f, 0.35f, 0.80f, 1.00f); } }
        public static Colour SlateGray { get { return new Colour(0.44f, 0.50f, 0.56f, 1.00f); } }
        public static Colour Snow { get { return new Colour(1.00f, 0.98f, 0.98f, 1.00f); } }
        public static Colour SpringGreen { get { return new Colour(0.00f, 1.00f, 0.50f, 1.00f); } }
        public static Colour SteelBlue { get { return new Colour(0.27f, 0.51f, 0.71f, 1.00f); } }
        public static Colour Tan { get { return new Colour(0.82f, 0.71f, 0.55f, 1.00f); } }
        public static Colour Teal { get { return new Colour(0.00f, 0.50f, 0.50f, 1.00f); } }
        public static Colour Thistle { get { return new Colour(0.85f, 0.75f, 0.85f, 1.00f); } }
        public static Colour Tomato { get { return new Colour(1.00f, 0.39f, 0.28f, 1.00f); } }
        public static Colour Turquoise { get { return new Colour(0.25f, 0.88f, 0.82f, 1.00f); } }
        public static Colour Violet { get { return new Colour(0.93f, 0.51f, 0.93f, 1.00f); } }
        public static Colour Wheat { get { return new Colour(0.96f, 0.87f, 0.70f, 1.00f); } }
        public static Colour White { get { return new Colour(1.00f, 1.00f, 1.00f, 1.00f); } }
        public static Colour WhiteSmoke { get { return new Colour(0.96f, 0.96f, 0.96f, 1.00f); } }
        public static Colour Yellow { get { return new Colour(1.00f, 1.00f, 0.00f, 1.00f); } }
        public static Colour YellowGreen { get { return new Colour(0.60f, 0.80f, 0.20f, 1.00f); } }

        //CA1815 Recommendations - Only useful for structs that are compared or hashed
        //May remove as these are not common uses of this struct

        public override bool Equals(object obj)
        {
            if (!(obj is Colour))
            {
                return false;
            }

            var other = (Colour)obj;

            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public bool Equals(Colour other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static bool operator ==(Colour left, Colour right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Colour left, Colour right)
        {
            return !(left == right);
        }

        public override int GetHashCode() =>
                    HashCode.Combine(R, G, B, A); //Or (R, G, B, A).GetHashCode();     
    }
}