#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) stack of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    public class SfStack<T> : IStack<T>
    {
        private readonly SfList<T> _data;

        /// <summary>
        /// Gets the number of elements in the stack.
        /// </summary>
        public int Count => _data.Count;

        /// <summary>
        /// Gets the total capacity of the underlying array.
        /// </summary>
        public int Capacity => _data.Capacity;

        /// <summary>
        /// Returns true if the stack contains no elements.
        /// </summary>
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

        /// <summary>
        /// Determines whether the stack contains a specific element using a custom comparer.
        /// </summary>
        public bool Contains(T item, IEqualityComparer<T> comparer) => _data.Contains(item, comparer);

        /// <summary>
        /// Determines whether the stack contains a specific element using default equality.
        /// </summary>
        public bool Contains(T item) => Contains(item, EqualityComparer<T>.Default);

        /// <summary>
        /// Executes the specified action on each element from top to bottom.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        public void ForEach(Action<T> action)
        {
            _data.ForEach(action);
        }

        /// <summary>
        /// Returns an enumerator that iterates from top to bottom.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                yield return _data[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Copies the stack elements to a destination array starting at specified index.
        /// </summary>
        /// <param name="array">Destination array.</param>
        /// <param name="arrayIndex">Starting index in destination.</param>
        /// <exception cref="ArgumentNullException">If array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If arrayIndex is negative.</exception>
        /// <exception cref="ArgumentException">If destination array is too small.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array too small");

            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = _data[Count - 1 - i];
        }

        /// <summary>
        /// Removes all elements from the stack.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Returns a new array containing elements from top to bottom.
        /// </summary>
        public T[] ToArray()
        {
            T[] result = new T[Count];
            for (int i = 0; i < Count; i++)
                result[i] = _data[Count - 1 - i];
            return result;
        }

        #endregion

        #region Stack Operations

        /// <summary>
        /// Inserts an item at the top of the stack.
        /// </summary>
        public void Push(T item)
        {
            _data.Add(item);
        }

        /// <summary>
        /// Removes and returns the top item.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
        public T Pop()
        {
            if (TryPop(out T item))
                return item;

            throw new InvalidOperationException("Stack is empty");
        }

        /// <summary>
        /// Returns the top item without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
        public T Peek()
        {
            if (TryPeek(out T item))
                return item;

            throw new InvalidOperationException("Stack is empty");
        }

        /// <summary>
        /// Attempts to remove and return the top item safely.
        /// </summary>
        /// <param name="item">The removed item if successful; default(T) otherwise.</param>
        /// <returns>True if successful; false if empty.</returns>
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

        /// <summary>
        /// Attempts to return the top item safely without removing it.
        /// </summary>
        /// <param name="item">The item at the top if successful; default(T) otherwise.</param>
        /// <returns>True if successful; false if empty.</returns>
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
