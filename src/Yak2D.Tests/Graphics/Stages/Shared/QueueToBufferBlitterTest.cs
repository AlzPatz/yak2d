using System.Numerics;
using NSubstitute;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class QueueToBufferBlitterTest
    {
        [Fact]
        public void QueueToBufferBlitter_TestUnpackAndTransfer_CorrectBufferCallsAndDataTransfered()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            IQueueToBufferBlitter blitter = new QueueToBufferBlitter();

            var q = new DrawQueue(messenger, new ComparerCollection(), 36, 8, false);

            Add(q, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            Add(q, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            Add(q, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            Add(q, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            Add(q, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            Add(q, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            Add(q, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);

            q.Sort();

            var wrap = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 16, BufferPositionOfFirstVertex = 32, Queue = q };

            var buffersMock = Substitute.For<IDrawStageBuffers>();

            blitter.UnpackAndTransferToGpu(wrap, buffersMock);

            buffersMock.Received(1).EnsureVertexBufferIsLargeEnough(Arg.Any<int>(), Arg.Any<bool>());
            buffersMock.Received(1).EnsureIndexBufferIsLargeEnough(Arg.Any<int>(), Arg.Any<bool>());

            buffersMock.ReceivedWithAnyArgs(21).CopyAVertexToStagingArray(default, default, default, default, default, default, default, default, default, default, default);
            buffersMock.ReceivedWithAnyArgs(21).CopyAnIndexToStagingArray(default, default, default);

            buffersMock.Received(1).CopyIndexBufferSegmentToGpu(16, 21);
            buffersMock.Received(1).CopyVertexBufferSegmentToGpu(32, 21);
        }

        //Helper adds request to queue
        private void Add(IDrawQueue queue,
                       FillType fill,
                       ulong texture0,
                       ulong texture1,
                       TextureCoordinateMode tmode0,
                       TextureCoordinateMode tmode1,
                       int layer,
                       float depth = 1.0f)
        {
            //Unsorted properties

            var verts = new Vertex2D[]
            {
                new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 1.0f},
                new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 1.0f},
                new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 1.0f},
            };

            var indices = new int[] { 0, 1, 2 };
            var target = CoordinateSpace.Screen;
            var colour = Colour.White;

            queue.Add(ref target,
                      ref fill,
                      ref colour,
                      ref verts,
                      ref indices,
                      ref texture0,
                      ref texture1,
                      ref tmode0,
                      ref tmode1,
                      ref depth,
                      ref layer);
        }
    }
}