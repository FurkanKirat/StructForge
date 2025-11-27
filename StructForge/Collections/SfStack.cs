using System;
using System.Collections;
using System.Collections.Generic;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) stack of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    public class SfStack<T> : ISfStack<T>
    {
        private readonly SfList<T> _data;

        /// <inheritdoc/>
        public int Count => _data.Count;

        /// <summary>
        /// Gets the total capacity of the underlying array.
        /// </summary>
        public int Capacity => _data.Capacity;

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        #region Constructors
        
        /// <summary>
        /// Initializes a new stack with specified capacity and growth factor.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        /// <param name="growthFactor">Growth factor when resizing is needed.</param>
        public SfStack(
            int capacity = SfList<T>.DefaultCapacity, 
            float growthFactor = SfList<T>.DefaultGrowthFactor)
        {
            _data = new SfList<T>(capacity, growthFactor);
        }

        /// <summary>
        /// Initializes a new stack from an existing collection.
        /// </summary>
        public SfStack(
            IEnumerable<T> collection, 
            int extraCapacity = 0, 
            float growthFactor = SfList<T>.DefaultGrowthFactor)
        {
            SfThrowHelper.ThrowIfNull(collection);
            _data = new SfList<T>(collection, extraCapacity, growthFactor);
        }
        
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Source Stack</param>
        public SfStack(SfStack<T> other)
        {
            _data = new SfList<T>(other._data);
        }
        #endregion

        #region IDataStructure<T> Implementation

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer) => _data.Contains(item, comparer);

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            _data.ForEach(action);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                yield return _data[i];
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array too small");

            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = _data[Count - 1 - i];
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _data.Clear();
        }
        
        #endregion

        #region Stack Operations

        /// <inheritdoc/>
        public void Push(T item)
        {
            _data.Add(item);
        }

        /// <inheritdoc/>
        public T Pop()
        {
            if (TryPop(out T item))
                return item;

            throw new InvalidOperationException("Stack is empty");
        }

        /// <inheritdoc/>
        public T Peek()
        {
            if (TryPeek(out T item))
                return item;

            throw new InvalidOperationException("Stack is empty");
        }

        /// <inheritdoc/>
        public bool TryPop(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            int lastIndex = Count - 1;
            item = _data.Last;
            _data.RemoveAt(lastIndex);
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

            item = _data.Last;
            return true;
        }

        /// <summary>
        /// Reduces the capacity to fit the current count.
        /// </summary>
        public void TrimExcess()
        {
            _data.TrimExcess();
        }

        #endregion
    }
}
