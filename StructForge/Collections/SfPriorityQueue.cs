#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;

namespace StructForge.Collections
{
    /// <summary>
    /// A priority queue that stores items with associated priorities.
    /// Supports min-heap or max-heap behavior.
    /// </summary>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    /// <typeparam name="TPriority">Type of the priority values.</typeparam>
    public class SfPriorityQueue<TItem, TPriority> : IDataStructure<TItem>
    {
        private readonly SfBinaryHeap<(TItem item, TPriority priority)> _heap;

        /// <summary>Number of items in the queue.</summary>
        public int Count => _heap.Count;

        /// <summary>Returns true if the queue has no elements.</summary>
        public bool IsEmpty => _heap.IsEmpty;

        /// <summary>
        /// Creates a priority queue with optional comparer and min/max behavior.
        /// </summary>
        public SfPriorityQueue(IComparer<TPriority> comparer = null, bool minHeap = true)
        {
            comparer ??= Comparer<TPriority>.Default;
            var tupleComparer = Comparer<(TItem item, TPriority priority)>.Create((a, b) =>
                minHeap
                    ? comparer.Compare(b.priority, a.priority)
                    : comparer.Compare(a.priority, b.priority)
            );
            _heap = new SfBinaryHeap<(TItem item, TPriority priority)>(comparer: tupleComparer);
        }

        /// <summary>Adds an item with a priority.</summary>
        public void Enqueue(TItem item, TPriority priority)
            => _heap.Add((item, priority));

        /// <summary>Removes and returns the item with the highest/lowest priority depending on heap type.</summary>
        public TItem Dequeue() => _heap.Pop().item;

        /// <summary>Returns the item with the highest/lowest priority without removing it.</summary>
        public TItem Peek() => _heap.Peek().item;

        /// <summary>Enumerates all items in arbitrary order.</summary>
        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var (item, _) in _heap)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates items in priority order without modifying the queue.</summary>
        public IEnumerable<TItem> EnumerateByPriority()
        {
            var copy = new SfBinaryHeap<(TItem item, TPriority priority)>(_heap);
            while (!copy.IsEmpty)
                yield return copy.Pop().item;
        }

        /// <summary>Executes an action for each item in the queue.</summary>
        public void ForEach(Action<TItem> action)
        {
            foreach (var (item, _) in _heap)
                action(item);
        }

        /// <summary>Returns true if the queue contains the item using the default comparer.</summary>
        public bool Contains(TItem item) => Contains(item, EqualityComparer<TItem>.Default);

        /// <summary>Returns true if the queue contains the item using a custom comparer.</summary>
        public bool Contains(TItem item, IEqualityComparer<TItem> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            foreach (var (heapItem, _) in _heap)
            {
                if (comparer.Equals(heapItem, item))
                    return true;
            }
            return false;
        }

        /// <summary>Clears all items from the queue.</summary>
        public void Clear() => _heap.Clear();

        /// <summary>Copies the items to an array starting at the specified index.</summary>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array too small");

            foreach (var (item, _) in _heap)
                array[arrayIndex++] = item;
        }

        /// <summary>Returns all items as an array.</summary>
        public TItem[] ToArray()
        {
            if (IsEmpty) return Array.Empty<TItem>();

            TItem[] array = new TItem[Count];
            CopyTo(array, 0);
            return array;
        }
    }

    
}