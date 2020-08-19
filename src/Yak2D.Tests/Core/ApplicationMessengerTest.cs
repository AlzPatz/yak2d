using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class ApplicationMessengerTest
    {
        [Fact]
        public void ApplicationMessenger_QueueMessenger_EnsureMessageTrackingWorksOnSingleAdd()
        {
            var debug = Substitute.For<IFrameworkMessenger>();
            var application = Substitute.For<IApplication>();

            IApplicationMessenger messenger = new ApplicationMessenger(debug, application);

            Assert.False(messenger.AreThereMessagesInQueue());

            messenger.QueueMessage(FrameworkMessage.GamepadAdded);

            Assert.True(messenger.AreThereMessagesInQueue());
        }

        [Fact]
        public void ApplicationMessenger_QueueMessenger_ValidateQueueEmptyingAndProcessingTriggers()
        {
            var debug = Substitute.For<IFrameworkMessenger>();
            var application = Substitute.For<IApplication>();
            var services = Substitute.For<IServices>();

            IApplicationMessenger messenger = new ApplicationMessenger(debug, application);

            Assert.False(messenger.AreThereMessagesInQueue());

            messenger.QueueMessage(FrameworkMessage.GamepadAdded);
            messenger.QueueMessage(FrameworkMessage.GamepadAdded);
            messenger.QueueMessage(FrameworkMessage.GamepadAdded);
            messenger.QueueMessage(FrameworkMessage.GamepadAdded);

            Assert.True(messenger.AreThereMessagesInQueue());

            messenger.ProcessMessageQueue(services);

            Assert.False(messenger.AreThereMessagesInQueue());

            application.Received(4).ProcessMessage(Arg.Any<FrameworkMessage>(), Arg.Any<IServices>());
        }
    }
}