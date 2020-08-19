namespace Yak2D.Graphics
{
    public class PersistentDrawQueueReference : IPersistentDrawQueue
    {
        public ulong Id { get; private set; }

        public PersistentDrawQueueReference(ulong id)
        {
            Id = id;
        }
    }
}