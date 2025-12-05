using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Enumerators;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// A high-performance Priority Queue backed by a Binary Heap.
    /// Items are dequeued based on their priority.
    /// </summary>
    public sealed class SfPriorityQueue<TItem, TPriority> : ISfDataStructure<TItem>
    {
        private readonly SfBinaryHeap<(TItem item, TPriority priority)> _heap;

        /// <inheritdoc/>
        public int Count 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _heap.Count;
        }

        /// <inheritdoc/>
        public bool IsEmpty 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _heap.IsEmpty;
        }

        /// <summary>
        /// Creates a priority queue with optional comparer and min/max behavior.
        /// </summary>
        public SfPriorityQueue(int capacity = 16, IComparer<TPriority> comparer = null)
        {
            comparer ??= Comparer<TPriority>.Default;
            var tupleComparer = 
                Comparer<(TItem item, TPriority priority)>.Create((a, b) => 
                    comparer.Compare(a.priority, b.priority));

            _heap = new SfBinaryHeap<(TItem item, TPriority priority)>(capacity, tupleComparer);
        }

        /// <summary>Adds an item with a priority.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(TItem item, TPriority priority) => _heap.Add((item, priority));

        /// <summary>Removes and returns the item with the highest/lowest priority depending on heap type.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TItem Dequeue() => _heap.Pop().item;

        /// <summary>Returns the item with the highest/lowest priority without removing it.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TItem Peek() => _heap.Peek().item;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out TItem item, out TPriority priority)
        {
            if (_heap.Count > 0)
            {
                var result = _heap.Pop();
                item = result.item;
                priority = result.priority;
                return true;
            }
            item = default;
            priority = default;
            return false;
        }

        
        public struct SfPqEnumerator : IEnumerator<TItem>
        {
            private SfArrayEnumerator<(TItem item, TPriority priority)> _heapEnumerator;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfPqEnumerator(SfBinaryHeap<(TItem, TPriority)> heap)
            {
                _heapEnumerator = heap.GetEnumerator();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _heapEnumerator.MoveNext();

            public TItem Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _heapEnumerator.Current.item;
            }

            object IEnumerator.Current => Current;
            public void Reset() => _heapEnumerator.Reset();
            public void Dispose() => _heapEnumerator.Dispose();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfPqEnumerator GetEnumerator() => new(_heap);

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Destructively enumerates elements in priority order.
        /// <b>Warning:</b> This method creates a copy of the heap (Allocation: O(N)).
        /// </summary>
        public IEnumerable<TItem> EnumerateByPriority()
        {
            SfBinaryHeap<(TItem item, TPriority priority)> copy =
                new SfBinaryHeap<(TItem item, TPriority priority)>(_heap);
            
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => _heap.Clear();

        /// <inheritdoc/>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");

            foreach (var (item, _) in _heap)
                array[arrayIndex++] = item;
        }

        /// <inheritdoc/>
        public TItem[] ToArray()
        {
            TItem[] arr = new TItem[Count];
            CopyTo(arr, 0);
            return arr;
        }

    }
}