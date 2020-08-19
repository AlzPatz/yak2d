using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class ApplicationMessenger : IApplicationMessenger
    {
        private const int MESSAGE_QUEUE_INITIAL_SIZE = 64;

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IApplication _application;

        private Queue<FrameworkMessage> _messageQueue;

        public ApplicationMessenger(IFrameworkMessenger frameworkMessenger,
                                    IApplication application)
        {
            _frameworkMessenger = frameworkMessenger;
            _application = application;

            _messageQueue = new Queue<FrameworkMessage>(MESSAGE_QUEUE_INITIAL_SIZE);
        }

        public void QueueMessage(FrameworkMessage msg)
        {
            _frameworkMessenger.Report(string.Concat("Framework Message:", msg.ToString()));
            _messageQueue.Enqueue(msg);
        }

        public bool AreThereMessagesInQueue()
        {
            return _messageQueue.Count > 0;
        }

        public void ProcessMessageQueue(IServices services)
        {
            while (_messageQueue.Count > 0)
            {
                var msg = _messageQueue.Dequeue();
                _application.ProcessMessage(msg, services);
            }
        }
    }
}