using Veldrid;

namespace Yak2D.Utility
{
    public static class TextureSampleCountConverter
    {
        public static TextureSampleCount ConvertYakToVeldrid(TexSampleCount sampleCount)
        {
            switch (sampleCount)
            {
                case TexSampleCount.X1:
                    return TextureSampleCount.Count1;
                case TexSampleCount.X2:
                    return TextureSampleCount.Count2;
                case TexSampleCount.X4:
                    return TextureSampleCount.Count4;
                case TexSampleCount.X8:
                    return TextureSampleCount.Count8;
                case TexSampleCount.X16:
                    return TextureSampleCount.Count16;
                case TexSampleCount.X32:
                    return TextureSampleCount.Count32;
            }

            //'Cannot' get here
            return TextureSampleCount.Count1;
        }
    }
}