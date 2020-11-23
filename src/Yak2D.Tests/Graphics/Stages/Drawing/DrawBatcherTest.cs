using System.Numerics;
using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class DrawBatcherTest
    {
        /*
         Batch boundaries are triggered by:
            - Change in layer
            - Change in Queue being used
            - Once one of the texture slots is attempted to be set more than once
            OR
            - When a texture wrap mode is changed on an active texture
         */

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

        //Helper created draw request to support creating persistent queue
        private InternalDrawRequest Create(FillType fill,
                       ulong texture0,
                       ulong texture1,
                       TextureCoordinateMode tmode0,
                       TextureCoordinateMode tmode1,
                       int layer,
                       float depth = 0.5f)
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

            return new InternalDrawRequest
            {
                Colour = colour,
                CoordinateSpace = target,
                Depth = depth,
                FillType = fill,
                Indices = indices,
                Layer = layer,
                Texture0 = texture0,
                Texture1 = texture1,
                TextureMode0 = tmode0,
                TextureMode1 = tmode1,
                Vertices = verts
            };
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_TestSuccessfulWithOnlyDynamicQueueAndOneRequest()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(1, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_TwoBatchesDueToLayerChange()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0);
            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 1);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(2, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[1];

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(3, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_ThreeBatchesDueToQueueChange()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            //Dynamic

            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0);
            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 1);

            queues.ProcessDynamicQueue();

            //Persistent

            var requests = new InternalDrawRequest[]
            {
                Create(FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0)
            };

            queues.CreateNewPersistentQueue(requests, false);

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(3, batcher.NumberOfBatches);

            var batch = batcher.Pool[0]; //In dynamic queue

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(3, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[1]; //In persistent queue so at start of buffer

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[2]; //Back to dynamic queue

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(6, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_FiveBatchesDueToTwoQueueChangesPlusADualBatchSingleQueueInThere()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            //Dynamic

            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 0.5f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 20UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.None, 1);

            queues.ProcessDynamicQueue();

            //Persistent

            var requests0 = new InternalDrawRequest[]
            {
                Create(FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 0.2f)
            };

            queues.CreateNewPersistentQueue(requests0, false);

            var requests1 = new InternalDrawRequest[]
            {
                //Depth 1.0f puts it at the beginning
                Create(FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.None, 0, 1.0f),
                Create(FillType.Textured, 10UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.None, 0, 1.0f)
            };

            queues.CreateNewPersistentQueue(requests1, false);

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(5, batcher.NumberOfBatches);

            var batch = batcher.Pool[0]; //In second persistent queue

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Mirror, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(3, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[1]; //In second persistent queue

            Assert.Equal(10UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Mirror, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(6, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[2]; //In dynamic queue

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(9, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[3]; //In first persistent queue

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[4]; //Back to dynamic queue

            Assert.Equal(20UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(12, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_TwoBatchesDueToTextureChange()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 10UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(2, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[1];

            Assert.Equal(10UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(3, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_TwoBatchesDueToTextureWrapModeChange()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.None, 0);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.None, 0);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(2, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[1];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Mirror, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            Assert.Equal(3, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_TwoBatchesDueToTextureWrapModeChangeForDualTextured()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.DualTextured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0);
            Add(queues.DynamicQueue.Queue, FillType.DualTextured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, 0);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(2, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);

            batch = batcher.Pool[1];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.Mirror, batch.TextureMode1);
            Assert.Equal(3, batch.StartIndex);
            Assert.Equal(3, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_TextureChangePersistence_OneBatchForSingleDualSingleTexAndColourInterleave()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(messenger),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 1.0f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0, 0.95f);
            Add(queues.DynamicQueue.Queue, FillType.DualTextured, 5UL, 10UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0, 0.9f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0, 0.8f);
            Add(queues.DynamicQueue.Queue, FillType.DualTextured, 5UL, 10UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0, 0.7f);
            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 0.6f);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(1, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(10UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(18, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_TextureChangePersistence_TwoBatchForSingleDualSingleTexAndColourInterleaveAsWrapChanges()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(Substitute.For<IFrameworkMessenger>()),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 1.0f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0, 0.95f);
            Add(queues.DynamicQueue.Queue, FillType.DualTextured, 5UL, 10UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0, 0.9f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Wrap, 0, 0.8f);
            Add(queues.DynamicQueue.Queue, FillType.DualTextured, 5UL, 10UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, 0, 0.7f);
            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 0.6f);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(2, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(10UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode1);
            Assert.Equal(0, batch.StartIndex);
            Assert.Equal(12, batch.NumIndices);

            batch = batcher.Pool[1];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(10UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.Mirror, batch.TextureMode1);
            Assert.Equal(12, batch.StartIndex);
            Assert.Equal(6, batch.NumIndices);
        }

        [Fact]
        public void DrawBatcher_TestBatchCreation_ReplicatingTextureModeIssues_HappensWhenModeNoneProvidedToEarlyTexCall()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var tools = new DrawStageBatcherTools();

            IDrawStageBatcher batcher = new DrawStageBatcher(12, tools);

            var properties = Substitute.For<IStartupPropertiesCache>();
            properties.Internal.Returns(new InternalStartUpProperties
            {
                DrawQueueInitialSizeNumberOfRequests = 32,
                DrawQueueInitialSizeElementsPerRequestScalar = 4
            });

            var queues = new DrawQueueGroup(new IdGenerator(Substitute.For<IFrameworkMessenger>()),
                                            new DrawQueueFactory(messenger, properties, new ComparerCollection()),
                                            Substitute.For<IDrawStageBuffers>(),
                                            Substitute.For<IQueueToBufferBlitter>(),
                                            false);

            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 1.0f);
            Add(queues.DynamicQueue.Queue, FillType.Coloured, 0UL, 0UL, TextureCoordinateMode.None, TextureCoordinateMode.None, 0, 1.0f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.None, 1, 0.5f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 5UL, 0UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.None, 1, 0.5f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 6UL, 0UL, TextureCoordinateMode.Mirror, TextureCoordinateMode.None, 1, 0.3f);
            Add(queues.DynamicQueue.Queue, FillType.Textured, 7UL, 8UL, TextureCoordinateMode.Wrap, TextureCoordinateMode.Mirror, 1, 0.3f);

            queues.ProcessDynamicQueue();

            batcher.Process(queues.DynamicQueue, queues.PersistentQueues);

            Assert.Equal(4, batcher.NumberOfBatches);

            var batch = batcher.Pool[0];

            Assert.Equal(0UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);
            batch = batcher.Pool[1];

            Assert.Equal(5UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);

            batch = batcher.Pool[2];

            Assert.Equal(6UL, batch.Texture0);
            Assert.Equal(0UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Mirror, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.None, batch.TextureMode1);

            batch = batcher.Pool[3];

            Assert.Equal(7UL, batch.Texture0);
            Assert.Equal(8UL, batch.Texture1);
            Assert.Equal(TextureCoordinateMode.Wrap, batch.TextureMode0);
            Assert.Equal(TextureCoordinateMode.Mirror, batch.TextureMode1);
        }
    }
}