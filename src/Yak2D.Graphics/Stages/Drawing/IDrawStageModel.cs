namespace Yak2D.Graphics
{
    public interface IDrawStageModel : IRenderStageModel
    {
        IDrawStageBuffers Buffers { get; }
        IDrawStageBatcher Batcher { get; }
        BlendState BlendState { get; }

        void DrawToDynamicQueue(ref CoordinateSpace target,
                                ref FillType type,
                                ref Colour colour,
                                ref Vertex2D[] vertices,
                                ref int[] indices,
                                ref ulong texture0,
                                ref ulong texture1,
                                ref TextureCoordinateMode texMode0,
                                ref TextureCoordinateMode texMode1,
                                ref float depth,
                                ref int layer, 
                                bool valildate = false);
        void ClearDynamicDrawQueue();
        void Process();

        IPersistentDrawQueue AddPersistentQueue(InternalDrawRequest[] requests,
                                                bool validate = false);

        void RemovePersistentQueue(IPersistentDrawQueue queue);
        void RemovePersistentQueue(ulong queue);
    }
}