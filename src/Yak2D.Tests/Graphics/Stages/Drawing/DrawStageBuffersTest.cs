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
    }
}