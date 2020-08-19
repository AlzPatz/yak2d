namespace Yak2D.Graphics
{
    public class PersistentDistortionQueueReference : IPersistentDistortionQueue
    {
        public ulong Id { get; private set; }

        public PersistentDistortionQueueReference(ulong id)
        {
            Id = id;
        }
    }
}