using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawQueueFactory : IDrawQueueFactory
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IStartupPropertiesCache _startUpPropertiesCache;
        private readonly IComparerCollection _comparerCollection;

        public DrawQueueFactory(IFrameworkMessenger frameworkMessenger,
                                IStartupPropertiesCache startUpPropertiesCache,
                                IComparerCollection comparerCollection)
        {
            _frameworkMessenger = frameworkMessenger;
            _startUpPropertiesCache = startUpPropertiesCache;
            _comparerCollection = comparerCollection;
        }

        public IDrawQueue Create(bool skipDrawQueueSortingByDepthsAndLayers)
        {
            return new DrawQueue(_frameworkMessenger,
                                 _comparerCollection,
                                 _startUpPropertiesCache.Internal.DrawQueueInitialSizeNumberOfRequests,
                                 _startUpPropertiesCache.Internal.DrawQueueInitialSizeElementsPerRequestScalar,
                                 skipDrawQueueSortingByDepthsAndLayers);
        }
    }
}