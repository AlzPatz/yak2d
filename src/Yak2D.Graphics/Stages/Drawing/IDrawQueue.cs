namespace Yak2D.Graphics
{
    public interface IDrawQueue
    {
        QueueData Data { get; }
        
        void Clear();
        void Sort();
        bool AddIfValid(ref CoordinateSpace target,
                        ref FillType type,
                        ref Colour triangleColour,
                        ref Vertex2D[] vertices,
                        ref int[] indices,
                        ref ulong textureIndex0,
                        ref ulong textureIndex1,
                        ref TextureCoordinateMode texmode0,
                        ref TextureCoordinateMode texmode1,
                        ref float depth,
                        ref int layer);
        void Add(ref CoordinateSpace target,
                        ref FillType type,
                        ref Colour triangleColour,
                        ref Vertex2D[] vertices,
                        ref int[] indices,
                        ref ulong textureIndex0,
                        ref ulong textureIndex1,
                        ref TextureCoordinateMode texmode0,
                        ref TextureCoordinateMode texmode1,
                        ref float depth,
                        ref int layer);
    }
}