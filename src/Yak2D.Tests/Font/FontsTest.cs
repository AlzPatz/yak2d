using Xunit;
using NSubstitute;
using Yak2D.Internal;
using Yak2D.Font;

namespace Yak2D.Tests
{
    public class FontsTest
    {
        [Fact]
        public void Fonts_LoadFontCaptureNullOrEmptyPath_ThrowsException()
        {
            var fontmanager = Substitute.For<IFontManager>();

            IFonts fonts = new Fonts(fontmanager);

            Assert.Throws<Yak2DException>(() => { fonts.LoadFont(null, AssetSourceEnum.Embedded); });
            Assert.Throws<Yak2DException>(() => { fonts.LoadFont("", AssetSourceEnum.Embedded); });
            Assert.Throws<Yak2DException>(() => { fonts.LoadFont("  ", AssetSourceEnum.Embedded); });
        }
    }
}