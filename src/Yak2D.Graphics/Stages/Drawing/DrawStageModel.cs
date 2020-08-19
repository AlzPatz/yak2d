using Veldrid;

namespace Yak2D.Graphics
{
    public class DrawStageModel : IDrawStageModel
    {
        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public IDrawStageBuffers Buffers => _queues?.Buffers;

        public IDrawStageBatcher Batcher { get; private set; }
        public BlendState BlendState { get; private set; }

        private readonly IDrawQueueGroup _queues;

        private bool _stageIsSortedAndProcessed;

        public DrawStageModel(IDrawStageBatcher batcher,
                              IDrawQueueGroup queues,
                              BlendState blendState)
        {
            Batcher = batcher;
            _queues = queues;

            BlendState = blendState;

            _stageIsSortedAndProcessed = false;
        }

        public void DrawToDynamicQueue(ref CoordinateSpace target,
                                       ref FillType type,
                                       ref Colour colour,
                                       ref Vertex2D[] vertices,
                                       ref int[] indices,
                                       ref ulong texture0,
                                       ref ulong texture1,
                                       ref TextureCoordinateMode texMode0,
                                       ref TextureCoordinateMode texMode1,
                                       ref float depth,
                                       ref int layer,
                                       bool validate = false)
        {
            AddToQueue(_queues.DynamicQueue.Queue,
                                                     ref target,
                                                     ref type,
                                                     ref colour,
                                                     ref vertices,
                                                     ref indices,
                                                     ref texture0,
                                                     ref texture1,
                                                     ref texMode0,
                                                     ref texMode1,
                                                     ref depth,
                                                     ref layer,
                                                     ref validate);
        }

        private void AddToQueue(IDrawQueue queue,
                                ref CoordinateSpace target,
                                ref FillType type,
                                ref Colour colour,
                                ref Vertex2D[] vertices,
                                ref int[] indices,
                                ref ulong texture0,
                                ref ulong texture1,
                                ref TextureCoordinateMode texMode0,
                                ref TextureCoordinateMode texMode1,
                                ref float depth,
                                ref int layer,
                                ref bool validate)
        {
            if (validate)
            {
                var success = queue.AddIfValid(ref target,
                                               ref type,
                                               ref colour,
                                               ref vertices,
                                               ref indices,
                                               ref texture0,
                                               ref texture1,
                                               ref texMode0,
                                               ref texMode1,
                                               ref depth,
                                               ref layer);

                if (!success)
                {
                    throw new Yak2DException("Add draw request to dynamic draw queue queue failed. Request validation failed. Reason written to debug output");
                }
            }
            else
            {
                queue.Add(ref target,
                          ref type,
                          ref colour,
                          ref vertices,
                          ref indices,
                          ref texture0,
                          ref texture1,
                          ref texMode0,
                          ref texMode1,
                          ref depth,
                          ref layer);
            }

            _stageIsSortedAndProcessed = false;
        }

        public void ClearDynamicDrawQueue()
        {
            _queues.ClearDynamicQueue();
            _stageIsSortedAndProcessed = false;
        }

        public void Process()
        {
            if (!_stageIsSortedAndProcessed)
            {
                _queues?.ProcessDynamicQueue();
                Batcher.Process(_queues.DynamicQueue, _queues.PersistentQueues);
                _stageIsSortedAndProcessed = true;
            }
        }

        public IPersistentDrawQueue AddPersistentQueue(InternalDrawRequest[] requests, bool validate = false)
        {
            if (requests == null || requests.Length == 0)
                return null;

            var queue = _queues.CreateNewPersistentQueue(requests, validate);

            _queues.ProcessPersistentQueue(queue.Id);

            return new PersistentDrawQueueReference(queue.Id);
        }

        public void RemovePersistentQueue(IPersistentDrawQueue queue) => _queues.RemovePersistentQueue(queue.Id);
        public void RemovePersistentQueue(ulong queue) => _queues.RemovePersistentQueue(queue);

        public virtual void Update(float seconds)
        {
            return;
        }

        public void DestroyResources()
        {
            Buffers?.DestroyResources();
        }
    }
}