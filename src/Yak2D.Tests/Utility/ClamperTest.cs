using Xunit;
using Yak2D.Utility;

namespace Yak2D.Tests
{
    //Added for completeness, but essentially just testing c#...

    public class ClamperTest
    {
        [Fact]
        public void Clamper_TestFloatTooSmall_ReturnsMin()
        {
            var val = 1.0f;
            var min = 2.0f;
            var max = 3.0f;

            var result = Clamper.Clamp(val, min, max);

            Assert.Equal(min, result);
        }

        [Fact]
        public void Clamper_TestFloatTooLarge_ReturnsMax()
        {
            var val = 4.0f;
            var min = 2.0f;
            var max = 3.0f;

            var result = Clamper.Clamp(val, min, max);

            Assert.Equal(max, result);
        }

        [Fact]
        public void Clamper_TestIntegerTooSmall_ReturnsMin()
        {
            var val = 1;
            var min = 2;
            var max = 3;

            var result = Clamper.Clamp(val, min, max);

            Assert.Equal(min, result);
        }

        [Fact]
        public void Clamper_TestIntegerTooLarge_ReturnsMax()
        {
            var val = 4;
            var min = 2;
            var max = 3;

            var result = Clamper.Clamp(val, min, max);

            Assert.Equal(max, result);
        }
    }
}
