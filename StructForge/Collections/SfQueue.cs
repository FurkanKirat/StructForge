using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StructForge.Comparers;

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

        private T[] _array;
        private readonly float _growthFactor;
        private int _start;

        /// <summary>
        /// Number of elements currently in the queue.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Returns true if the queue has no elements.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Capacity of the underlying array.
        /// </summary>
        public int Capacity => _array.Length;

        /// <summary>
        /// Gets the index of the first empty slot in the circular array.
        /// </summary>
        private int FirstEmpty => (_start + Count) % Capacity;

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
            _array = new T[capacity];
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
            ArgumentNullException.ThrowIfNull(enumerable);
            if (enumerable is ICollection<T> collection)
            {
                _array = new T[collection.Count + extraCapacity];
                collection.CopyTo(_array, 0);
                Count = collection.Count;
            }
            else
            {
                T[] array = enumerable.ToArray();
                Count = array.Length;
                _array = new T[extraCapacity + array.Length];
                Array.Copy(array, _array, Count);
            }

            _start = 0;
            _growthFactor = Math.Max(growthFactor, 1.5f);
        }

        #endregion

        #region Enumeration

        /// <summary>
        /// Returns an enumerator that iterates over the queue in FIFO order.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return _array[(_start + i) % Capacity];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Executes the specified action for each element in queue order.
        /// </summary>
        /// <param name="action">The action to perform on each element.</param>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
                action(_array[(_start + i) % Capacity]);
        }

        #endregion

        #region Core Methods

        /// <summary>Adds an item to the end of the queue.</summary>
        public void Enqueue(T item)
        {
            GrowIfNeeded(Count + 1);
            _array[FirstEmpty] = item;
            Count++;
        }

        /// <summary>Removes and returns the front item of the queue.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        public T Dequeue()
        {
            if (TryDequeue(out T item)) return item;
            throw new InvalidOperationException("Queue is empty");
        }

        /// <summary>Returns the front item without removing it.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        public T Peek()
        {
            if (TryPeek(out T item)) return item;
            throw new InvalidOperationException("Queue is empty");
        }

        /// <summary>Attempts to remove and return the front item. Returns false if the queue is empty.</summary>
        public bool TryDequeue(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            item = _array[_start];
            _array[_start] = default;
            _start = (_start + 1) % Capacity;
            Count--;
            return true;
        }

        /// <summary>Attempts to return the front item without removing it. Returns false if empty.</summary>
        public bool TryPeek(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }
            item = _array[_start];
            return true;
        }

        /// <summary>Removes all elements from the queue.</summary>
        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                _array[(_start + i) % Capacity] = default;
            _start = 0;
            Count = 0;
        }

        /// <summary>Checks if the queue contains the specified item using the default comparer.</summary>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <summary>Checks if the queue contains the specified item using a custom comparer.</summary>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < Count; i++)
            {
                int index = (_start + i) % Capacity;
                if (comparer.Equals(_array[index], item)) return true;
            }
            return false;
        }

        /// <summary>Copies the elements of the queue to an array starting at the specified index.</summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array too small");

            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = _array[(_start + i) % Capacity];
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
                    newArray[i] = _array[(_start + i) % Capacity];
                _array = newArray;
                _start = 0;
            }
        }

        /// <summary>Returns the last item without removing it. Throws if empty.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        public T PeekLast()
        {
            if (IsEmpty) throw new InvalidOperationException("Queue is empty");
            int lastIndex = (_start + Count - 1) % Capacity;
            return _array[lastIndex];
        }

        /// <summary>Attempts to return the last item without removing it. Returns false if empty.</summary>
        public bool TryPeekLast(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }
            int lastIndex = (_start + Count - 1) % Capacity;
            item = _array[lastIndex];
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
                    newArray[i] = _array[(_start + i) % Capacity];
                _array = newArray;
                _start = 0;
            }
        }

        #endregion
    }
}
