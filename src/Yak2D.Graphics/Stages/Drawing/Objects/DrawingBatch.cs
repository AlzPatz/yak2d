namespace Yak2D.Graphics
{
    public class DrawingBatch
    {
        public ulong Texture0 { get; set; }
        public ulong Texture1 { get; set; }
        public TextureCoordinateMode TextureMode0 { get; set; }
        public TextureCoordinateMode TextureMode1 { get; set; }
        public int StartIndex { get; set; }
        public int NumIndices { get; set; }
    }
}