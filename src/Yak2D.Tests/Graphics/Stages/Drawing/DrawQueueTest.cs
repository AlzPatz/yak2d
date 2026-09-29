using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class DrawQueueTest
    {
        [Fact]
        public void DrawQueueTest_InitialQueue_CatchSmallSizeInputs()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            IDrawQueue queue = new DrawQueue(messenger, -10, -4, false);

            Assert.Equal(512, queue.Data.QueueSizeSingleProperty);
            Assert.Equal(4096, queue.Data.QueueSizeIndex);
        }

        [Fact]
        public void DrawQueueTest_CheckBasicAdd_NumberAndContentOfRequestsOK()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            IDrawQueue queue = new DrawQueue(messenger, 32, 4, false);

            for (var n = 0; n < 10; n++)
            {
                var even = n % 2 == 0;
                var target = even ? CoordinateSpace.Screen : CoordinateSpace.World;
                var fill = even ? FillType.Coloured : FillType.Textured;
                if (n == 1)
                {
                    fill = FillType.DualTextured;
                }
                var val = 0.1f * n;
                var colour = new Colour(val, val, val, val);
                var verts = new Vertex2D[] { new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.One, TexWeighting = 0.9f } };
                var indices = new int[] { 0, 1, 0 };
                var texture0 = (ulong)n;
                var texture1 = (ulong)n;
                var texMode0 = TextureCoordinateMode.Mirror;
                var texMode1 = TextureCoordinateMode.Wrap;
                var depth = 0.05f * n;
                var layer = n;

                queue.Add(
                    ref target,
                    ref fill,
                    ref colour,
                    ref verts,
                    ref indices,
                    ref texture0,
                    ref texture1,
                    ref texMode0,
                    ref texMode1,
                    ref depth,
                    ref layer);
            }

            Assert.Equal(10, queue.Data.NumRequests);

            var data = queue.Data;
            //Test values in single request
            Assert.Equal(CoordinateSpace.World, data.Targets[5]);
            Assert.Equal(FillType.Textured, data.Types[5]);
            Assert.Equal(0.5f, data.BaseColours[5].R, 5);
            Assert.Equal(0.9f, data.Vertices[data.FirstVertexPosition[5]].TexWeighting, 5);
            Assert.Equal(1, data.Indices[data.FirstIndexPosition[5] + 1]);
            Assert.Equal((ulong)5, data.Texture1[5]);
            Assert.Equal(0.25f, data.Depths[5], 5);
            Assert.Equal(5, data.Layers[5]);
        }

        [Fact]
        public void DrawQueueTest_CheckBasicClear_ConfirmZeroPostClear()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            IDrawQueue queue = new DrawQueue(messenger, 32, 4, false);

            for (var n = 0; n < 10; n++)
            {
                var even = n % 2 == 0;
                var target = even ? CoordinateSpace.Screen : CoordinateSpace.World;
                var fill = even ? FillType.Coloured : FillType.Textured;
                if (n == 1)
                {
                    fill = FillType.DualTextured;
                }
                var val = 0.1f * n;
                var colour = new Colour(val, val, val, val);
                var verts = new Vertex2D[] { new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.One, TexWeighting = 0.9f } };
                var indices = new int[] { 0, 1, 0 };
                var texture0 = (ulong)n;
                var texture1 = (ulong)n;
                var texMode0 = TextureCoordinateMode.Mirror;
                var texMode1 = TextureCoordinateMode.Wrap;
                var depth = 0.05f * n;
                var layer = n;

                queue.Add(
                    ref target,
                    ref fill,
                    ref colour,
                    ref verts,
                    ref indices,
                    ref texture0,
                    ref texture1,
                    ref texMode0,
                    ref texMode1,
                    ref depth,
                    ref layer);
            }

            Assert.Equal(10, queue.Data.NumRequests);

            queue.Clear();

            Assert.Equal(0, queue.Data.NumRequests);
        }

        [Theory]
        [InlineData(0, 3, FillType.Coloured, 0UL, 0UL, 0.0f, 0)] //Fails on too few vertices
        [InlineData(12, 0, FillType.Coloured, 0UL, 0UL, 0.0f, 0)] //Fails on zero indicies
        [InlineData(12, 7, FillType.Coloured, 0UL, 0UL, 0.0f, 0)] //Fails on number indices not divisible by 3
        [InlineData(12, 24, FillType.Textured, 0UL, 0UL, 0.0f, 0)] //Fails Texture but no Texture0
        [InlineData(12, 24, FillType.DualTextured, 0UL, 0UL, 0.0f, 0)] //Fails Dual Texture but no Texture0
        [InlineData(12, 24, FillType.DualTextured, 1UL, 0UL, 0.0f, 0)] //Fails Dual Texture but no Texture1
        [InlineData(12, 24, FillType.DualTextured, 1UL, 1UL, 0.0f, 0)] //Fails Dual Texture but both textures the same
        [InlineData(12, 24, FillType.DualTextured, 1UL, 2UL, -1.0f, 0)] //Fails invalid depth
        [InlineData(12, 24, FillType.DualTextured, 1UL, 2UL, 1.0f, -1)] //Fails invalid layer
        public void DrawQueueTest_TestingAddIfValid_FailOnAllAsEachHasConfigurationIssue(int numverts, int numindices, FillType fill, ulong tex0, ulong tex1, float depth, int layer)
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            IDrawQueue queue = new DrawQueue(messenger, 32, 4, false);

            var target = CoordinateSpace.Screen;
            var colour = Colour.White;

            var vlist = new List<Vertex2D>();
            for (var n = 0; n < numverts; n++)
            {
                vlist.Add(new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.One, TexWeighting = 0.9f });
            }
            var verts = vlist.ToArray();

            var ilist = new List<int>();
            for (var n = 0; n < numindices; n++)
            {
                ilist.Add(n);
            }
            var indices = ilist.ToArray();

            var texture0 = tex0;
            var texture1 = tex1;
            var texMode0 = TextureCoordinateMode.Mirror;
            var texMode1 = TextureCoordinateMode.Wrap;

            Assert.False(queue.AddIfValid(
                ref target,
                ref fill,
                ref colour,
                ref verts,
                ref indices,
                ref texture0,
                ref texture1,
                ref texMode0,
                ref texMode1,
                ref depth,
                ref layer));
        }

        [Fact]
        public void DrawQueueTest_TestingSort_OrderingIsCorrect()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            IDrawQueue queue = new DrawQueue(messenger, 32, 4, false);

            AddItem(queue, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, FillType.Coloured, 0UL, 0UL, 0.5f, 1); //0
            AddItem(queue, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, FillType.Coloured, 0UL, 0UL, 0.6f, 1); //1
            AddItem(queue, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, FillType.Coloured, 0UL, 0UL, 0.8f, 1); //2
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 1UL, 0.7f, 0); //3
            AddItem(queue, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 1UL, 0.7f, 0); //4
            AddItem(queue, TextureCoordinateMode.Mirror, TextureCoordinateMode.Wrap, FillType.DualTextured, 0UL, 1UL, 0.7f, 0); //5
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, FillType.Textured, 0UL, 0UL, 0.8f, 0); //6
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, FillType.Textured, 2UL, 0UL, 0.8f, 0); //7
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, FillType.Textured, 1UL, 0UL, 0.8f, 0);  //8
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 1UL, 0.3f, 3); //9
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 1UL, 0.3f, 4); //10
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 1UL, 0.3f, 5); //11
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 3UL, 0.7f, 6); //12
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 2UL, 0.7f, 6); //13
            AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, FillType.DualTextured, 0UL, 1UL, 0.7f, 6); //14

            queue.Sort();

            /*
                6, 8, 7 -> Ordered by Texture0
                3, 5, 4 -> Ordered by Tex0 and Tex1 wrap modes
                2, 1, 0  -> ordered by depth
                9, 10, 11 -> ordered by layer
                14, 13, 12 -> ordered by Texture1 in Dual Textured
            */

            var order = new int[] { 6, 8, 7, 3, 5, 4, 2, 1, 0, 9, 10, 11, 14, 13, 12 };

            for(var n = 0; n < order.Length; n++)
            {
                Assert.Equal(order[n], queue.Data.Ordering[n]);
            }
        }

        [Fact]
        public void DrawQueueTest_TestingSort_IdenticalRequestsKeepSubmissionOrder()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            IDrawQueue queue = new DrawQueue(messenger, 16, 4, false);

            var count = 1000;
            for (var n = 0; n < count; n++)
            {
                AddItem(queue, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, FillType.Textured, 5UL, 0UL, 0.5f, 2);
            }

            queue.Sort();

            for (var n = 0; n < count; n++)
            {
                Assert.Equal(n, queue.Data.Ordering[n]);
            }
        }

        [Fact]
        public void DrawQueueTest_TestingSort_MatchesStableReferenceSortOnRandomData()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var random = new Random(1234);
            var layers = new[] { -3, -1, 0, 1, 7, int.MinValue, int.MaxValue };
            var depths = new[] { -1.0f, -0.0f, 0.0f, 0.25f, 0.5f, 1.0f, 3.5f };
            var textures = new[] { 0UL, 1UL, 2UL, 1000UL, ulong.MaxValue - 1UL, ulong.MaxValue, 1UL << 63 };
            var fills = new[] { FillType.Coloured, FillType.Textured, FillType.DualTextured };
            var modes = new[] { TextureCoordinateMode.None, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror };

            foreach (var skipLayersAndDepths in new[] { false, true })
            {
                IDrawQueue queue = new DrawQueue(messenger, 16, 4, skipLayersAndDepths);

                var count = 5000;
                var requests = new List<(int Index, int Layer, float Depth, ulong T0, ulong T1, FillType Fill, TextureCoordinateMode M0, TextureCoordinateMode M1)>();
                for (var n = 0; n < count; n++)
                {
                    var r = (Index: n,
                             Layer: layers[random.Next(layers.Length)],
                             Depth: depths[random.Next(depths.Length)],
                             T0: textures[random.Next(textures.Length)],
                             T1: textures[random.Next(textures.Length)],
                             Fill: fills[random.Next(fills.Length)],
                             M0: modes[random.Next(modes.Length)],
                             M1: modes[random.Next(modes.Length)]);
                    requests.Add(r);
                    AddItem(queue, r.M0, r.M1, r.Fill, r.T0, r.T1, r.Depth, r.Layer);
                }

                queue.Sort();

                // LINQ OrderBy / ThenBy is documented as a stable sort
                var ordered = skipLayersAndDepths ? requests.OrderBy(r => 0) : requests.OrderBy(r => r.Layer).ThenByDescending(r => r.Depth);
                var expected = ordered.ThenBy(r => r.T0)
                                      .ThenBy(r => r.T1)
                                      .ThenBy(r => (int)r.Fill)
                                      .ThenBy(r => (int)r.M0)
                                      .ThenBy(r => (int)r.M1)
                                      .Select(r => r.Index)
                                      .ToArray();

                Assert.Equal(expected, queue.Data.Ordering.Take(count).ToArray());

                // Sorting a persistent (already sorted) queue again gives the same result
                queue.Sort();
                Assert.Equal(expected, queue.Data.Ordering.Take(count).ToArray());
            }
        }

        private void AddItem(IDrawQueue queue,
                             TextureCoordinateMode texcoord0,
                             TextureCoordinateMode texcoord1,
                             FillType fill,
                             ulong t0,
                             ulong t1,
                             float depth,
                             int layer)
        {
            var space = CoordinateSpace.Screen;
            var colour = Colour.White;

            var verts = new Vertex2D[] 
            { 
                new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.One, TexWeighting = 0.9f }, 
                new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.One, TexWeighting = 0.9f }, 
                new Vertex2D { Colour = Colour.White, Position = Vector2.Zero, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.One, TexWeighting = 0.9f } 
            };

            var indices = new int[] { 0, 1, 2 };

            queue.Add(ref space,
                      ref fill,
                      ref colour,
                      ref verts,
                      ref indices,
                      ref t0,
                      ref t1,
                      ref texcoord0,
                      ref texcoord1,
                      ref depth,
                      ref layer);
        }
    }
}