using System;
using System.Collections.Generic;

namespace Yak2D.Utility
{
    public class ArrayComparer<T> : IComparer<int> where T : IComparable
    {
        private readonly bool _reverse;
        public ArrayComparer(bool reverse)
        {
            _reverse = reverse;
        }

        public void SetItems(T[] items)
        {
            _items = items ?? throw new Yak2DException("Internal Framework Exception: Array Comparer object was passed a null list", new ArgumentNullException("items"));
        }
        private T[] _items;

        //Used by sorting algorithms
        //Array out of bounds checks not made as assume algorithim correctness (and for speed)
        public int Compare(int left, int right)
        {
            if (_reverse)
            {
                return _items[right].CompareTo(_items[left]);
            }
            else
            {
                return _items[left].CompareTo(_items[right]);
            }
        }
    }
}