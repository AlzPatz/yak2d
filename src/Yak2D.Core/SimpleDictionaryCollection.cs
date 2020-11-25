using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class SimpleDictionaryCollection<T> : ISimpleCollection<T>
    {
        //Just a dictionary wrapper
        //Added to enable possible future replacement of collection type and simplify access / provide some debug messaging

        private readonly IFrameworkMessenger _frameworkMessenger;

        public int Count { get { return _items.Count; } }

        private Dictionary<ulong, T> _items;

        public SimpleDictionaryCollection(IFrameworkMessenger frameworkMessenger, uint initialSize)
        {
            _frameworkMessenger = frameworkMessenger;
            _items = new Dictionary<ulong, T>((int)initialSize);
        }

        public bool Add(ulong id, T item)
        {
            if (_items.ContainsKey(id))
            {
                _frameworkMessenger.Report("Unable to add Type: " + typeof(T) + " to collection");
                return false;
            }

            _items.Add(id, item);
            return true;
        }

        public bool Remove(ulong id)
        {
            if (!_items.ContainsKey(id))
            {
                _frameworkMessenger.Report("Unable to remove Type: " + typeof(T) + " from collection");
                return false;
            }

            _items.Remove(id);
            return true;
        }

        public void RemoveAll()
        {
            _items.Clear();
        }

        public T Retrieve(ulong id)
        {
            T item;
            if (_items.TryGetValue(id, out item))
            {
                return item;
            }
            else
            {
                _frameworkMessenger.Report("Simple Collection: Unable to retrieve type: " + typeof(T) + " as ulong does not exist in collection");
                return default(T);
            }
        }

        public IEnumerable<T> Iterate()
        {
            foreach (var item in _items)
            {
                yield return item.Value;
            }
        }

        public bool Contains(ulong id)
        {
            return _items.ContainsKey(id);
        }

        public List<ulong> ReturnAllIds()
        {
            return new List<ulong>(_items.Keys);
        }
    }
}