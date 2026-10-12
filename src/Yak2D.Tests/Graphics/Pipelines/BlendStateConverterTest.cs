using NeoVeldrid;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class BlendStateConverterTest
    {
        [Fact]
        public void BlendStateConverter_Alpha_KeepsCoverageInAlphaChannel()
        {
            var attachment = new BlendStateConverter().Convert(BlendState.Alpha).AttachmentStates[0];

            Assert.Equal(BlendFactor.SourceAlpha, attachment.SourceColorFactor);
            Assert.Equal(BlendFactor.InverseSourceAlpha, attachment.DestinationColorFactor);
            Assert.Equal(BlendFactor.One, attachment.SourceAlphaFactor);
            Assert.Equal(BlendFactor.InverseSourceAlpha, attachment.DestinationAlphaFactor);
        }

        [Fact]
        public void BlendStateConverter_Premultiplied_DoesNotMultiplyColourBySourceAlpha()
        {
            var attachment = new BlendStateConverter().Convert(BlendState.Premultiplied).AttachmentStates[0];

            Assert.Equal(BlendFactor.One, attachment.SourceColorFactor);
            Assert.Equal(BlendFactor.InverseSourceAlpha, attachment.DestinationColorFactor);
            Assert.Equal(BlendFactor.One, attachment.SourceAlphaFactor);
            Assert.Equal(BlendFactor.InverseSourceAlpha, attachment.DestinationAlphaFactor);
        }
    }
}
