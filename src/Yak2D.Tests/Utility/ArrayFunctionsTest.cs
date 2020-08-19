using Xunit;
using Yak2D.Utility;

namespace Yak2D.Tests
{
    public class ArrayFunctionsTest
    {
        [Fact]
        public void DoublesArraySize()
        {
            var array = new float[4];

            var doubled = ArrayFunctions.DoubleArraySizeAndKeepContents<float>(array);

            Assert.Equal(8, doubled.Length);
        }

        [Fact]
        public void DoubledArrayKeepsContent()
        {
            var array = new float[] { 3.0f, 2.0f, 1.0f, 5.0f };

            var doubled = ArrayFunctions.DoubleArraySizeAndKeepContents<float>(array);

            for (var n = 0; n < 4; n++)
            {
                Assert.Equal(array[n], doubled[n]);
            }
        }

        [Fact]
        public void DoubledArrayDoesNotContainDataInSecondHalf()
        {
            var array = new object[] { new object(), new object(), new object(), new object() };

            var doubled = ArrayFunctions.DoubleArraySizeAndKeepContents<object>(array);

            for (var n = 4; n < 8; n++)
            {
                Assert.Null(doubled[n]);
            }
        }

        [Fact]
        public void DoubledArrayReturnsNullIfNullArrayProvided()
        {
            int[] array = null;

            var doubled = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(array);

            Assert.Null(doubled);
        }
    }
}