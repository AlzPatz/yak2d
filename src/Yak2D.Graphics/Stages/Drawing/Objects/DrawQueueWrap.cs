namespace Yak2D.Graphics
{
    public class QueueWrap
    {
        public ulong Id { get; set; }
        public IDrawQueue Queue { get; set; }
        public int BufferPositionOfFirstVertex { get; set; }
        public int BufferPositionOfFirstIndex { get; set; }
    }
}