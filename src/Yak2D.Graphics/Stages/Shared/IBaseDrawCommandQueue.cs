namespace Yak2D.Graphics
{
    public interface IBaseDrawCommandQueue
    {
        void Flush();
        void Add(ulong drawStageId, ref InternalDrawRequest request);
    }
}
