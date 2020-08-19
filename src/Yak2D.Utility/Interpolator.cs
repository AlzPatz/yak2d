using System.Numerics;

namespace Yak2D.Utility
{
    public static class Interpolator
    {
        public static float Interpolate(float initial, float final, ref float fraction)
        {
            return Interpolate(ref initial, ref final, ref fraction);
        }

        public static float Interpolate(ref float initial, ref float final, ref float fraction)
        {
            return initial + (fraction * (final - initial));
        }

        public static Vector2 Interpolate(Vector2 initial, Vector2 final, ref float fraction)
        {
            return Interpolate(ref initial, ref final, ref fraction);
        }

        public static Vector2 Interpolate(ref Vector2 initial, ref Vector2 final, ref float fraction)
        {
            return initial + (fraction * (final - initial));
        }

        public static Vector3 Interpolate(Vector3 initial, Vector3 final, ref float fraction)
        {
            return Interpolate(ref initial, ref final, ref fraction);
        }

        public static Vector3 Interpolate(ref Vector3 initial, ref Vector3 final, ref float fraction)
        {
            return initial + (fraction * (final - initial));
        }

        public static Vector4 Interpolate(Vector4 initial, Vector4 final, ref float fraction)
        {
            return Interpolate(ref initial, ref final, ref fraction);
        }

        public static Vector4 Interpolate(ref Vector4 initial, ref Vector4 final, ref float fraction)
        {
            return initial + (fraction * (final - initial));
        }

        public static int Interpolate(int initial, int final, ref float fraction)
        {
            return Interpolate(ref initial, ref final, ref fraction);
        }

        public static int Interpolate(ref int initial, ref int final, ref float fraction)
        {
            return (int)(initial + (fraction * (final - initial)));
        }

        public static Colour Interpolate(Colour initial, Colour final, ref float fraction)
        {
            return new Colour(
                Interpolate(initial.R, final.R, ref fraction),
                Interpolate(initial.G, final.G, ref fraction),
                Interpolate(initial.B, final.B, ref fraction),
                Interpolate(initial.A, final.A, ref fraction)
            );
        }
    }
}