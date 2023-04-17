/*
Copyright (c) Luchunpen.
Date: 28/03/2023
*/

using System;
using System.Collections.Generic;

namespace Nano3.Collection
{
    internal class HeapMin<TValue>
        where TValue: IComparable<TValue>
    {
        //private static readonly string stringUID = "FB88DAB184857801";

        public List<TValue> _items;
        public int Count { get { return _items.Count; } }

        public HeapMin() : this(4) { }
        public HeapMin(int capacity)
        {
            if (capacity <= 0) { throw new ArgumentOutOfRangeException("Need capacity > 0"); }
            _items = new List<TValue>(capacity);
        }

        public void Add(TValue item)
        {
            _items.Add(item);
            int indexChild = _items.Count - 1;
            int indexParent = (indexChild - 1) / 2;

            TValue child = _items[indexChild];
            TValue parent = _items[indexParent];

            TValue temp;

            while (indexChild > 0 && parent.CompareTo(child) > 0)
            {
                temp = child; _items[indexChild] = parent; _items[indexParent] = temp;

                indexChild = indexParent;
                indexParent = (indexChild - 1) / 2;

                child = _items[indexChild];
                parent = _items[indexParent];
            }
        }
        public TValue GetBest()
        {
            int count = _items.Count - 1;
            if (count < 0) { throw new ArgumentOutOfRangeException(); }
            if (count == 0) { TValue result = _items[0]; _items.RemoveAt(0); return result; }

            TValue res = _items[0]; _items[0] = _items[count]; _items.RemoveAt(count);
            Heapify(0);

            return res;

        }

        public void Heapify(int i)
        {
            int leftIndex, rightIndex;
            int transIndex = i;

            int count = _items.Count;

            TValue left;
            TValue right;
            TValue trans;

            while (true)
            {
                leftIndex = 2 * i + 1;
                rightIndex = 2 * i + 2;

                trans = _items[transIndex];

                if (leftIndex < count)
                {
                    left = _items[leftIndex];
                    if (left.CompareTo(trans) < 0)
                    {
                        transIndex = leftIndex;
                        trans = left;
                    }
                }

                if (rightIndex < count)
                {
                    right = _items[rightIndex];
                    if (right.CompareTo(trans) < 0)
                    {
                        transIndex = rightIndex;
                        trans = right;
                    }
                }

                if (transIndex == i) break;

                TValue temp = _items[i]; _items[i] = trans; _items[transIndex] = temp;
                i = transIndex;
            }
        }
        public void Clear()
        {
            _items = new List<TValue>();
        }
    }
}
