using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawQueueFactory : IDrawQueueFactory
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IStartupPropertiesCache _startUpPropertiesCache;

        public DrawQueueFactory(IFrameworkMessenger frameworkMessenger,
                                IStartupPropertiesCache startUpPropertiesCache)
        {
            _frameworkMessenger = frameworkMessenger;
            _startUpPropertiesCache = startUpPropertiesCache;
        }

        public IDrawQueue Create(bool skipDrawQueueSortingByDepthsAndLayers)
        {
            return new DrawQueue(_frameworkMessenger,
                                 _startUpPropertiesCache.Internal.DrawQueueInitialSizeNumberOfRequests,
                                 _startUpPropertiesCache.Internal.DrawQueueInitialSizeElementsPerRequestScalar,
                                 skipDrawQueueSortingByDepthsAndLayers);
        }
    }
}