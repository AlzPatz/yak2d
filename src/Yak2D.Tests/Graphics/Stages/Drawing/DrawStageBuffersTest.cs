using System.Collections.Generic;
using System.Numerics;
using NSubstitute;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class DrawStageBuffersTest
    {
        [Fact]
        public void DrawStageBuffers_SetMaxLayersForDepthScaling_EnsureChanges()
        {
            var components = Substitute.For<ISystemComponents>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawStageInitialSizeVertexBuffer = 12,
                DrawStageInitialSizeIndexBuffer = 36,
                DrawStageInitialMaxNumberOfLayersForDepthScaling = 12
            });

            IDrawStageBuffers buffers = new DrawStageBuffers(components, properties);

            buffers.SetMaxNumberOfLayersForDepthScaling(24);

            Assert.Equal(24, buffers.ReturnMaxNumberLayersForDepthScaling());
        }

        [Fact]
        public void DrawStageBuffers_EnsureVertexBufferLargeEnough_IncreasesBufferSize()
        {
            var components = Substitute.For<ISystemComponents>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawStageInitialSizeVertexBuffer = 12,
                DrawStageInitialSizeIndexBuffer = 36,
                DrawStageInitialMaxNumberOfLayersForDepthScaling = 12
            });

            IDrawStageBuffers buffers = new DrawStageBuffers(components, properties);

            buffers.EnsureVertexBufferIsLargeEnough(36);
            //48 as will double twice
            Assert.Equal(48U, buffers.VertexBufferSize);
        }

        [Fact]
        public void DrawStageBuffers_EnsureIndexBufferLargeEnough_IncreasesBufferSize()
        {
            var components = Substitute.For<ISystemComponents>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawStageInitialSizeVertexBuffer = 12,
                DrawStageInitialSizeIndexBuffer = 36,
                DrawStageInitialMaxNumberOfLayersForDepthScaling = 12
            });

            IDrawStageBuffers buffers = new DrawStageBuffers(components, properties);

            buffers.EnsureIndexBufferIsLargeEnough(145);
            //doubling from 36 up to large enough to hold 145... 288
            Assert.Equal(288U, buffers.IndexBufferSize);
        }

        private static DrawStageBuffers CreateBuffersWithProductionLayerMax()
        {
            var components = Substitute.For<ISystemComponents>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawStageInitialSizeVertexBuffer = 32,
                DrawStageInitialSizeIndexBuffer = 96,
                DrawStageInitialMaxNumberOfLayersForDepthScaling = 10
            });

            return new DrawStageBuffers(components, properties);
        }

        private static void StageVertex(DrawStageBuffers buffers, int index, int layer, float depth)
        {
            buffers.CopyAVertexToStagingArray(index,
                                              true,
                                              0,
                                              Vector2.Zero,
                                              layer,
                                              depth,
                                              Colour.White,
                                              Vector2.Zero,
                                              Vector2.Zero,
                                              0.0f,
                                              false);
        }

        [Fact]
        public void DrawStageBuffers_LayerAtOrAboveMax_GrowsMaxAndKeepsDepthsInClipRangeAndOrdered()
        {
            var buffers = CreateBuffersWithProductionLayerMax();

            for (var layer = 0; layer < 10; layer++)
            {
                StageVertex(buffers, layer, layer, 0.5f);
            }

            StageVertex(buffers, 10, 10, 0.5f);
            StageVertex(buffers, 11, 25, 0.5f);

            //10 -> 20 (fits layer 10) -> 40 (fits layer 25)
            Assert.Equal(40, buffers.ReturnMaxNumberLayersForDepthScaling());

            for (var i = 0; i < 12; i++)
            {
                Assert.InRange(buffers.ReturnStagedVertexDepth(i), 0.0f, 1.0f);
            }

            //Higher layer = nearer = smaller z
            for (var i = 1; i < 12; i++)
            {
                Assert.True(buffers.ReturnStagedVertexDepth(i) < buffers.ReturnStagedVertexDepth(i - 1),
                            $"Depth at index {i} ({buffers.ReturnStagedVertexDepth(i)}) is not in front of index {i - 1} ({buffers.ReturnStagedVertexDepth(i - 1)})");
            }
        }

        [Fact]
        public void DrawStageBuffers_GrowingMaxLayers_KeepsRelativeOrderOfDepthsQueuedBefore()
        {
            var buffers = CreateBuffersWithProductionLayerMax();

            var staged = new List<(int Layer, float Depth)>
            {
                (0, 0.25f),
                (0, 0.75f),
                (3, 0.1f),
                (9, 1.0f),
                (9, 0.0f),
                (5, 0.5f)
            };

            for (var i = 0; i < staged.Count; i++)
            {
                StageVertex(buffers, i, staged[i].Layer, staged[i].Depth);
            }

            var before = new float[staged.Count];
            for (var i = 0; i < staged.Count; i++)
            {
                before[i] = buffers.ReturnStagedVertexDepth(i);
            }

            StageVertex(buffers, staged.Count, 25, 0.5f);

            Assert.Equal(40, buffers.ReturnMaxNumberLayersForDepthScaling());

            var after = new float[staged.Count];
            for (var i = 0; i < staged.Count; i++)
            {
                after[i] = buffers.ReturnStagedVertexDepth(i);
            }

            for (var i = 0; i < staged.Count; i++)
            {
                for (var j = 0; j < staged.Count; j++)
                {
                    Assert.Equal(System.Math.Sign(before[i] - before[j]), System.Math.Sign(after[i] - after[j]));
                }
            }

            for (var i = 0; i < staged.Count; i++)
            {
                Assert.InRange(after[i], 0.0f, 1.0f);

                var expected = 1.0f - (staged[i].Layer / 40.0f) - ((1.0f - staged[i].Depth) / 40.0f);
                Assert.Equal(expected, after[i], 5);
            }
        }

        [Fact]
        public void DrawStageBuffers_NegativeLayer_DoesNotGrowMax()
        {
            var buffers = CreateBuffersWithProductionLayerMax();

            StageVertex(buffers, 0, -1, 0.5f);

            Assert.Equal(10, buffers.ReturnMaxNumberLayersForDepthScaling());
            Assert.True(buffers.ReturnStagedVertexDepth(0) >= 1.0f);
        }
    }
}