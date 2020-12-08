using Veldrid;

namespace Yak2D.Internal
{
    public static class TextureSampleCountConverter
    {
        public static Veldrid.TextureSampleCount ConvertYakToVeldrid(Yak2D.Internal.TextureSampleCount sampleCount)
        {
            switch (sampleCount)
            {
                case Yak2D.Internal.TextureSampleCount.X1:
                    return Veldrid.TextureSampleCount.Count1;
                case Yak2D.Internal.TextureSampleCount.X2:
                    return Veldrid.TextureSampleCount.Count2;
                case Yak2D.Internal.TextureSampleCount.X4:
                    return Veldrid.TextureSampleCount.Count4;
                case Yak2D.Internal.TextureSampleCount.X8:
                    return Veldrid.TextureSampleCount.Count8;
                case Yak2D.Internal.TextureSampleCount.X16:
                    return Veldrid.TextureSampleCount.Count16;
                case Yak2D.Internal.TextureSampleCount.X32:
                    return Veldrid.TextureSampleCount.Count32;
            }

            //'Cannot' get here
            return Veldrid.TextureSampleCount.Count1;
        }

        public static Yak2D.Internal.TextureSampleCount ConvertVeldridToYak(Veldrid.TextureSampleCount sampleCount)
        {
            switch (sampleCount)
            {
                case Veldrid.TextureSampleCount.Count1:
                    return Yak2D.Internal.TextureSampleCount.X1;
                case Veldrid.TextureSampleCount.Count2:
                    return Yak2D.Internal.TextureSampleCount.X2;
                case Veldrid.TextureSampleCount.Count4:
                    return Yak2D.Internal.TextureSampleCount.X4;
                case Veldrid.TextureSampleCount.Count8:
                    return Yak2D.Internal.TextureSampleCount.X8;
                case Veldrid.TextureSampleCount.Count16:
                    return Yak2D.Internal.TextureSampleCount.X16;
                case Veldrid.TextureSampleCount.Count32:
                    return Yak2D.Internal.TextureSampleCount.X32;
            }

            //'Cannot' get here
            return Yak2D.Internal.TextureSampleCount.X1;
        }
    }
}