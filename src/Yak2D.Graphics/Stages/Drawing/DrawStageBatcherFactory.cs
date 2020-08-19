using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawStageBatcherFactory : IDrawStageBatcherFactory
    {
        private readonly IStartupPropertiesCache _startUpPropertiesCache;
        private readonly IDrawStageBatcherTools _batcherTools;

        public DrawStageBatcherFactory(IStartupPropertiesCache startUpPropertiesCache, IDrawStageBatcherTools batcherTools)
        {
            _startUpPropertiesCache = startUpPropertiesCache;
            _batcherTools = batcherTools;
        }

        public IDrawStageBatcher Create()
        {
            return new DrawStageBatcher(_startUpPropertiesCache.Internal.DrawStageBatcherInitialNumberOfBatches, _batcherTools);
        }
    }
}