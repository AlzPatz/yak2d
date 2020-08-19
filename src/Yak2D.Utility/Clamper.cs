namespace Yak2D.Utility
{
    public static class Clamper
    {
        public static float Clamp(float value, float min, float max)
        {
            var clamped = value;
            if (clamped < min)
            {
                clamped = min;
            }
            if (clamped > max)
            {
                clamped = max;
            }
            return clamped;
        }

        public static int Clamp(int value, int min, int max)
        {
            var clamped = value;
            if (clamped < min)
            {
                clamped = min;
            }
            if (clamped > max)
            {
                clamped = max;
            }
            return clamped;
        }
    }
}