using NSubstitute;
using System.Collections.Generic;
using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class DrawBatcherToolsTest
    {
        [Fact]
        public void DrawBatcherTools_CreatePoolSize_ExistingDataPersists()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var pool = new DrawingBatch[]
            {
                new DrawingBatch
                {
                     NumIndices = 5,
                     StartIndex = 7,
                     Texture0 = 0UL,
                     Texture1 = 8UL,
                     TextureMode0 = TextureCoordinateMode.None,
                     TextureMode1 = TextureCoordinateMode.Wrap
                },
                new DrawingBatch
                {
                     NumIndices = 9,
                     StartIndex = 3,
                     Texture0 = 4UL,
                     Texture1 = 9UL,
                     TextureMode0 = TextureCoordinateMode.Mirror,
                     TextureMode1 = TextureCoordinateMode.Wrap
                },
                new DrawingBatch
                {
                     NumIndices = 9,
                     StartIndex = 2,
                     Texture0 = 2UL,
                     Texture1 = 12UL,
                     TextureMode0 = TextureCoordinateMode.Wrap,
                     TextureMode1 = TextureCoordinateMode.None
                }
            };

            var result = tools.CreatePoolOfSize(6, true, pool);

            Assert.Equal(8UL, result[0].Texture1);
            Assert.Equal(TextureCoordinateMode.Mirror, result[1].TextureMode0);
            Assert.Equal(2, result[2].StartIndex);
        }

        [Fact]
        public void DrawBatcherTools_CombineList_DoesSoInCorrectOrder()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var messenger = Substitute.For<IFrameworkMessenger>();

            var q0 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q0, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            var w0 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q0 };

            var q1 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q1, FillType.Coloured, 1UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            var w1 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q1 };

            var q2 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q2, FillType.Coloured, 2UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 0);
            var w2 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q2 };

            var others = new List<QueueWrap> { w1, w2 };

            var combined = tools.CombinedList(w0, others);

            Assert.Equal(2UL, combined[2].Queue.Data.Texture0[0]);
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

        [Fact]
        public void DrawBatcherTools_AreAlLQueuesInactive_True()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var num = 3;

            var active = new bool[] { false, false, false };

            Assert.True(tools.AreAllQueuesInactive(ref num, active));
        }

        [Fact]
        public void DrawBatcherTools_AreAlLQueuesInactive_False()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var num = 3;

            var active = new bool[] { false, true, false };

            Assert.False(tools.AreAllQueuesInactive(ref num, active));
        }

        [Fact]
        public void DrawBatcherTools_FindLowestLayer_SimpleTest()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var messenger = Substitute.For<IFrameworkMessenger>();

            var q0 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q0, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            var w0 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q0 };

            var q1 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q1, FillType.Coloured, 1UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 2);
            var w1 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q1 };

            var q2 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q2, FillType.Coloured, 2UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10);
            var w2 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q2 };

            var others = new List<QueueWrap> { w1, w2 };
            var combined = tools.CombinedList(w0, others).ToArray();

            var num = 3;

            Assert.Equal(3, tools.FindLowestLayer(ref num, combined, new bool[] { true, false, true }, new int[] { 0, 0, 0 }));
        }

        [Fact]
        public void DrawBatcherTools_IdentifyActiveInLayer_SimpleTest()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var messenger = Substitute.For<IFrameworkMessenger>();

            var q0 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q0, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            var w0 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q0 };

            var q1 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q1, FillType.Coloured, 1UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10);
            var w1 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q1 };

            var q2 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q2, FillType.Coloured, 2UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10);
            var w2 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q2 };

            var others = new List<QueueWrap> { w1, w2 };
            var combined = tools.CombinedList(w0, others).ToArray();

            var num = 3;

            var activeInLayer = new bool[3];

            tools.IdentifyWhichQueuesAreActiveInTheCurrentLayer(ref num, combined, new int[] { 0, 0, 0 }, 10, activeInLayer, new bool[] { true, false, true });

            Assert.False(activeInLayer[0]);
            Assert.False(activeInLayer[1]);
            Assert.True(activeInLayer[2]);
        }

        [Fact]
        public void DrawBatcherTools_FindDeepestAtLowestLayer_SimpleTest()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var messenger = Substitute.For<IFrameworkMessenger>();

            var q0 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q0, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            var w0 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q0 };

            var q1 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q1, FillType.Coloured, 1UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10, 1.0f);
            var w1 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q1 };

            var q2 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q2, FillType.Coloured, 2UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10, 0.5f);
            var w2 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q2 };

            var others = new List<QueueWrap> { w1, w2 };
            var combined = tools.CombinedList(w0, others).ToArray();

            var num = 3;

            var index = tools.FindDeepestQueueAtLowestLayer(ref num, combined, new int[] { 0, 0, 0 }, 10, new bool[] { true, false, true });

            Assert.Equal(2, index);
        }

        [Fact]
        public void DrawBatcherTools_GenerateTheNextBatch_ConsumesASingleSizedBatchFromOneQueue()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var messenger = Substitute.For<IFrameworkMessenger>();

            var q0 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q0, FillType.Coloured, 12UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            var w0 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q0 };

            var q1 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q1, FillType.Coloured, 1UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10, 1.0f);
            var w1 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q1 };

            var q2 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q2, FillType.Coloured, 2UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10, 0.5f);
            var w2 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q2 };

            var others = new List<QueueWrap> { w1, w2 };
            var combined = tools.CombinedList(w0, others).ToArray();

            var num = 3;

            var pool = new DrawingBatch[]
            {
                new DrawingBatch()
            };

            tools.GenerateTheNextBatch(pool, 0, 0, ref num, combined, new int[] { 0, 0, 0 }, 3, new bool[] { true, true, true }, new int[] { 0, 0, 0 }, new bool[] { true, true, true });

            var batch = pool[0];

            Assert.Equal(12UL, batch.Texture0);
        }

        [Fact]
        public void DrawBatcherTools_GenerateTheNextBatch_ConsumesATwoRequestSizedBatchFromOneQueueStopsOnAnotherQueueTurn()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var messenger = Substitute.For<IFrameworkMessenger>();

            var q0 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q0, FillType.Coloured, 12UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            Add(q0, FillType.Coloured, 12UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            Add(q0, FillType.Coloured, 12UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 11);
            var w0 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q0 };

            var q1 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q1, FillType.Coloured, 1UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 9, 1.0f);
            var w1 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q1 };

            var q2 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q2, FillType.Coloured, 2UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10, 0.5f);
            var w2 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q2 };

            var others = new List<QueueWrap> { w1, w2 };
            var combined = tools.CombinedList(w0, others).ToArray();

            var num = 3;

            var pool = new DrawingBatch[]
            {
                new DrawingBatch()
            };

            tools.GenerateTheNextBatch(pool, 0, 0, ref num, combined, new int[] { 0, 0, 0 }, 3, new bool[] { true, true, true }, new int[] { 0, 0, 0 }, new bool[] { true, true, true });

            var batch = pool[0];

            Assert.Equal(12UL, batch.Texture0);
            Assert.Equal(6, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcherTools_GenerateTheNextBatch_ConsumesOneRequestButSkipsQueueDueToInactive()
        {
            IDrawStageBatcherTools tools = new DrawStageBatcherTools();

            var messenger = Substitute.For<IFrameworkMessenger>();

            var q0 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q0, FillType.Coloured, 12UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            Add(q0, FillType.Coloured, 12UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 3);
            Add(q0, FillType.Coloured, 12UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 11);
            var w0 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q0 };

            var q1 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q1, FillType.Coloured, 1UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 9, 1.0f);
            var w1 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q1 };

            var q2 = new DrawQueue(messenger, new ComparerCollection(), 12, 4, false);
            Add(q2, FillType.Coloured, 2UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 10, 0.5f);
            var w2 = new QueueWrap { Id = 0, BufferPositionOfFirstIndex = 0, BufferPositionOfFirstVertex = 0, Queue = q2 };

            var others = new List<QueueWrap> { w1, w2 };
            var combined = tools.CombinedList(w0, others).ToArray();

            var num = 3;

            var pool = new DrawingBatch[]
            {
                new DrawingBatch()
            };

            tools.GenerateTheNextBatch(pool, 1, 0, ref num, combined, new int[] { 0, 0, 0 }, 3, new bool[] { true, false, true }, new int[] { 0, 0, 0 }, new bool[] { true, false, true });

            var batch = pool[0];

            Assert.Equal(1UL, batch.Texture0);
            Assert.Equal(3, batch.NumIndices);
        }
    }
}