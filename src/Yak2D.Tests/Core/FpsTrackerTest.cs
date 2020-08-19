using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class FpsTrackerTest
    {
        [Fact]
        public void FpsTracker_CalculationTest_ConfirmCorrectFPSIsCalculated()
        {
            var timer = Substitute.For<ITimer>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            properties.Internal.Returns(new InternalStartUpProperties { DefaultFpsTrackerUpdatePeriodInSeconds = 1.0f });

            IFpsTracker tracker = new FpsTracker(timer, properties);

            for (var n = 0; n < 24; n++)
            {
                tracker.RegisterFrame();
            }

            timer.Seconds.Returns(1.0f);

            tracker.Update();

            Assert.Equal(24.0f, tracker.FPS);
        }
    }
}