using System;
using System.Numerics;

namespace Yak2D.Utility
{
    public static class Geometry
    {
        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180.0f;
        }

        public static Vector2 RotateVectorClockwise(Vector2 v, float radians)
        {
            return Vector2.Transform(v, Matrix3x2.CreateRotation(-radians));
        }
    }
}