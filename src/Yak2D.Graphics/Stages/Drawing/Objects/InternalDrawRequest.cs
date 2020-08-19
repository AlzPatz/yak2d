namespace Yak2D.Graphics
{
    public struct InternalDrawRequest
    {
        public CoordinateSpace CoordinateSpace;
        public FillType FillType;
        public Vertex2D[] Vertices;
        public int[] Indices;
        public Colour Colour;
        public ulong Texture0;
        public ulong Texture1;
        public TextureCoordinateMode TextureMode0;
        public TextureCoordinateMode TextureMode1;
        public float Depth;
        public int Layer;
    }
}