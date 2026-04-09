using NeoVeldrid;

namespace Yak2D.Internal
{
    public static class TextureSampleCountConverter
    {
        public static NeoVeldrid.TextureSampleCount ConvertYakToVeldrid(Yak2D.Internal.TextureSampleCount sampleCount)
        {
            switch (sampleCount)
            {
                case Yak2D.Internal.TextureSampleCount.X1:
                    return NeoVeldrid.TextureSampleCount.Count1;
                case Yak2D.Internal.TextureSampleCount.X2:
                    return NeoVeldrid.TextureSampleCount.Count2;
                case Yak2D.Internal.TextureSampleCount.X4:
                    return NeoVeldrid.TextureSampleCount.Count4;
                case Yak2D.Internal.TextureSampleCount.X8:
                    return NeoVeldrid.TextureSampleCount.Count8;
                case Yak2D.Internal.TextureSampleCount.X16:
                    return NeoVeldrid.TextureSampleCount.Count16;
                case Yak2D.Internal.TextureSampleCount.X32:
                    return NeoVeldrid.TextureSampleCount.Count32;
            }

            //'Cannot' get here
            return NeoVeldrid.TextureSampleCount.Count1;
        }

        public static Yak2D.Internal.TextureSampleCount ConvertVeldridToYak(NeoVeldrid.TextureSampleCount sampleCount)
        {
            switch (sampleCount)
            {
                case NeoVeldrid.TextureSampleCount.Count1:
                    return Yak2D.Internal.TextureSampleCount.X1;
                case NeoVeldrid.TextureSampleCount.Count2:
                    return Yak2D.Internal.TextureSampleCount.X2;
                case NeoVeldrid.TextureSampleCount.Count4:
                    return Yak2D.Internal.TextureSampleCount.X4;
                case NeoVeldrid.TextureSampleCount.Count8:
                    return Yak2D.Internal.TextureSampleCount.X8;
                case NeoVeldrid.TextureSampleCount.Count16:
                    return Yak2D.Internal.TextureSampleCount.X16;
                case NeoVeldrid.TextureSampleCount.Count32:
                    return Yak2D.Internal.TextureSampleCount.X32;
            }

            //'Cannot' get here
            return Yak2D.Internal.TextureSampleCount.X1;
        }
    }
}