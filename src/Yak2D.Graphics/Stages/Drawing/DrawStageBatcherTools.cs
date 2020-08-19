using System.Collections.Generic;

namespace Yak2D.Graphics
{
    public class DrawStageBatcherTools : IDrawStageBatcherTools
    {
        public DrawingBatch[] CreatePoolOfSize(int size, bool copyExisting, DrawingBatch[] existing = null)
        {
            var oldPool = existing;

            var pool = new DrawingBatch[size];
            for (var p = 0; p < size; p++)
            {
                pool[p] = new DrawingBatch();
            }

            if (copyExisting)
            {
                var oldSize = oldPool.Length;
                var num = oldSize < size ? oldSize : size;
                for (var p = 0; p < num; p++)
                {
                    DeepCopyBatchValues(oldPool[p], pool[p]);
                }
            }

            return pool;
        }

        private void DeepCopyBatchValues(DrawingBatch from, DrawingBatch to)
        {
            to.NumIndices = from.NumIndices;
            to.StartIndex = from.StartIndex;
            to.TextureMode0 = from.TextureMode0;
            to.TextureMode1 = from.TextureMode1;
            to.Texture0 = from.Texture0;
            to.Texture1 = from.Texture1;
        }

        public List<QueueWrap> CombinedList(QueueWrap first, List<QueueWrap> others)
        {
            var list = new List<QueueWrap>(others.Count + 1);
            list.Add(first);
            others.ForEach(x => list.Add(x));
            return list;
        }

        public bool AreAllQueuesInactive(ref int numQueues, bool[] activeQueues)
        {
            for (var n = 0; n < numQueues; n++)
            {
                if (activeQueues[n])
                    return false;
            }
            return true;
        }

        public int FindLowestLayer(ref int numQueues, QueueWrap[] wraps, bool[] active, int[] nextRequest)
        {
            var lowest = int.MaxValue;

            for (var n = 0; n < numQueues; n++)
            {
                if (active[n])
                {
                    var queue = wraps[n].Queue;
                    var candidate = queue.Data.Layers[queue.Data.Ordering[nextRequest[n]]];
                    if (candidate < lowest)
                    {
                        lowest = candidate;
                    }
                }
            }

            return lowest;
        }

        public void IdentifyWhichQueuesAreActiveInTheCurrentLayer(ref int numQueues, QueueWrap[] wraps, int[] nextRequest, int layer, bool[] activeInLayer, bool[] activeOverAllLayers)
        {
            for (var n = 0; n < numQueues; n++)
            {
                if (activeOverAllLayers[n])
                {
                    var queue = wraps[n].Queue.Data;
                    var candidate = queue.Layers[queue.Ordering[nextRequest[n]]];
                    activeInLayer[n] = candidate == layer;
                }
                else
                {
                    activeInLayer[n] = false;
                }
            }
        }

        public int FindDeepestQueueAtLowestLayer(ref int numQueues, QueueWrap[] wraps, int[] nextRequest, int lowestLayer, bool[] activeInCurrentLayer)
        {
            var deepestDepth = float.MinValue;
            var deepestIndex = -1;

            for (var n = 0; n < numQueues; n++)
            {
                if (!activeInCurrentLayer[n])
                {
                    continue;
                }

                var queue = wraps[n].Queue.Data;

                var layer = queue.Layers[queue.Ordering[nextRequest[n]]];

                if (layer != lowestLayer)
                {
                    continue;
                }

                var candidate = queue.Depths[queue.Ordering[nextRequest[n]]];

                if (candidate > deepestDepth)
                {
                    deepestDepth = candidate;
                    deepestIndex = n;
                }
            }

            return deepestIndex;
        }

        public bool GenerateTheNextBatch(DrawingBatch[] pool, int queue, int batchIndex, ref int numQueues, QueueWrap[] wraps, int[] nextRequestToConsume, int lowestLayer, bool[] activeInCurrentLayer, int[] indexCounter, bool[] activeOverAllLayers)
        {
            var lengthOfBatchInQueue = 0;

            //We Switch On:
            //    Change in Layer
            //    Change in Queue
            //    Once one of the textures is attempted to be set more than once each 
            //OR  TextureWrapMode is changed on active texture
            //    When new texture is selected the batch's texture wrap mode is also set for that texture

            var rawQs = new IDrawQueue[numQueues];
            for (var n = 0; n < numQueues; n++)
            {
                rawQs[n] = wraps[n].Queue;
            }

            var queueStartIndex = indexCounter[queue];

            var nextQueueIndex = rawQs[queue].Data.Ordering[nextRequestToConsume[queue]];
            var layer = lowestLayer;
            var tex0 = rawQs[queue].Data.Texture0[nextQueueIndex];
            var hasTexBeenSet0 = tex0 != 0UL;
            var tMode0 = rawQs[queue].Data.TextureMode0[nextQueueIndex];
            var tex1 = rawQs[queue].Data.Texture1[nextQueueIndex];
            var tMode1 = rawQs[queue].Data.TextureMode1[nextQueueIndex];
            var hasTexBeenSet1 = tex1 != 0UL;

            ConsumeNextRequest(queue,
                               rawQs,
                               nextRequestToConsume,
                               indexCounter,
                               ref tex0,
                               ref hasTexBeenSet0,
                               ref tMode0,
                               ref tex1,
                               ref hasTexBeenSet1,
                               ref tMode1,
                               activeOverAllLayers);
            lengthOfBatchInQueue++;

            while (true)
            {
                if (CheckIfNextInQueueShouldBeIncluded(queue,
                                                       numQueues,
                                                       rawQs,
                                                       nextRequestToConsume,
                                                       activeInCurrentLayer,
                                                       layer,
                                                       tex0,
                                                       tMode0,
                                                       hasTexBeenSet0,
                                                       tex1,
                                                       tMode1,
                                                       hasTexBeenSet1,
                                                       activeOverAllLayers))
                {
                    ConsumeNextRequest(queue,
                                       rawQs,
                                       nextRequestToConsume,
                                       indexCounter,
                                       ref tex0,
                                       ref hasTexBeenSet0,
                                       ref tMode0,
                                       ref tex1,
                                       ref hasTexBeenSet1,
                                       ref tMode1,
                                       activeOverAllLayers);
                    lengthOfBatchInQueue++;
                }
                else
                {
                    //End DrawingBatch
                    break;
                }
            }

            var length = indexCounter[queue] - queueStartIndex;

            if (length == 0)
            {
                return false;
            }

            pool[batchIndex].StartIndex = queueStartIndex + wraps[queue].BufferPositionOfFirstIndex;
            pool[batchIndex].NumIndices = indexCounter[queue] - queueStartIndex;
            pool[batchIndex].Texture0 = tex0;
            pool[batchIndex].Texture1 = tex1;
            pool[batchIndex].TextureMode0 = tMode0;
            pool[batchIndex].TextureMode1 = tMode1;

            return true;
        }

        private void ConsumeNextRequest(int queue,
                        IDrawQueue[] rawQs,
                        int[] nextRequestToConsume,
                        int[] indexCounter,
                        ref ulong tex0,
                        ref bool hastex0,
                        ref TextureCoordinateMode tMode0,
                        ref ulong tex1,
                        ref bool hastex1,
                        ref TextureCoordinateMode tMode1,
                        bool[] activeQueuesOverAllLayers)
        {
            var currentQueueNextIndex = rawQs[queue].Data.Ordering[nextRequestToConsume[queue]];

            var cTex0 = rawQs[queue].Data.Texture0[currentQueueNextIndex];
            var cTex1 = rawQs[queue].Data.Texture1[currentQueueNextIndex];

            if (!hastex0)
            {
                tMode0 = rawQs[queue].Data.TextureMode0[currentQueueNextIndex];
            }

            if (!hastex1)
            {
                tMode1 = rawQs[queue].Data.TextureMode1[currentQueueNextIndex];
            }

            if (cTex0 != 0UL)
            {
                tex0 = cTex0;
                hastex0 = true;
            }

            if (cTex1 != 0UL)
            {
                tex1 = cTex1;
                hastex1 = true;
            }

            var numIndicesInRequest = rawQs[queue].Data.NumIndices[currentQueueNextIndex];

            indexCounter[queue] += numIndicesInRequest;

            //Advance next request to consume and ensure has not reached the end of the Queue
            nextRequestToConsume[queue]++;
            if (nextRequestToConsume[queue] >= rawQs[queue].Data.NumRequests)
            {
                //Shut this queue down
                activeQueuesOverAllLayers[queue] = false;
            }
        }

        private bool CheckIfNextInQueueShouldBeIncluded(int queue,
                                                        int numQueues,
                                                        IDrawQueue[] rawQs,
                                                        int[] nextRequestToConsume,
                                                        bool[] activeInCurrentLayer,
                                                        int layer,
                                                        ulong tex0,
                                                        TextureCoordinateMode tMode0,
                                                        bool hasTexBeenSet0,
                                                        ulong tex1,
                                                        TextureCoordinateMode tMode1,
                                                        bool hasTexBeenSet1,
                                                        bool[] activeQueuesOverAllLayers)
        {
            //Ensure queue remains valid
            if (!activeQueuesOverAllLayers[queue])
            {
                return false;
            }

            //Ensure next in queue has the highest depth
            var currentQueueNextIndex = rawQs[queue].Data.Ordering[nextRequestToConsume[queue]];
            var currentQueueDepthIsHighest = true;
            var currentQueueDepth = rawQs[queue].Data.Depths[currentQueueNextIndex];

            for (var n = 0; n < numQueues; n++)
            {
                if (n == queue || !activeQueuesOverAllLayers[n] || !activeInCurrentLayer[n]) //made some changes here might breaksomething
                {
                    continue;
                }

                var nextIndex = rawQs[n].Data.Ordering[nextRequestToConsume[n]];
                var nextDepth = rawQs[n].Data.Depths[nextIndex];

                if (nextDepth > currentQueueDepth)
                {
                    currentQueueDepthIsHighest = false;
                    break;
                }
            }

            if (!currentQueueDepthIsHighest)
            {
                return false;
            }

            //Now we are confirmed to be potentially consuming from the current queue, we check for reasons to end the DrawingBatch
            //Layer and (invalid) Texture Changing

            var nextLayer = rawQs[queue].Data.Layers[currentQueueNextIndex];

            if (nextLayer != layer)
            {
                return false;
            }

            var nextTexture0 = rawQs[queue].Data.Texture0[currentQueueNextIndex];

            if (nextTexture0 != tex0)
            {
                if (nextTexture0 != 0UL && hasTexBeenSet0)
                {
                    return false;
                }
            }

            var nextTextureMode0 = rawQs[queue].Data.TextureMode0[currentQueueNextIndex];

            if (nextTextureMode0 != tMode0)
            {
                if (nextTextureMode0 != TextureCoordinateMode.None && tMode0 != TextureCoordinateMode.None)
                {
                    return false;
                }
            }

            var nextTexture1 = rawQs[queue].Data.Texture1[currentQueueNextIndex];

            if (nextTexture1 != tex1)
            {
                if (nextTexture1 != 0UL && hasTexBeenSet1)
                {
                    return false;
                }
            }

            var nextTextureMode1 = rawQs[queue].Data.TextureMode1[currentQueueNextIndex];

            if (nextTextureMode1 != tMode1)
            {
                if (nextTextureMode1 != TextureCoordinateMode.None && tMode1 != TextureCoordinateMode.None)
                {
                    return false;
                }
            }

            return true;
        }
    }
}