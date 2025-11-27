using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// A circular array-based queue implementation similar to <see cref="System.Collections.Generic.Queue{T}"/>.
    /// Supports Enqueue, Dequeue, Peek, TryPeek, ToArray, Contains, Clear, and enumeration.
    /// Additional methods: TrimExcess, PeekLast, TryPeekLast, ForEach.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the queue.</typeparam>
    public class SfQueue<T> : ISfQueue<T>
    {
        private const int DefaultCapacity = 4;
        private const float DefaultGrowthFactor = 2f;

        private T[] _buffer;
        private readonly float _growthFactor;
        private int _head;

        /// <inheritdoc/>
        public int Count { get; private set; }

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Capacity of the underlying array.
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        /// Gets the index of the first empty slot in the circular array.
        /// </summary>
        private int FirstEmpty => (_head + Count) % Capacity;

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
            _growthFactor = Math.Max(growthFactor, 1.5f);
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
            SfThrowHelper.ThrowIfNull(enumerable);
            if (enumerable is ICollection<T> collection)
            {
                _buffer = new T[collection.Count + extraCapacity];
                collection.CopyTo(_buffer, 0);
                Count = collection.Count;
            }
            else
            {
                T[] array = enumerable.ToArray();
                Count = array.Length;
                _buffer = new T[extraCapacity + array.Length];
                Array.Copy(array, _buffer, Count);
            }

            _head = 0;
            _growthFactor = Math.Max(growthFactor, 1.5f);
        }

        #endregion

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

        #region Core Methods

        /// <inheritdoc/>
        public void Enqueue(T item)
        {
            GrowIfNeeded(Count + 1);
            _buffer[FirstEmpty] = item;
            Count++;
        }

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

        /// <inheritdoc/>
        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                _buffer[(_head + i) % Capacity] = default;
            _head = 0;
            Count = 0;
        }

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
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array too small");

            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = _buffer[(_head + i) % Capacity];
        }

        #endregion

        #region Additional Methods

        /// <summary>Reduces the underlying array capacity to fit the current count.</summary>
        public void TrimExcess()
        {
            if (Count < Capacity)
            {
                T[] newArray = new T[Count];
                for (int i = 0; i < Count; i++)
                    newArray[i] = _buffer[(_head + i) % Capacity];
                _buffer = newArray;
                _head = 0;
            }
        }

        /// <inheritdoc/>
        public T PeekLast()
        {
            if (IsEmpty) throw new InvalidOperationException("Queue is empty");
            int lastIndex = (_head + Count - 1) % Capacity;
            return _buffer[lastIndex];
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
                for (int i = 0; i < Count; i++)
                    newArray[i] = _buffer[(_head + i) % Capacity];
                _buffer = newArray;
                _head = 0;
            }
        }

        #endregion
    }
}
