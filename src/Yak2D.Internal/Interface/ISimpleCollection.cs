using System.Collections.Generic;

namespace Yak2D.Internal
{
    public interface ISimpleCollection<T>
    {
        int Count { get; }
        bool Add(ulong id, T item);
        bool Remove(ulong id);
        void RemoveAll();
        bool Contains(ulong id);
        T Retrieve(ulong id);
        IEnumerable<T> Iterate();
        List<ulong> ReturnAllIds();
    }
}