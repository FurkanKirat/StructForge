using System;
using System.Collections;
using System.Collections.Generic;
using StructForge.Comparers;

namespace StructForge.Collections
{
    /// <summary>
    /// A priority queue that stores items with associated priorities.
    /// Supports min-heap or max-heap behavior.
    /// </summary>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    /// <typeparam name="TPriority">Type of the priority values.</typeparam>
    public class SfPriorityQueue<TItem, TPriority> : ISfDataStructure<TItem>
    {
        private readonly SfBinaryHeap<(TItem item, TPriority priority)> _heap;
        private readonly bool _isMinHeap;

        /// <inheritdoc/>
        public int Count => _heap.Count;

        /// <inheritdoc/>
        public bool IsEmpty => _heap.IsEmpty;

        /// <summary>
        /// Creates a priority queue with optional comparer and min/max behavior.
        /// </summary>
        public SfPriorityQueue(IComparer<TPriority> comparer = null, bool minHeap = true)
        {
            comparer ??= Comparer<TPriority>.Default;
            var tupleComparer = 
                Comparer<(TItem item, TPriority priority)>.Create((a, b) => 
                    comparer.Compare(a.priority, b.priority));

            if (minHeap)
            {
                _heap = new SfMinHeap<(TItem item, TPriority priority)>(comparer: tupleComparer);
            }
            else
            {
                _heap = new SfMaxHeap<(TItem item, TPriority priority)>(comparer: tupleComparer);
            }
            _isMinHeap = minHeap;
            
        }

        /// <summary>Adds an item with a priority.</summary>
        public void Enqueue(TItem item, TPriority priority)
            => _heap.Add((item, priority));

        /// <summary>Removes and returns the item with the highest/lowest priority depending on heap type.</summary>
        public TItem Dequeue() => _heap.Pop().item;

        /// <summary>Returns the item with the highest/lowest priority without removing it.</summary>
        public TItem Peek() => _heap.Peek().item;

        /// <inheritdoc/>
        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var (item, _) in _heap)
                yield return item;
        }
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates items in priority order without modifying the queue.</summary>
        public IEnumerable<TItem> EnumerateByPriority()
        {
            SfBinaryHeap<(TItem item, TPriority priority)> copy;
            if (_isMinHeap)
                copy = new SfMinHeap<(TItem item, TPriority priority)>(_heap);
            else 
                copy = new SfMaxHeap<(TItem item, TPriority priority)>(_heap);
            while (!copy.IsEmpty)
                yield return copy.Pop().item;
        }

        /// <inheritdoc/>
        public void ForEach(Action<TItem> action)
        {
            foreach (var (item, _) in _heap)
                action(item);
        }

        /// <inheritdoc/>
        public bool Contains(TItem item) => Contains(item, EqualityComparer<TItem>.Default);

        /// <inheritdoc/>
        public bool Contains(TItem item, IEqualityComparer<TItem> comparer)
        {
            comparer ??= SfEqualityComparers<TItem>.Default;

            foreach (var (heapItem, _) in _heap)
            {
                if (comparer.Equals(heapItem, item))
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void Clear() => _heap.Clear();

        /// <inheritdoc/>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array too small");

            foreach (var (item, _) in _heap)
                array[arrayIndex++] = item;
        }
        
    }

    
}