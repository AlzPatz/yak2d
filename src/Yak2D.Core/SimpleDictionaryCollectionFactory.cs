using Yak2D.Internal;

namespace Yak2D.Core
{
    public class SimpleDictionaryCollectionFactory : ISimpleCollectionFactory
    {
        private IFrameworkMessenger _frameworkMessenger;

        public SimpleDictionaryCollectionFactory(IFrameworkMessenger frameworkMessenger)
        {
            _frameworkMessenger = frameworkMessenger;
        }

        public ISimpleCollection<T> Create<T>(uint initialCollectionSize)
        {
            return new SimpleDictionaryCollection<T>(_frameworkMessenger, initialCollectionSize);
        }
    }
}