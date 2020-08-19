namespace Yak2D.Internal
{
    public interface ISimpleCollectionFactory
    {
        ISimpleCollection<T> Create<T>(uint initialCollectionSize);
    }
}