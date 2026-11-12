using Xunit;
using Yak2D.Surface;

namespace Yak2D.Tests
{
    public class PixelRowMirrorTest
    {
        //2 wide, 3 high: rows {0,1} {2,3} {4,5}
        private static int[] TwoByThree() => new int[] { 0, 1, 2, 3, 4, 5 };

        [Theory]
        [InlineData(GraphicsApi.OpenGL)]
        [InlineData(GraphicsApi.OpenGLES)]
        public void PixelRowMirror_OpenGL_MirrorsRowsWithoutMutatingInput(GraphicsApi backend)
        {
            var data = TwoByThree();

            var result = PixelRowMirror.MirrorOverXAxisIfGraphicsApiRequires(backend, 2, 3, data);

            Assert.True(PixelRowMirror.IsRequired(backend));
            Assert.Equal(new int[] { 4, 5, 2, 3, 0, 1 }, result);
            Assert.NotSame(data, result);
            Assert.Equal(TwoByThree(), data);
        }

        [Theory]
        [InlineData(GraphicsApi.Vulkan)]
        [InlineData(GraphicsApi.Direct3D11)]
        [InlineData(GraphicsApi.Metal)]
        public void PixelRowMirror_OtherBackends_ReturnSameArrayUnchanged(GraphicsApi backend)
        {
            var data = TwoByThree();

            var result = PixelRowMirror.MirrorOverXAxisIfGraphicsApiRequires(backend, 2, 3, data);

            Assert.False(PixelRowMirror.IsRequired(backend));
            Assert.Same(data, result);
            Assert.Equal(TwoByThree(), data);
        }
    }
}
