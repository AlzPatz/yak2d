using NSubstitute;
using Veldrid;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class RenderCommandQueueTest
    {
        [Fact]
        public void RenderCommandQueue_ValidateIteration_CorrectOrderReturned()
        {
            var properties = Substitute.For<IStartupPropertiesCache>();
            var internalProps = Substitute.For<InternalStartUpProperties>();
            internalProps.RenderCommandMinPoolSize = 4;

            properties.Internal.Returns(internalProps);
            IRenderCommandQueue queue = new RenderCommandQueue(properties);

            var vals = new ulong[] { 4UL, 3UL, 2UL, 1UL, 0UL };

            for (var n = 0; n < 5; n++)
            {
                var i = vals[n];
                queue.Add(RenderCommandType.CopyStage, i, i, i, i, i, i, i, RgbaFloat.White);
            }

            var index = 4;
            foreach (var item in queue.FlushCommands())
            {
                Assert.Equal(RenderCommandType.CopyStage, item.Type);
                Assert.Equal((ulong)index, item.Stage);
                Assert.Equal((ulong)index, item.Surface);
                Assert.Equal((ulong)index, item.Camera);
                Assert.Equal((ulong)index, item.Texture0);
                Assert.Equal((ulong)index, item.Texture1);
                Assert.Equal((ulong)index, item.SpareId0);
                Assert.Equal((ulong)index, item.SpareId0);
                Assert.Equal(RgbaFloat.White, item.Colour);

                index--;
            }
        }

        [Fact]
        public void RenderCommandQueue_ValidateDoubleInSize_MaintainsContents()
        {
            var properties = Substitute.For<IStartupPropertiesCache>();
            var internalProps = Substitute.For<InternalStartUpProperties>();
            internalProps.RenderCommandMinPoolSize = 3; //Must increase in size as this is too small to hold all data

            properties.Internal.Returns(internalProps);
            IRenderCommandQueue queue = new RenderCommandQueue(properties);

            var vals = new ulong[] { 4UL, 3UL, 2UL, 1UL, 0UL };

            for (var n = 0; n < 5; n++)
            {
                var i = vals[n];
                queue.Add(RenderCommandType.CopyStage, i, i, i, i, i, i, i, RgbaFloat.White);
            }

            var index = 4;
            foreach (var item in queue.FlushCommands())
            {
                Assert.Equal(RenderCommandType.CopyStage, item.Type);
                Assert.Equal((ulong)index, item.Stage);
                Assert.Equal((ulong)index, item.Surface);
                Assert.Equal((ulong)index, item.Camera);
                Assert.Equal((ulong)index, item.Texture0);
                Assert.Equal((ulong)index, item.Texture1);
                Assert.Equal((ulong)index, item.SpareId0);
                Assert.Equal((ulong)index, item.SpareId0);
                Assert.Equal(RgbaFloat.White, item.Colour);

                index--;
            }
        }
    }
}