namespace Yak2D.Internal
{
    public interface IApplicationMessenger
    {
        void QueueMessage(FrameworkMessage msg);
        bool AreThereMessagesInQueue();
        void ProcessMessageQueue(IServices services);
    }
}
