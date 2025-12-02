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
    /// A high-performance Binary Heap implementation.
    /// <para>
    /// <b>Default Behavior:</b> Acts as a <b>Min-Heap</b> (smallest item is popped first) 
    /// unless a custom comparer is provided.
    /// </para>
    /// </summary>
    public sealed class SfBinaryHeap<T> : ISfDataStructure<T>
    {
        private const int DefaultCapacity = 16;
        private T[] _buffer;
        private readonly IComparer<T> _comparer;
        private int _count;

        /// <inheritdoc/>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count == 0;
        }

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer.Length;
        }

        /// <summary>
        /// Creates a new heap with the specified capacity, growth factor, and comparer.
        /// </summary>
        public SfBinaryHeap(int capacity = DefaultCapacity, IComparer<T> comparer = null)
        {
            _buffer = new T[capacity];
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
            _count = 0;
        }

        /// <summary>
        /// Creates a heap from an existing collection.
        /// </summary>
        public SfBinaryHeap(IEnumerable<T> items, IComparer<T> comparer = null)
        {
            if (items is null)
                SfThrowHelper.ThrowArgumentNull(nameof(items));

            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
            
            if (items is ICollection<T> col)
            {
                _buffer = new T[col.Count];
                col.CopyTo(_buffer, 0);
                _count = col.Count;
            }
            else
            {
                _buffer = new T[DefaultCapacity];
                foreach (var item in items) Add(item); // Fallback
            }
            
            for (int i = (_count >> 1) - 1; i >= 0; i--)
                HeapifyDown(i);
        }

        /// <summary>Copy constructor.</summary>
        public SfBinaryHeap(SfBinaryHeap<T> other)
        {
            _comparer = other._comparer;
            _count = other._count;
            _buffer = new T[other._buffer.Length];
            Array.Copy(other._buffer, _buffer, other._buffer.Length);
        }
        
        /// <summary>
        /// Creates a Min-Heap (Smallest item first).
        /// </summary>
        public static SfBinaryHeap<T> CreateMinHeap(int capacity = DefaultCapacity)
        {
            return new SfBinaryHeap<T>(capacity, SfComparers<T>.DefaultComparer);
        }

        /// <summary>
        /// Creates a Max-Heap (Largest item first).
        /// </summary>
        public static SfBinaryHeap<T> CreateMaxHeap(int capacity = DefaultCapacity)
        {
            return new SfBinaryHeap<T>(capacity, SfComparers<T>.ReverseComparer);
        }

        /// <summary>
        /// Returns the element at the top of the heap without removing it.
        /// </summary>
        /// <returns>The top element of the heap.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the heap is empty.</exception>
        public T Peek()
        {
            if (IsEmpty) 
                SfThrowHelper.ThrowInvalidOperation("Heap is empty");
            return _buffer[0];
        }

        /// <summary>
        /// Removes and returns the element at the top of the heap.
        /// </summary>
        /// <returns>The removed top element.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the heap is empty.</exception>
        public T Pop()
        {
            if (IsEmpty) 
                SfThrowHelper.ThrowInvalidOperation("Heap is empty");

            T item = _buffer[0];
            int lastIndex = --_count;
            _buffer[0] = _buffer[lastIndex];
            _buffer[lastIndex] = default;

            if (_count > 0) 
                HeapifyDown(0);
            return item;
        }

        /// <summary>
        /// Adds an item to the heap while maintaining the heap property.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            if (Capacity == _count) Resize();
            _buffer[_count] = item;
            HeapifyUp(_count);
            _count++;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Resize()
        {
            T[] newBuffer = new T[_buffer.Length * 2];
            if (_count > 0)
            {
                Array.Copy(_buffer, 0, newBuffer, 0, _count);
            }
            _buffer = newBuffer;
        }
        
        /// <inheritdoc/>
        public void Clear()
        {
            Array.Clear(_buffer, 0, _count);
            _count = 0;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");

            Array.Copy(_buffer, 0, array, arrayIndex, _count);
        }

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[_count];
            CopyTo(arr, 0);
            return arr;
        }

        /// <summary>Performs the heapify-down operation from a given index.</summary>
        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = (index << 1) + 1;
                int right = left + 1;
                int smallest = index;

                if (left < _count && _comparer.Compare(_buffer[left], _buffer[smallest]) < 0)
                    smallest = left;
                if (right < _count && _comparer.Compare(_buffer[right], _buffer[smallest]) < 0)
                    smallest = right;
                if (smallest == index) break;

                (_buffer[index], _buffer[smallest]) = (_buffer[smallest], _buffer[index]);
                index = smallest;
            }
        }
        
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) >> 1;
                if (_comparer.Compare(_buffer[index], _buffer[parentIndex]) >= 0)
                    break;
                
                (_buffer[index], _buffer[parentIndex]) = (_buffer[parentIndex], _buffer[index]);
                index = parentIndex;
            }
        }
        
        /// <summary>
        /// Returns the underlying data array as span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => _buffer.AsSpan();
        
        /// <summary>
        /// Returns the underlying data array as readonly span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsReadOnlySpan() => new ReadOnlySpan<T>(_buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfArrayEnumerator<T> GetEnumerator() => new(_buffer, _count);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < _count; ++i)
                action(_buffer[i]);
        }

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < _count; ++i)
            {
                if (comparer.Equals(_buffer[i], item))
                    return true;
            }
            return false;
        }
    }

}