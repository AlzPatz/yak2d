using System.Collections.Generic;
using System.Numerics;
using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class DrawQueueGroupTest
    {
        //Refactor note
        //Most of DrawStageModel's Queue manipulation has been extracted to DrawQueueGroup 
        //Enabling better testing and separation. This division is not completely clean / encapsulated 
        //(mostly as we want the render to have direct access to buffers and we expose the queues publically for testing)
        //As DrawStageModel is now just a pass-through / delegater, there is currently no testing for that class

        private IDrawQueueGroup CreateAQueueGroup()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var idGenerator = new IdGenerator(messenger);
            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });
            var queueFactory = new DrawQueueFactory(messenger, properties, new ComparerCollection());
            var buffers = Substitute.For<IDrawStageBuffers>();
            var blitter = Substitute.For<IQueueToBufferBlitter>();

            var queues = new DrawQueueGroup(idGenerator, queueFactory, buffers, blitter, false);

            return queues;
        }

        private InternalDrawRequest[] CreateASetOfDrawRequests(int numRequests,
                                                               int numVerticesPerRequest,
                                                               int numIndicesPerRequest)
        {
            var requests = new List<InternalDrawRequest>();

            var verts = new Vertex2D[numVerticesPerRequest];
            for (var n = 0; n < numVerticesPerRequest; n++)
            {
                verts[n] = new Vertex2D
                {
                    Colour = Colour.White,
                    Position = Vector2.Zero,
                    TexCoord0 = Vector2.Zero,
                    TexCoord1 = Vector2.Zero,
                    TexWeighting = 0.0f
                };
            }

            var indices = new int[numIndicesPerRequest];
            for (var n = 0; n < numIndicesPerRequest; n++)
            {
                indices[n] = 0;
            }

            for (var n = 0; n < numRequests; n++)
            {
                var request = new InternalDrawRequest
                {
                    Colour = Colour.White,
                    CoordinateSpace = CoordinateSpace.Screen,
                    Depth = 0.0f,
                    FillType = FillType.Coloured,
                    Layer = 0,
                    Texture0 = 5UL,
                    Texture1 = 6UL,
                    TextureMode0 = TextureCoordinateMode.Mirror,
                    TextureMode1 = TextureCoordinateMode.Mirror,
                    Vertices = verts,
                    Indices = indices

                };

                requests.Add(request);
            }

            return requests.ToArray();
        }

        [Fact]
        public void DrawQueueGroup_CatchInvalidRequestForPersistentQueue_ThrowsException()
        {
            var queues = CreateAQueueGroup();

            var requests = CreateASetOfDrawRequests(1, 3, 7); //# indices not divisible by 3 will trigger on validate

            Assert.Throws<Yak2DException>(() => { queues.CreateNewPersistentQueue(requests, true); });
        }

        [Theory]
        [InlineData(5, 4, 12)]
        [InlineData(1, 12, 72)]
        [InlineData(12, 7, 42)]
        [InlineData(100, 12, 144)]
        public void DrawQueueGroup_AddPersistentQueueAndCheckBufferStartIndices(int numRequests, int numVerticesPerRequest, int numIndicesPerRequest)
        {
            var queues = CreateAQueueGroup();

            var requests = CreateASetOfDrawRequests(numRequests, numVerticesPerRequest, numIndicesPerRequest);

            queues.CreateNewPersistentQueue(requests, false);

            var dynamicQueueVertexStartIndex = numRequests * numVerticesPerRequest;
            var dynamicQueueIndicesStartIndex = numRequests * numIndicesPerRequest;

            Assert.Equal(dynamicQueueVertexStartIndex, queues.DynamicQueue.BufferPositionOfFirstVertex);
            Assert.Equal(dynamicQueueIndicesStartIndex, queues.DynamicQueue.BufferPositionOfFirstIndex);
        }

        [Theory]
        [InlineData(5, 4, 12)]
        [InlineData(1, 12, 72)]
        [InlineData(12, 7, 42)]
        [InlineData(100, 12, 144)]
        public void DrawQueueGroup_AddThreePersistentQueueAndCheckBufferStartIndices(int numRequests, int numVerticesPerRequest, int numIndicesPerRequest)
        {
            var queues = CreateAQueueGroup();

            var requests = CreateASetOfDrawRequests(numRequests, numVerticesPerRequest, numIndicesPerRequest);

            queues.CreateNewPersistentQueue(requests, false);
            queues.CreateNewPersistentQueue(requests, false);
            queues.CreateNewPersistentQueue(requests, false);

            var dynamicQueueVertexStartIndex = 3 * (numRequests * numVerticesPerRequest);
            var dynamicQueueIndicesStartIndex = 3 * (numRequests * numIndicesPerRequest);

            Assert.Equal(dynamicQueueVertexStartIndex, queues.DynamicQueue.BufferPositionOfFirstVertex);
            Assert.Equal(dynamicQueueIndicesStartIndex, queues.DynamicQueue.BufferPositionOfFirstIndex);
        }

        [Theory]
        [InlineData(5, 4, 12)]
        [InlineData(1, 12, 72)]
        [InlineData(12, 7, 42)]
        [InlineData(100, 12, 144)]
        public void DrawQueueGroup_AddThreeThenRemoveMiddlePersistentQueueCheckBufferStartIndices(int numRequests, int numVerticesPerRequest, int numIndicesPerRequest)
        {
            var queues = CreateAQueueGroup();

            var requests = CreateASetOfDrawRequests(numRequests, numVerticesPerRequest, numIndicesPerRequest);
            var requestsMiddle = CreateASetOfDrawRequests(2 * numRequests, numVerticesPerRequest, numIndicesPerRequest);

            queues.CreateNewPersistentQueue(requests, false);
            var middle = queues.CreateNewPersistentQueue(requestsMiddle, false);
            queues.CreateNewPersistentQueue(requests, false);

            queues.RemovePersistentQueue(middle.Id);

            var dynamicQueueVertexStartIndex = 2 * (numRequests * numVerticesPerRequest);
            var dynamicQueueIndicesStartIndex = 2 * (numRequests * numIndicesPerRequest);

            Assert.Equal(dynamicQueueVertexStartIndex, queues.DynamicQueue.BufferPositionOfFirstVertex);
            Assert.Equal(dynamicQueueIndicesStartIndex, queues.DynamicQueue.BufferPositionOfFirstIndex);
        }
    }
}