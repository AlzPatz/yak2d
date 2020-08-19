using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class CoreMessenger : ICoreMessenger
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IResourceReinitialiser _systemResourcesReinitialiser;

        private const int MESSAGE_QUEUE_INITIAL_SIZE = 64;

        private Queue<CoreMessage> _messageQueue;

        public CoreMessenger(IFrameworkMessenger frameworkMessenger,
                                ISystemComponents systemComponents,
                                  IResourceReinitialiser systemResourcesReinitialiser)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _systemResourcesReinitialiser = systemResourcesReinitialiser;

            _messageQueue = new Queue<CoreMessage>(MESSAGE_QUEUE_INITIAL_SIZE);
        }

        public bool AreThereMessagesInQueue()
        {
            return _messageQueue.Count > 0;
        }

        public void ProcessMessageQueue(LoopProperties loopProperties)
        {
            while (_messageQueue.Count > 0)
            {
                var msg = _messageQueue.Dequeue();

                switch (msg)
                {
                    case CoreMessage.Shutdown:
                        loopProperties.Running = false;
                        break;
                    case CoreMessage.DeviceOrRenderTargetsReset:
                        _systemComponents.RecreateDeviceAndReinitialiseAllResources(_systemResourcesReinitialiser.ReInitialise);
                        break;
                }
            }
        }

        public void QueueMessage(CoreMessage msg)
        {
            _frameworkMessenger.Report(string.Concat("Core Message:", msg.ToString()));
            _messageQueue.Enqueue(msg);
        }
    }
}