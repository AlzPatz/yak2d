using Yak2D.Utility;

namespace Yak2D.Graphics
{
    public interface IComparerCollection
    {
        ArrayComparer<int> Integer { get; set; }
        ArrayComparer<float> ReverseFloat { get; set; }
        ArrayComparer<FillType> DrawType { get; set; }
        ArrayComparer<CoordinateSpace> DrawTarget { get; set; }
        ArrayComparer<TextureCoordinateMode> TextureCoordMode { get; set; }
        ArrayComparer<ulong> ULong { get; set; }
    }
}
