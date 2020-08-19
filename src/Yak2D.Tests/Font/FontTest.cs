using System.Collections.Generic;
using Xunit;
using Yak2D.Font;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class FontTest
    {
        [Fact]
        public void FontModel_SubFontAtSize_ReturnCorrectSubfont()
        {
            var subFonts = new List<SubFont>
            {
                new SubFont(12.0f, 0.0f, null, null, false, null),
                new SubFont(18.0f, 0.0f, null, null, false, null),
                new SubFont(24.0f, 0.0f, null, null, false, null),
                new SubFont(32.0f, 0.0f, null, null, false, null),
                new SubFont(48.0f, 0.0f, null, null, false, null),
            };

            var font = new FontModel(subFonts);

            Assert.Equal(32.0f, font.SubFontAtSize(30.0f).Size);
            Assert.Equal(18.0f, font.SubFontAtSize(13.0f).Size);
            Assert.Equal(12.0f, font.SubFontAtSize(6.0f).Size);
            Assert.Equal(12.0f, font.SubFontAtSize(-5.0f).Size);
            Assert.Equal(48.0f, font.SubFontAtSize(128.0f).Size);
        }

        [Fact]
        public void FontModel_DealsWithNullOrZeroSizedSubFontArray_ThrowsException()
        {
            Assert.Throws<Yak2DException>(() => { var obj = new FontModel(null); });
            Assert.Throws<Yak2DException>(() => { var obj = new FontModel(new List<SubFont>()); });
        }
    }
}