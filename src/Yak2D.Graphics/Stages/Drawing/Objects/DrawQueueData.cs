namespace Yak2D.Graphics
{
    public class QueueData
    {
        public int QueueSizeSingleProperty { get; set; }
        public int QueueSizeVertex { get; set; }
        public int QueueSizeIndex { get; set; }
        public int[] Ordering { get; set; }
        public CoordinateSpace[] Targets { get; set; }
        public FillType[] Types { get; set; }
        public Colour[] BaseColours { get; set; }
        public int[] NumVertices { get; set; }
        public int[] FirstVertexPosition { get; set; }
        public Vertex2D[] Vertices { get; set; }
        public int[] NumIndices { get; set; }
        public int[] FirstIndexPosition { get; set; }
        public int[] Indices { get; set; }
        public ulong[] Texture0 { get; set; }
        public ulong[] Texture1 { get; set; }
        public TextureCoordinateMode[] TextureMode0 { get; set; }
        public TextureCoordinateMode[] TextureMode1 { get; set; }
        public float[] Depths { get; set; }
        public int[] Layers { get; set; }
        public int NumRequests { get; set; }
        public int NumVerticesUsed { get; set; }
        public int NumIndicesUsed { get; set; }
        public int NumberOfTrianglesInQueue { get { return NumIndicesUsed / 3; } }
    }
}