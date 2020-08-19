using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Internal;
using Yak2D.Tests.ManualFakes;

namespace Yak2D.Tests
{
    public class UpdatePeriodTest
    {
        [Fact]
        public void UpdatePeriod_Fixed_EnsureCorrectNumberOfUpdateCalls()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var analytics = Substitute.For<IDebugAnalytics>();
            var monitor = Substitute.For<IFramesPerSecondMonitor>();
            ITimer timer = new StopWatchTimerFactory().Create();

            IUpdatePeriod fixedupdate = new UpdatePeriodFixed(messenger, analytics);

            var count = 0;

            fixedupdate.ProcessRequiredUpdates(1.0,
                                               new LoopProperties { FixedUpdateTimeStepInSeconds = 0.01f, Running = true },
                                               monitor,
                                               (t) => { count++; return true; },
                                               timer);

            Assert.Equal(100, count);
        }

        [Fact]
        public void UpdatePeriod_Variable_EnsureCorrectNumberOfUpdateCalls()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var analytics = Substitute.For<IDebugAnalytics>();
            var monitor = Substitute.For<IFramesPerSecondMonitor>();
            ITimer timer = new StopWatchTimerFactory().Create();

            IUpdatePeriod variableupdate = new UpdatePeriodVariable(messenger, analytics);

            var count = 0;

            variableupdate.ProcessRequiredUpdates(1.0,
                                               new LoopProperties { FixedUpdateTimeStepInSeconds = 0.01f, Running = true },
                                               monitor,
                                               (t) => { count++; return true; },
                                               timer);

            Assert.Equal(1, count);
        }

        //TODO: Get UpdatePeriodFixedAdaptive under test, perhaps will require some refactoring to enable easier testing of update period changes
    }
}