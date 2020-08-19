namespace Yak2D.Graphics
{
    public class QueueToBufferBlitter : IQueueToBufferBlitter
    {
        public void UnpackAndTransferToGpu(QueueWrap wrappedQueue, IDrawStageBuffers buffers)
        {
            UnpackQueueDataIntoStagingArrays(wrappedQueue, buffers);

            TransferStagedQueueDataToGpu(wrappedQueue, buffers);
        }

        private void UnpackQueueDataIntoStagingArrays(QueueWrap wrappedQueue, IDrawStageBuffers buffers)
        {
            var queue = wrappedQueue.Queue.Data;

            var minRequiredSizeOfVertexBuffer = wrappedQueue.BufferPositionOfFirstVertex + queue.NumVerticesUsed;
            var minRequiredSizeOfIndexBuffer = wrappedQueue.BufferPositionOfFirstIndex + queue.NumIndicesUsed;

            buffers.EnsureVertexBufferIsLargeEnough(minRequiredSizeOfVertexBuffer, true);
            buffers.EnsureIndexBufferIsLargeEnough(minRequiredSizeOfIndexBuffer, true);

            var posVertex = wrappedQueue.BufferPositionOfFirstVertex;
            var posIndex = wrappedQueue.BufferPositionOfFirstIndex;

            for (var req = 0; req < queue.NumRequests; req++)
            {
                var r = queue.Ordering[req];

                var isWorld = queue.Targets[r] == CoordinateSpace.World;
                var textureType = queue.Types[r] == FillType.Coloured ? 0 : queue.Types[r] == FillType.Textured ? 1 : 2;
                var baseColour = queue.BaseColours[r];

                var nV = queue.NumVertices[r];
                var v0 = queue.FirstVertexPosition[r];

                var layer = queue.Layers[r];
                var depth = queue.Depths[r];

                var firstVertex = posVertex;

                for (var v = 0; v < nV; v++)
                {
                    var qPos = v0 + v;

                    var vert = queue.Vertices[qPos];

                    var mixedColour = MixColour(baseColour, vert.Colour);

                    buffers.CopyAVertexToStagingArray(posVertex,
                                        isWorld,
                                        textureType,
                                        vert.Position,
                                        layer,
                                        depth,
                                        mixedColour,
                                        vert.TexCoord0,
                                        vert.TexCoord1,
                                        vert.TexWeighting,
                                        false);
                    posVertex++;
                }

                var nI = queue.NumIndices[r];
                var i0 = queue.FirstIndexPosition[r];

                for (var i = 0; i < nI; i++)
                {
                    var qPos = i0 + i;

                    var index = (uint)(queue.Indices[qPos] + firstVertex);

                    var pos = (uint)posIndex;

                    //Stopped using ref overload, originally as had trouble testing with nsub, but then realised copy for ints is fine here)
                    buffers.CopyAnIndexToStagingArray(pos, index, false);

                    posIndex++;
                }
            }
        }

        private Colour MixColour(Colour c0, Colour c1)
        {
            return new Colour(c0.R * c1.R, c0.G * c1.G, c0.B * c1.B, c0.A * c1.A);
        }

        private void TransferStagedQueueDataToGpu(QueueWrap wrapped, IDrawStageBuffers buffers)
        {
            var v0 = wrapped.BufferPositionOfFirstVertex;
            var vN = wrapped.Queue.Data.NumVerticesUsed;

            buffers.CopyVertexBufferSegmentToGpu(v0, vN);

            var i0 = wrapped.BufferPositionOfFirstIndex;
            var iN = wrapped.Queue.Data.NumIndicesUsed;

            buffers.CopyIndexBufferSegmentToGpu(i0, iN);
        }
    }
}