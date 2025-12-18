using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Enumerators;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Fixed-size, circular FIFO (First-In-First-Out) queue.
    /// Supports overwrite when full or TryEnqueue for overflow-safe insertion.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(SfRingBufferDebugView<>))]
    public sealed class SfRingBuffer<T> : ISfQueue<T>
    {
        private readonly T[] _buffer; // The underlying fixed-size array storing elements
        private int _head; // Index of the first element in the queue (front)
        private int _tail;
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

        /// <summary>
        /// Checks if the Ring Buffer is full or not.
        /// </summary>
        public bool IsFull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count == Capacity;
        }

        /// <summary>
        /// Capacity of the underlying buffer.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer.Length;
        }

        /// <summary>
        /// Initializes a new ring buffer with the specified capacity.
        /// </summary>
        /// <param name="capacity">Maximum number of elements the buffer can hold.</param>
        public SfRingBuffer(int capacity)
        {
            _buffer = new T[capacity];
        }

        #region Enumeration

        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfCircularQueueEnumerator<T> GetEnumerator() => new(_buffer, _head, _count);
        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < _count; i++)
                action(_buffer[(_head + i) % Capacity]);
        }

        #endregion

        #region Search & Utility Methods

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < _count; i++)
            {
                int index = (_head + i) % Capacity;
                if (comparer.Equals(_buffer[index], item)) return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            if (!IsEmpty)
            {
                if (_head < _tail)
                {
                    Array.Clear(_buffer, _head, _count);
                }
                else
                {
                    Array.Clear(_buffer, _head, Capacity - _head);
                    Array.Clear(_buffer, 0, _tail);
                }
            }
            
            _head = 0;
            _tail = 0;
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

            if (IsEmpty) return;
            
            if (_head < _tail)
                Array.Copy(_buffer, _head, array, arrayIndex, _count);
            else
            {
                int firstPart = Capacity - _head;
                Array.Copy(_buffer, _head, array, arrayIndex, firstPart);
                Array.Copy(_buffer, 0, array, arrayIndex + firstPart, _tail);
            }
                
        }

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[_count];
            CopyTo(arr, 0);
            return arr;
        }

        #endregion

        #region Enqueue Methods

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T item)
        {
            _buffer[_tail] = item;
            int nextTail = _tail + 1;
            if (nextTail == Capacity) nextTail = 0;
            _tail = nextTail;
            
            if (IsFull) 
            {
                // Overwrite the oldest element and move head forward
                int nextHead = _head + 1;
                if (nextHead == Capacity) nextHead = 0;
                _head = nextHead;
            }
            else
            {
                // Add normally to the end
                _count++;
            }
        }

        /// <summary>
        /// Attempts to add an item without overwriting.
        /// Returns false if the queue is full.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryEnqueue(T item)
        {
            if (_count == Capacity) return false;
            _buffer[_tail] = item;
            
            int nextTail = _tail + 1;
            if (nextTail == Capacity) nextTail = 0;
            _tail = nextTail;
            _count++;
            return true;
        }

        #endregion

        #region Dequeue & Peek Methods

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Dequeue()
        {
            if (IsEmpty)
                SfThrowHelper.ThrowInvalidOperation("Queue is empty");
            
            T item = _buffer[_head];
            _buffer[_head] = default;

            int nextHead = _head + 1;
            if (nextHead == Capacity) nextHead = 0;
            _head = nextHead;

            _count--;
            return item;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            if (IsEmpty)
                SfThrowHelper.ThrowInvalidOperation("Queue is empty");
            
            return _buffer[_head];
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            item = _buffer[_head];
            _buffer[_head] = default;
            int nextHead = _head + 1;
            if (nextHead == Capacity) nextHead = 0;
            _head = nextHead;
            
            _count--;
            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekLast()
        {
            if (IsEmpty) 
                SfThrowHelper.ThrowInvalidOperation("Queue is empty");
            int index = _tail - 1;
            if (index < 0) 
            {
                index = Capacity - 1;
            }

            return _buffer[index];
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeekLast(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            int index = _tail - 1;
            if (index < 0) 
            {
                index = Capacity - 1;
            }

            item = _buffer[index];
            return true;
        }

        #endregion
        
        private string DebuggerDisplay => $"SfRingBuffer<{typeof(T).Name}> (Capacity = {Capacity}, Count = {Count})";
    }
    
    internal class SfRingBufferDebugView<T>
    {
        private readonly SfRingBuffer<T> _sfRingBuffer;
        public SfRingBufferDebugView(SfRingBuffer<T> sfRingBuffer) => _sfRingBuffer = sfRingBuffer;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _sfRingBuffer.ToArray();
    }
}
