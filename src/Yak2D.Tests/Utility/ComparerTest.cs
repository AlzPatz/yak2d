using Xunit;
using Yak2D.Utility;

namespace Yak2D.Tests
{
    public class ComparerTest
    {
        [Fact]
        public void IntegerComparer_NotReversed_ExpectResultLessThanZero()
        {
            var comparer = new ArrayComparer<int>(false);
            comparer.SetItems(new int[] { 0, 1 });

            var result = comparer.Compare(0, 1);

            Assert.True(result < 0);
        }

        [Fact]
        public void IntegerComparer_Reversed_ExpectResultGreaterThanZero()
        {
            var comparer = new ArrayComparer<int>(true);
            comparer.SetItems(new int[] { 0, 1 });

            var result = comparer.Compare(0, 1);

            Assert.True(result > 0);
        }

        [Fact]
        public void IntegerComparer_ComparingEqualValues_ExpectResultEqualToZero()
        {
            var comparer = new ArrayComparer<int>(false);
            comparer.SetItems(new int[] { 2, 2 });

            var result = comparer.Compare(0, 1);

            Assert.True(result == 0);
        }

        [Fact]
        public void Comparer_PassingNullItemList_ThrowsException()
        {
            var comparer = new ArrayComparer<int>(false);

            Assert.Throws<Yak2DException>(() => { comparer.SetItems(null); });
        }
    }
}