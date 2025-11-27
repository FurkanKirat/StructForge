using System;
using System.Collections;
using System.Collections.Generic;
using StructForge.Comparers;

namespace StructForge.Collections
{
    /// <summary>
    /// Fixed-size, circular FIFO (First-In-First-Out) queue.
    /// Supports overwrite when full or TryEnqueue for overflow-safe insertion.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    public class SfRingBuffer<T> : ISfQueue<T>
    {
        private readonly T[] _buffer; // The underlying fixed-size array storing elements
        private int _head;            // Index of the first element in the queue (front)

        /// <inheritdoc/>
        public int Count { get; private set; }

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Capacity of the underlying buffer.
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        /// Initializes a new ring buffer with the specified capacity.
        /// </summary>
        /// <param name="capacity">Maximum number of elements the buffer can hold.</param>
        public SfRingBuffer(int capacity)
        {
            _buffer = new T[capacity];
        }

        #region Enumeration

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return _buffer[(_head + i) % Capacity];
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
                action(_buffer[(_head + i) % Capacity]);
        }

        #endregion

        #region Search & Utility Methods

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < Count; i++)
            {
                int index = (_head + i) % Capacity;
                if (comparer.Equals(_buffer[index], item)) return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                _buffer[(_head + i) % Capacity] = default;
            _head = 0;
            Count = 0;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array too small");

            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = _buffer[(_head + i) % Capacity];
        }

        #endregion

        #region Enqueue Methods

        /// <inheritdoc/>
        public void Enqueue(T item)
        {
            if (Count == Capacity) 
            {
                // Overwrite the oldest element and move head forward
                _buffer[_head] = item;
                _head = (_head + 1) % Capacity;
            }
            else
            {
                // Add normally to the end
                _buffer[(_head + Count) % Capacity] = item;
                Count++;
            }
        }

        /// <summary>
        /// Attempts to add an item without overwriting.
        /// Returns false if the queue is full.
        /// </summary>
        public bool TryEnqueue(T item)
        {
            if (Count == Capacity) return false;
            _buffer[(_head + Count) % Capacity] = item;
            Count++;
            return true;
        }

        #endregion

        #region Dequeue & Peek Methods

        /// <inheritdoc/>
        public T Dequeue()
        {
            if (TryDequeue(out T item)) return item;
            throw new InvalidOperationException("Queue is empty");
        }

        /// <inheritdoc/>
        public T Peek()
        {
            if (TryPeek(out T item)) return item;
            throw new InvalidOperationException("Queue is empty");
        }

        /// <inheritdoc/>
        public bool TryDequeue(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            item = _buffer[_head];
            _buffer[_head] = default;
            _head = (_head + 1) % Capacity;
            Count--;
            return true;
        }

        /// <inheritdoc/>
        public bool TryPeek(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }
            item = _buffer[_head];
            return true;
        }

        #endregion

        #region Last Item Access

        /// <inheritdoc/>
        public T PeekLast()
        {
            if (TryPeekLast(out T item)) return item;
            throw new InvalidOperationException("Queue is empty");
        }

        /// <inheritdoc/>
        public bool TryPeekLast(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }
            int lastIndex = (_head + Count - 1) % Capacity;
            item = _buffer[lastIndex];
            return true;
        }

        #endregion
    }
}
