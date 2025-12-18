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
    /// Represents a last-in-first-out (LIFO) stack of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(SfStackDebugView<>))]
    public sealed class SfStack<T> : ISfDataStructure<T>
    {
        private readonly SfList<T> _buffer;

        /// <inheritdoc/>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer.Count;
        }

        /// <summary>
        /// Gets the total capacity of the underlying array.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer.Capacity;
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count == 0;
        }

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
            _buffer = new SfList<T>(capacity, growthFactor);
        }

        /// <summary>
        /// Initializes a new stack from an existing collection.
        /// </summary>
        public SfStack(
            IEnumerable<T> collection, 
            int extraCapacity = 0, 
            float growthFactor = SfList<T>.DefaultGrowthFactor)
        {
            if (collection is null)
                SfThrowHelper.ThrowArgumentNull(nameof(collection));
            
            _buffer = new SfList<T>(collection, extraCapacity, growthFactor);
        }
        
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Source Stack</param>
        public SfStack(SfStack<T> other)
        {
            _buffer = new SfList<T>(other._buffer);
        }
        #endregion

        #region IDataStructure<T> Implementation

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item, IEqualityComparer<T> comparer) => _buffer.Contains(item, comparer);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<T> action) => _buffer.ForEach(action);

        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfReverseArrayEnumerator<T> GetEnumerator() => _buffer.GetReverseEnumerator();
        
        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");
            
            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = _buffer[Count - 1 - i];
        }

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[Count];
            CopyTo(arr, 0);
            return arr;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _buffer.Clear();
        }
        
        #endregion
        
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
        public ReadOnlySpan<T> AsReadOnlySpan() => _buffer.AsReadOnlySpan();

        #region Stack Operations

        /// <summary>
        /// Inserts an item at the top of the stack.
        /// </summary>
        /// <param name="item">The item to push onto the stack.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item) => _buffer.Add(item);

        /// <summary>
        /// Removes and returns the item at the top of the stack.
        /// </summary>
        /// <returns>The item removed from the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            if (TryPop(out T item))
                return item;
            
            SfThrowHelper.ThrowInvalidOperation("Stack is empty");
            return default;
        }

        /// <summary>
        /// Returns the item at the top of the stack without removing it.
        /// </summary>
        /// <returns>The item at the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            if (TryPeek(out T item))
                return item;
            
            SfThrowHelper.ThrowInvalidOperation("Stack is empty");
            return default;
        }

        /// <summary>
        /// Attempts to remove and return the item at the top of the stack.
        /// </summary>
        /// <param name="item">When this method returns, contains the object removed, if successful; otherwise, the default value of T.</param>
        /// <returns>True if an item was removed; false if the stack was empty.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPop(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            int lastIndex = Count - 1;
            item = _buffer.Last;
            _buffer.RemoveAt(lastIndex);
            return true;
        }

        /// <summary>
        /// Attempts to return the item at the top of the stack without removing it.
        /// </summary>
        /// <param name="item">When this method returns, contains the object at the top, if successful; otherwise, the default value of T.</param>
        /// <returns>True if an item was retrieved; false if the stack was empty.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeek(out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }

            item = _buffer.Last;
            return true;
        }

        /// <summary>
        /// Reduces the capacity to fit the current count.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TrimExcess()
        {
            _buffer.TrimExcess();
        }

        #endregion
        
        private string DebuggerDisplay => $"SfStack<{typeof(T).Name}> (Count = {Count})";
    }

    internal class SfStackDebugView<T>
    {
        private readonly SfStack<T> _sfStack;
        public SfStackDebugView(SfStack<T> sfStack) => _sfStack = sfStack;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _sfStack.ToArray();
    }
}
