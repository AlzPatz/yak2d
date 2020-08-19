using Yak2D.Utility;

namespace Yak2D.Graphics
{
    public class ComparerCollection : IComparerCollection
    {
        public ArrayComparer<int> Integer { get; set; }
        public ArrayComparer<float> ReverseFloat { get; set; }
        public ArrayComparer<FillType> DrawType { get; set; }
        public ArrayComparer<CoordinateSpace> DrawTarget { get; set; }
        public ArrayComparer<TextureCoordinateMode> TextureCoordMode { get; set; }
        public ArrayComparer<ulong> ULong { get; set; }

        public ComparerCollection()
        {
            Integer = new ArrayComparer<int>(false);
            ReverseFloat = new ArrayComparer<float>(true);
            DrawType = new ArrayComparer<FillType>(false);
            DrawTarget = new ArrayComparer<CoordinateSpace>(false);
            TextureCoordMode = new ArrayComparer<TextureCoordinateMode>(false);
            ULong = new ArrayComparer<ulong>(false);
        }
    }
}