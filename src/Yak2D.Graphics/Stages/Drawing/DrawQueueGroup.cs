using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawQueueGroup : IDrawQueueGroup
    {
        public IDrawStageBuffers Buffers { get; private set; }
        public QueueWrap DynamicQueue { get; private set; }
        public List<QueueWrap> PersistentQueues { get; private set; }

        private readonly IIdGenerator _idGenerator;
        private readonly IDrawQueueFactory _queueFactory;
        private readonly IQueueToBufferBlitter _blitter;
        private readonly bool _skipDrawQueueSortingByDepthsAndLayers;

        public DrawQueueGroup(IIdGenerator idGenerator,
                              IDrawQueueFactory queueFactory,
                              IDrawStageBuffers buffers,
                              IQueueToBufferBlitter blitter,
                              bool skipDrawQueueSortingByDepthsAndLayers)
        {
            Buffers = buffers;
            _idGenerator = idGenerator;
            _queueFactory = queueFactory;
            _blitter = blitter;

            _skipDrawQueueSortingByDepthsAndLayers = skipDrawQueueSortingByDepthsAndLayers;

            DynamicQueue = new QueueWrap
            {
                Id = 0UL,
                Queue = _queueFactory.Create(_skipDrawQueueSortingByDepthsAndLayers),
                BufferPositionOfFirstVertex = 0,
                BufferPositionOfFirstIndex = 0
            };

            PersistentQueues = new List<QueueWrap>(); //List not dictionary keyed with id so can be confident about order when iterating (dic probably fine...)
        }

        public void ClearDynamicQueue()
        {
            DynamicQueue.Queue.Clear();
        }

        public void ProcessDynamicQueue()
        {
            DynamicQueue.Queue.Sort();
            LoadDynamicQueueDataToGpu();
        }

        private void LoadDynamicQueueDataToGpu()
        {
            if (DynamicQueue.Queue.Data.NumRequests == 0)
            {
                return;
            }

            _blitter.UnpackAndTransferToGpu(DynamicQueue, Buffers);
        }

        public QueueWrap CreateNewPersistentQueue(InternalDrawRequest[] requests, bool validate)
        {
            var queue = _queueFactory.Create(_skipDrawQueueSortingByDepthsAndLayers);

            for (var n = 0; n < requests.Length; n++)
            {
                var tex0 = requests[n].Texture0;
                var tex1 = requests[n].Texture1;

                if (validate)
                {
                    var success = queue.AddIfValid(ref requests[n].CoordinateSpace,
                                                   ref requests[n].FillType,
                                                   ref requests[n].Colour,
                                                   ref requests[n].Vertices,
                                                   ref requests[n].Indices,
                                                   ref tex0,
                                                   ref tex1,
                                                   ref requests[n].TextureMode0,
                                                   ref requests[n].TextureMode1,
                                                   ref requests[n].Depth,
                                                   ref requests[n].Layer);

                    if (!success)
                    {
                        throw new Yak2DException("Create new persistent queue failed. Request validation failed. Reason written to debug output");
                    }
                }
                else
                {
                    queue.Add(ref requests[n].CoordinateSpace,
                              ref requests[n].FillType,
                              ref requests[n].Colour,
                              ref requests[n].Vertices,
                              ref requests[n].Indices,
                              ref tex0,
                              ref tex1,
                              ref requests[n].TextureMode0,
                              ref requests[n].TextureMode1,
                              ref requests[n].Depth,
                              ref requests[n].Layer);
                }
            }

            var id = _idGenerator.New();

            var wrappedQueue = new QueueWrap
            {
                Id = id,
                BufferPositionOfFirstVertex = DynamicQueue.BufferPositionOfFirstVertex,
                BufferPositionOfFirstIndex = DynamicQueue.BufferPositionOfFirstIndex,
                Queue = queue
            };

            PersistentQueues.Add(wrappedQueue);

            DynamicQueue.BufferPositionOfFirstVertex += queue.Data.NumVerticesUsed;
            DynamicQueue.BufferPositionOfFirstIndex += queue.Data.NumIndicesUsed;

            return wrappedQueue;
        }

        public void ProcessPersistentQueue(ulong id)
        {
            var queue = FindPersistentQueueById(id);

            if (queue == null)
            {
                return;
            }

            queue.Queue.Sort();
            LoadPersistentQueueDataToGpu(queue);
        }

        private QueueWrap FindPersistentQueueById(ulong id)
        {
            for (var n = 0; n < PersistentQueues.Count; n++)
            {
                if (PersistentQueues[n].Id == id)
                {
                    return PersistentQueues[n];
                }
            }

            return null;
        }

        private void LoadPersistentQueueDataToGpu(QueueWrap queue)
        {
            if (queue.Queue.Data.NumRequests == 0)
            {
                return;
            }

            _blitter.UnpackAndTransferToGpu(queue, Buffers);
        }

        public void RemovePersistentQueue(ulong id)
        {
            var index = FindPersistentQueueIndex(id);

            if (index == -1)
            {
                return;
            }

            RemovePersistentQueue(index);
        }

        private int FindPersistentQueueIndex(ulong id)
        {
            for (var n = 0; n < PersistentQueues.Count; n++)
            {
                if (PersistentQueues[n].Id == id)
                {
                    return n;
                }
            }

            return -1;
        }

        private void RemovePersistentQueue(int index)
        {
            if (index < 0 || index >= PersistentQueues.Count)
            {
                return;
            }

            PersistentQueues.RemoveAt(index);

            //Shift all later persistent queue data downwards if there are queues after the one removed
            if (index < PersistentQueues.Count)
            {
                for (var q = index; q < PersistentQueues.Count; q++)
                {
                    var newStartVertexPosition = index == 0 ? 0 : PersistentQueues[index - 1].BufferPositionOfFirstVertex + PersistentQueues[index - 1].Queue.Data.NumVerticesUsed;
                    var newStartIndexPosition = index == 0 ? 0 : PersistentQueues[index - 1].BufferPositionOfFirstIndex + PersistentQueues[index - 1].Queue.Data.NumIndicesUsed;

                    PersistentQueues[index].BufferPositionOfFirstVertex = newStartVertexPosition;
                    PersistentQueues[index].BufferPositionOfFirstIndex = newStartIndexPosition;

                    LoadPersistentQueueDataToGpu(PersistentQueues[index]);
                }
            }

            SetDynamicBufferFirstTriangleIndexToOneAfterPersistentQueues();
        }

        private void SetDynamicBufferFirstTriangleIndexToOneAfterPersistentQueues()
        {
            DynamicQueue.BufferPositionOfFirstVertex = PersistentQueues.Count == 0 ? 0 :
                PersistentQueues[PersistentQueues.Count - 1].BufferPositionOfFirstVertex + PersistentQueues[PersistentQueues.Count - 1].Queue.Data.NumVerticesUsed;

            DynamicQueue.BufferPositionOfFirstIndex = PersistentQueues.Count == 0 ? 0 :
                PersistentQueues[PersistentQueues.Count - 1].BufferPositionOfFirstIndex + PersistentQueues[PersistentQueues.Count - 1].Queue.Data.NumIndicesUsed;
        }
    }
}