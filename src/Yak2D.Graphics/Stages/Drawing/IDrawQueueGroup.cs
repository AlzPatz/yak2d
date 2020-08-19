using System.Collections.Generic;

namespace Yak2D.Graphics
{
    public interface IDrawQueueGroup
    {
        IDrawStageBuffers Buffers { get; }
        QueueWrap DynamicQueue { get; }
        List<QueueWrap> PersistentQueues { get; }

        void ClearDynamicQueue();
        void ProcessDynamicQueue();

        QueueWrap CreateNewPersistentQueue(InternalDrawRequest[] requests, bool validate);
        void RemovePersistentQueue(ulong id);

        void ProcessPersistentQueue(ulong id);
    }
}