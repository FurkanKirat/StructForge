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
        private int _head, _count;

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

        /// <summary>
        /// Gets the index of the first empty slot in the circular array.
        /// </summary>
        private int FirstEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_head + _count) % Capacity;
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
            for (int i = 0; i < _count; i++)
                action(_buffer[(_head + i) % Capacity]);
        }

        #endregion

        #region Core Methods

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T item)
        {
            GrowIfNeeded(_count + 1);
            _buffer[FirstEmpty] = item;
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
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            item = _buffer[_head];
            _buffer[_head] = default;
            _head = (_head + 1) % Capacity;
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
            for (int i = 0; i < _count; i++)
                _buffer[(_head + i) % Capacity] = default;
            _head = 0;
            _count = 0;
        }

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
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");

            for (int i = 0; i < _count; i++)
                array[arrayIndex + i] = _buffer[(_head + i) % Capacity];
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
            if (_count < Capacity)
            {
                T[] newArray = new T[_count];
                for (int i = 0; i < _count; i++)
                    newArray[i] = _buffer[(_head + i) % Capacity];
                _buffer = newArray;
                _head = 0;
            }
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekLast()
        {
            if (IsEmpty) 
                SfThrowHelper.ThrowInvalidOperation("Queue is empty");
            
            int lastIndex = (_head + _count - 1) % Capacity;
            return _buffer[lastIndex];
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
            int lastIndex = (_head + _count - 1) % Capacity;
            item = _buffer[lastIndex];
            return true;
        }

        #endregion

        #region Internal Helpers

        /// <summary>
        /// Ensures the internal array has enough capacity to hold the specified number of elements.
        /// Grows the array if needed, preserving existing elements in order.
        /// </summary>
        /// <param name="newCount">The total number of elements the array should accommodate.</param>
        private void GrowIfNeeded(int newCount)
        {
            if (newCount > Capacity)
            {
                int newCapacity = Math.Max((int)(Capacity * _growthFactor), newCount);
                T[] newArray = new T[newCapacity];
                for (int i = 0; i < _count; i++)
                    newArray[i] = _buffer[(_head + i) % Capacity];
                _buffer = newArray;
                _head = 0;
            }
        }

        #endregion
        
        private string DebuggerDisplay => $"SfQueue<{typeof(T).Name}> (Count = {Count})";
    }
    
    internal class SfQueueDebugView<T>
    {
        private readonly SfQueue<T> _sfQueue;
        public SfQueueDebugView(SfQueue<T> sfQueue) => _sfQueue = sfQueue;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _sfQueue.ToArray();
    }
}
