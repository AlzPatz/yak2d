namespace Yak2D.Graphics
{
    public interface IQueueToBufferBlitter
    {
        void UnpackAndTransferToGpu(QueueWrap wrappedQueue, IDrawStageBuffers buffers);
    }
}
