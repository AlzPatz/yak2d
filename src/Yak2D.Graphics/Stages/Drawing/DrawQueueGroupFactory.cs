using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawQueueGroupFactory : IDrawQueueGroupFactory
    {
        private readonly IDrawQueueFactory _queueFactory;
        private readonly IIdGenerator _idGenerator;
        private readonly IDrawStageBuffersFactory _buffersFactory;
        private readonly IQueueToBufferBlitter _queueToBufferBlitter;

        public DrawQueueGroupFactory(IDrawQueueFactory queueFactory,
                                    IIdGenerator idGenerator,
                                    IDrawStageBuffersFactory buffersFactory,
                                    IQueueToBufferBlitter queueToBufferBlitter)

        {
            _queueFactory = queueFactory;
            _idGenerator = idGenerator;
            _buffersFactory = buffersFactory;
            _queueToBufferBlitter = queueToBufferBlitter;
        }

        public IDrawQueueGroup Create(bool skipDrawQueueSortingByDepthsAndLayers)
        {
            return new DrawQueueGroup(_idGenerator,
                                      _queueFactory,
                                      _buffersFactory.Create(),
                                      _queueToBufferBlitter,
                                      skipDrawQueueSortingByDepthsAndLayers);
        }
    }
}