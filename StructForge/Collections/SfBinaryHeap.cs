using System;
using System.Collections;
using System.Collections.Generic;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a generic binary heap data structure.
    /// Supports add, pop, peek, and heapify operations.
    /// Can be used as a min-heap or max-heap depending on the comparer.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the heap.</typeparam>
    public abstract class SfBinaryHeap<T> : ISfHeap<T>
    {
        private readonly SfList<T> _data;
        private readonly IComparer<T> _comparer;

        /// <inheritdoc/>
        public int Count => _data.Count;

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Creates a new heap with the specified capacity, growth factor, and comparer.
        /// </summary>
        public SfBinaryHeap(int capacity = SfList<T>.DefaultCapacity,
                            float growthFactor = SfList<T>.DefaultGrowthFactor,
                            IComparer<T> comparer = null)
        {
            _data = new SfList<T>(capacity, growthFactor);
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
        }

        /// <summary>
        /// Creates a heap from an existing collection.
        /// </summary>
        public SfBinaryHeap(IEnumerable<T> items, IComparer<T> comparer = null)
        {
            SfThrowHelper.ThrowIfNull(items);
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
            _data = new SfList<T>();
            foreach (var item in items)
                _data.Add(item);

            for (int i = (_data.Count - 2) / 2; i >= 0; i--)
                HeapifyDown(i);
        }

        /// <summary>Copy constructor.</summary>
        public SfBinaryHeap(SfBinaryHeap<T> other)
        {
            _comparer = other._comparer;
            _data = new SfList<T>(other._data);
        }

        /// <inheritdoc/>
        public T Peek()
        {
            if (IsEmpty) throw new InvalidOperationException("Heap is empty");
            return _data[0];
        }

        /// <inheritdoc/>
        public T Pop()
        {
            if (IsEmpty) throw new InvalidOperationException("Heap is empty");

            T item = _data[0];
            int lastIndex = Count - 1;
            _data[0] = _data[lastIndex];
            _data.RemoveAt(lastIndex);

            HeapifyDown(0);
            return item;
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            _data.Add(item);
            int index = _data.Count - 1;
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (_comparer.Compare(_data[index], _data[parentIndex]) <= 0)
                    break;
                _data.Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        /// <inheritdoc/>
        public void Clear() => _data.Clear();

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array is not large enough.");

            for (int i = 0; i < Count; ++i)
                array[arrayIndex + i] = _data[i];
        }

        /// <summary>Performs the heapify-down operation from a given index.</summary>
        private void HeapifyDown(int index)
        {
            int count = _data.Count;
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int largest = index;

                if (left < count && _comparer.Compare(_data[left], _data[largest]) > 0)
                    largest = left;
                if (right < count && _comparer.Compare(_data[right], _data[largest]) > 0)
                    largest = right;
                if (largest == index) break;

                _data.Swap(index, largest);
                index = largest;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
                yield return _data[i];
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; ++i)
                action(_data[i]);
        }

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (comparer.Equals(_data[i], item))
                    return true;
            }
            return false;
        }
    }

    /// <summary>Represents a max-heap implementation.</summary>
    public sealed class SfMaxHeap<T> : SfBinaryHeap<T>
    {
        public SfMaxHeap(int capacity = SfList<T>.DefaultCapacity, 
            float growthFactor = SfList<T>.DefaultGrowthFactor, 
            IComparer<T> comparer = null)
            : base(capacity, growthFactor, comparer) { }

        public SfMaxHeap(IEnumerable<T> items, IComparer<T> comparer = null) : base(items, comparer) { }

        public SfMaxHeap(SfBinaryHeap<T> heap) : base(heap) { }
    }

    /// <summary>Represents a min-heap implementation.</summary>
    public sealed class SfMinHeap<T> : SfBinaryHeap<T>
    {
        public SfMinHeap(int capacity = SfList<T>.DefaultCapacity, 
            float growthFactor = SfList<T>.DefaultGrowthFactor,
            IComparer<T> comparer = null)
            : base(capacity, growthFactor, comparer.Reverse()) { }

        public SfMinHeap(IEnumerable<T> items, IComparer<T> comparer = null) :
            base(items, comparer.Reverse()) { }

        public SfMinHeap(SfBinaryHeap<T> heap) : base(heap) { }
    }

}