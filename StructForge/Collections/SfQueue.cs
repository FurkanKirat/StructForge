using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Enumerators;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// A circular array-based queue implementation similar to <see cref="System.Collections.Generic.Queue{T}"/>.
    /// Supports Enqueue, Dequeue, Peek, TryPeek, ToArray, Contains, Clear, and enumeration.
    /// Additional methods: TrimExcess, PeekLast, TryPeekLast, ForEach.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the queue.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(SfQueueDebugView<>))]
    public sealed class SfQueue<T> : ISfQueue<T>
    {
        private const int DefaultCapacity = 4;
        private const float DefaultGrowthFactor = 2f;
        private const float MinGrowthFactor = 1.5f;

        private T[] _buffer;
        private readonly float _growthFactor;
        private int _head, _tail, _count;

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
        /// Capacity of the underlying array.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer.Length;
        }
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SfQueue{T}"/> class with the specified capacity and growth factor.
        /// </summary>
        /// <param name="capacity">Initial capacity of the queue.</param>
        /// <param name="growthFactor">Growth factor used when resizing the underlying array.</param>
        public SfQueue(
            int capacity = DefaultCapacity,
            float growthFactor = DefaultGrowthFactor)
        {
            _buffer = new T[capacity];
            _growthFactor = Math.Max(growthFactor, MinGrowthFactor);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfQueue{T}"/> class from an existing collection.
        /// </summary>
        /// <param name="enumerable">The collection to copy elements from.</param>
        /// <param name="extraCapacity">Additional capacity to allocate beyond the initial elements.</param>
        /// <param name="growthFactor">Growth factor used when resizing.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumerable"/> is null.</exception>
        public SfQueue(
            IEnumerable<T> enumerable, 
            int extraCapacity = 0, 
            float growthFactor = DefaultGrowthFactor)
        {
            if (enumerable is null)
                SfThrowHelper.ThrowArgumentNull(nameof(enumerable));
            
            if (enumerable is ICollection<T> collection)
            {
                _buffer = new T[collection.Count + extraCapacity];
                collection.CopyTo(_buffer, 0);
                _count = collection.Count;
            }
            else
            {
                T[] array = enumerable.ToArray();
                _count = array.Length;
                _buffer = new T[extraCapacity + array.Length];
                Array.Copy(array, _buffer, _count);
            }

            _head = 0;
            _tail = _count == _buffer.Length ? 0 : _count;
            _growthFactor = Math.Max(growthFactor, MinGrowthFactor);
        }

        #endregion

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
            if (_count == 0) return;

            if (_head < _tail)
            {
                for (int i = _head; i < _tail; i++) action(_buffer[i]);
            }
            else
            {
                for (int i = _head; i < _buffer.Length; i++) action(_buffer[i]);
                for (int i = 0; i < _tail; i++) action(_buffer[i]);
            }
        }

        #endregion

        #region Core Methods

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T item)
        {
            if (_count == _buffer.Length)
                GrowIfNeeded();
            
            _buffer[_tail] = item;
            _tail++;
            if (_tail == _buffer.Length)
            {
                _tail = 0;
            }

            _count++;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Dequeue()
        {
            if (TryDequeue(out T item)) return item;
            SfThrowHelper.ThrowInvalidOperation("Queue is empty");
            return default;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            if (TryPeek(out T item)) return item;
            SfThrowHelper.ThrowInvalidOperation("Queue is empty");
            return default;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out T item)
        {
            if (_count == 0)
            {
                item = default;
                return false;
            }

            item = _buffer[_head];
            _buffer[_head] = default;
            _head++;
            if (_head == _buffer.Length)
            {
                _head = 0;
            }
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

        /// <inheritdoc/>
        public void Clear()
        {
            if (_head < _tail)
            {
                Array.Clear(_buffer, _head, _count);
            }
            else
            {
                Array.Clear(_buffer, _head, _buffer.Length - _head);
                Array.Clear(_buffer, 0, _tail);
            }
            
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            comparer ??= SfEqualityComparers<T>.Default;
            if (_head < _tail)
            {
                for (int i = _head; i < _tail; i++)
                {
                    if (comparer.Equals(_buffer[i], item))
                        return true;
                }
            }
            else
            {
                for (int i = _head; i < _buffer.Length; i++)
                {
                    if (comparer.Equals(_buffer[i], item))
                        return true;
                }
                
                for (int i = 0; i < _tail; i++)
                {
                    if (comparer.Equals(_buffer[i], item))
                        return true;
                }
            }
            return false;
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

            if (_count == 0)
                return;
            
            if (_head < _tail)
            {
                Array.Copy(_buffer, _head, array, arrayIndex, _count);
            }
            else
            {
                int headChunk = _buffer.Length - _head;
                Array.Copy(_buffer, _head, array, arrayIndex, headChunk);
                Array.Copy(_buffer, 0, array, arrayIndex + headChunk, _tail);
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

        #region Additional Methods

        /// <summary>Reduces the underlying array capacity to fit the current count.</summary>
        public void TrimExcess()
        {
            if (_count < _buffer.Length * 0.9)
            {
                T[] newArray = new T[_count];
                if (_count > 0)
                {
                    if (_head < _tail)
                    {
                        Array.Copy(_buffer, _head, newArray, 0, _count);
                    }
                    else
                    {
                        Array.Copy(_buffer, _head, newArray, 0, _buffer.Length - _head);
                        Array.Copy(_buffer, 0, newArray, _buffer.Length - _head, _tail);
                    }
                }
                _buffer = newArray;
                _head = 0;
                _tail = (_count == newArray.Length) ? 0 : _count;
            }
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekLast()
        {
            if (_count == 0) 
                SfThrowHelper.ThrowInvalidOperation("Queue is empty");
            
            int index = _tail == 0 ? _buffer.Length - 1 : _tail - 1;
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
            
            int index = _tail == 0 ? _buffer.Length - 1 : _tail - 1;
            item = _buffer[index];
            return true;
        }

        #endregion

        #region Internal Helpers

        /// <summary>
        /// Ensures the internal array has enough capacity to hold the specified number of elements.
        /// Grows the array if needed, preserving existing elements in order.
        /// </summary>
        private void GrowIfNeeded()
        {
            int newCapacity = (int)(_buffer.Length * _growthFactor);
            T[] newArray = new T[newCapacity];

            if (_count > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_buffer, _head, newArray, 0, _count);
                }
                else
                {
                    Array.Copy(_buffer, _head, newArray, 0, _buffer.Length - _head);
                    Array.Copy(_buffer, 0, newArray, _buffer.Length - _head, _tail);
                }
            }

            _buffer = newArray;
            _head = 0;
            _tail = _count;
        }

        #endregion
        
        private string DebuggerDisplay => $"SfQueue<{typeof(T).Name}> (Count = {Count})";
    }
    
    internal sealed class SfQueueDebugView<T>
    {
        private readonly SfQueue<T> _sfQueue;
        public SfQueueDebugView(SfQueue<T> sfQueue) => _sfQueue = sfQueue;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _sfQueue.ToArray();
    }
}
