namespace Yak2D.Internal
{
    public interface ICoreMessenger
    {
        void QueueMessage(CoreMessage msg);
        bool AreThereMessagesInQueue();
        void ProcessMessageQueue(LoopProperties loopProperties);
    }
}