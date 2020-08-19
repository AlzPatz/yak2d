namespace Yak2D.Graphics
{
    public struct DrawRequestWrap
    {
        public ulong DrawStageId { get; set; }
        public InternalDrawRequest Request { get; set; }
    }
}