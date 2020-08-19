using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class CoreMessagerTest
    {
        [Fact]
        public void CoreMessenger_QueueMessenger_EnsureMessageTrackingWorksOnSingleAdd()
        {
            var debug = Substitute.For<IFrameworkMessenger>();
            var init = Substitute.For<IResourceReinitialiser>();
            var components = Substitute.For<ISystemComponents>();

            ICoreMessenger messenger = new CoreMessenger(debug, components, init);

            Assert.False(messenger.AreThereMessagesInQueue());

            messenger.QueueMessage(CoreMessage.DeviceOrRenderTargetsReset);

            Assert.True(messenger.AreThereMessagesInQueue());
        }

        [Fact]
        public void CoreMessenger_QueueMessenger_ValidateQueueEmptyingAndProcessingTriggers()
        {
            var debug = Substitute.For<IFrameworkMessenger>();
            var init = Substitute.For<IResourceReinitialiser>();
            var components = Substitute.For<ISystemComponents>();

            ICoreMessenger messenger = new CoreMessenger(debug, components, init);

            Assert.False(messenger.AreThereMessagesInQueue());

            messenger.QueueMessage(CoreMessage.DeviceOrRenderTargetsReset);
            messenger.QueueMessage(CoreMessage.DeviceOrRenderTargetsReset);
            messenger.QueueMessage(CoreMessage.DeviceOrRenderTargetsReset);
            messenger.QueueMessage(CoreMessage.DeviceOrRenderTargetsReset);
            messenger.QueueMessage(CoreMessage.DeviceOrRenderTargetsReset);

            Assert.True(messenger.AreThereMessagesInQueue());

            messenger.ProcessMessageQueue(new LoopProperties());

            Assert.False(messenger.AreThereMessagesInQueue());
        }
    }
}