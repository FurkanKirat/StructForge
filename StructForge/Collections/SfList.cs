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
    /// A dynamic array (List) implementation similar to System.Collections.Generic.List{T}.
    /// Supports Add, Insert, Remove, RemoveAt, Indexer access, CopyTo, and enumeration.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the list.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(SfListDebugView<>))]
    public sealed class SfList<T> : IList<T>, ISfDataStructure<T>
    {
        private T[] _buffer;
        private readonly float _growthFactor;
        private int _count;

        // Default initial capacity
        internal const int DefaultCapacity = 16;

        // Default growth factor when array needs expansion
        internal const float DefaultGrowthFactor = 2;
        private const float MinGrowthFactor = 1.5f;

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
        }

        /// <summary>
        /// Gets the total capacity of the underlying array.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer.Length;
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count == 0;
        }

        #region Constructors

        /// <summary>
        /// Creates a list with default capacity.
        /// </summary>
        public SfList(int capacity = DefaultCapacity, float growthFactor = DefaultGrowthFactor)
        {
            if (capacity < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(capacity));
            
            _buffer = new T[capacity];
            _growthFactor = Math.Max(growthFactor, MinGrowthFactor);
        }
        /// <summary>
        /// Creates a list with given items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="extraCapacity">extra capacity if needed</param>
        /// <param name="growthFactor">growth factor of the list</param>
        public SfList(IEnumerable<T> items, int extraCapacity = 0, float growthFactor = DefaultGrowthFactor)
        {
            if (items is null)
                SfThrowHelper.ThrowArgumentNull(nameof(items));
            
            if (items is ICollection<T> collection)
            {
                _buffer = new T[collection.Count + extraCapacity];
                _count = collection.Count;
                collection.CopyTo(_buffer, 0);
            }
            else
            {
                var enumerable = items.ToArray();
                _buffer = new T[enumerable.Length + extraCapacity];
                foreach (var item in enumerable)
                    Add(item);
            }
            
            _growthFactor = Math.Max(growthFactor, MinGrowthFactor);
        }
        
        /// <summary>
        /// Copy constructor of SfList{T} class
        /// </summary>
        /// <param name="other">Source List</param>
        public SfList(SfList<T> other)
        {
            _buffer = new T[other.Capacity];
            Array.Copy(other._buffer, _buffer, other._count);
            _count = other._count;
            _growthFactor = other._growthFactor;
        }
        #endregion

        #region Enumeration

        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfArrayEnumerator<T> GetEnumerator() => new(_buffer, _count);
        
        /// <summary>
        /// Returns an enumerator for iterating over the collection in reverse order.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>A reverse enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfReverseArrayEnumerator<T> GetReverseEnumerator()
        {
            return new SfReverseArrayEnumerator<T>(_buffer, _count);
        }

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < _count; i++)
                action(_buffer[i]);
        }

        #endregion
        #region Core Methods
        
        /// <summary>
        /// Adds an item to the end of the list.
        /// Similar to <see cref="ICollection{T}.Add"/>, but uses <c>in T</c> for performance to avoid unnecessary copies.
        /// </summary>
        /// <param name="item">The item to add to the list.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T item)
        {
            ReGrowthIfNeeded();
            ref var baseRef = ref _buffer[0];
            Unsafe.Add(ref baseRef, _count++) = item; 
        }
        
        /// <inheritdoc />
        void ICollection<T>.Add(T item)
        {
            Add(in item);
        }
        
        /// <summary>
        /// Adds all elements from the given collection to the end of the list.
        /// </summary>
        /// <param name="enumerable">Collection to add</param>
        public void AddRange(IEnumerable<T> enumerable)
        {
            if (enumerable is null) 
                SfThrowHelper.ThrowArgumentNull(nameof(enumerable));

            if (enumerable is ICollection<T> collection)
            {
                int addCount = collection.Count;
                ReGrow(_count + addCount);
                collection.CopyTo(_buffer, _count);
                _count += addCount;
            }
            else
            {
                foreach (var item in enumerable)
                    Add(item);
            }
            
        }
        
        /// <inheritdoc cref="ISfDataStructure{T}.Clear" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_buffer, 0, _count);
            _count = 0;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in T item) => Array.IndexOf(_buffer, item, 0, _count) >= 0;
        
        bool ICollection<T>.Contains(T item) => Contains(in item);
        bool ISfDataStructure<T>.Contains(T item) => Contains(in item);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            comparer ??= SfEqualityComparers<T>.Default;
            for (int i = 0; i < _count; i++)
                if (comparer.Equals(_buffer[i], item))
                    return true;
            return false;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.CopyTo" />
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
        
        /// <summary>
        /// Removes the first occurrence of a specific item from the list.
        /// Similar to <see cref="ICollection{T}.Remove"/>, but uses <c>in T</c> for performance.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was successfully removed; otherwise, false.</returns>
        public bool Remove(in T item)
        {
            int index = Array.IndexOf(_buffer, item, 0, _count);
            if (index < 0) 
                return false;
            
            ref var baseRef = ref _buffer[0];
            for (int j = index; j < _count - 1; j++)
            {
                Unsafe.Add(ref baseRef, j) = Unsafe.Add(ref baseRef, j + 1);
            }
            Unsafe.Add(ref baseRef, --_count) = default; // Clear last slot
            return true;
        }
        bool ICollection<T>.Remove(T item) => Remove(in item);
        
        /// <summary>
        /// Returns the index of the first occurrence of the specified item in the list.
        /// Similar to <see cref="List{T}.IndexOf(T)"/>, but uses <c>in T</c> for performance to avoid unnecessary copies.
        /// </summary>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of the item, or -1 if not found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item) => Array.IndexOf(_buffer, item, 0, _count);
        int IList<T>.IndexOf(T item) => IndexOf(in item);

        /// <summary>
        /// Inserts an item at the specified index in the list.
        /// This is similar to <see cref="IList{T}.Insert"/>, but uses <c>in T</c> for performance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, in T item)
        {
            if ((uint)index > (uint)_count) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));

            ReGrowthIfNeeded();

            // Shift elements right
            if (index < _count)
            {
                Array.Copy(_buffer, index, _buffer, index + 1, _count - index);
            }

            ref var baseRef = ref _buffer[0];
            Unsafe.Add(ref baseRef, index) = item;
            _count++;
        }
        
        void IList<T>.Insert(int index, T item) => Insert(index, in item);

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_count)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));

            _count--;
            if (index < _count)
                Array.Copy(_buffer, index + 1, _buffer, index, _count - index);
            
            ref var baseRef = ref _buffer[0];
            Unsafe.Add(ref baseRef, _count) = default;
        }
        
        /// <summary>
        /// Removes element at index by replacing it with the last element. O(1).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAtSwap(int index)
        {
            if ((uint)index >= (uint)_count) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));

            _count--;
            
            ref var baseRef = ref _buffer[0];
            if (index < _count)
            {
                Unsafe.Add(ref baseRef, index) = Unsafe.Add(ref baseRef, _count);
            }

            Unsafe.Add(ref baseRef, _count) = default;
        }

        /// <summary>
        /// Sorts the array
        /// </summary>
        public void Sort() => Sort(SfComparers<T>.DefaultComparer);
        
        /// <summary>
        /// Sorts the array according to comparer
        /// </summary>
        public void Sort(IComparer<T> comparer) => Array.Sort(_buffer, 0, _count, comparer);
        
        /// <summary>
        /// Searchs the specified item with O(logn), to working properly the list should be sorted.
        /// </summary>
        /// <param name="item">The item to search.</param>
        /// <returns>Index of the specified item, if not found -1.</returns>
        public int BinarySearch(T item)
        {
            return Array.BinarySearch(_buffer, 0, _count, item, SfComparers<T>.DefaultComparer);
        }
        
        /// <summary>
        /// Searchs the specified item with O(logn), to working properly the list should be sorted.
        /// </summary>
        /// <param name="item">The item to search.</param>
        /// <param name="comparer">The given comparer</param>
        /// <returns>Index of the specified item, if not found -1.</returns>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return Array.BinarySearch(_buffer, 0, _count, item, comparer);
        }
        
        /// <summary>
        /// Swaps the two variables at given indexes
        /// </summary>
        /// <param name="i">first index</param>
        /// <param name="j">second index</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap(int i, int j)
        {
            if ((uint)i >= (uint)_count)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(i));
            
            if ((uint)j >= (uint)_count)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(j));
            
            ref var baseRef = ref _buffer[0];
            ref T refI = ref Unsafe.Add(ref baseRef, i);
            ref T refJ = ref Unsafe.Add(ref baseRef, j);
            
            (refI, refJ) = (refJ, refI);
        }

        /// <summary>
        /// Reference indexer for SfList class
        /// </summary>
        /// <param name="index"></param>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[index];
        }

        /// <inheritdoc/>
        T IList<T>.this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count) 
                    SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));
                
                ref var baseRef = ref _buffer[0];
                return Unsafe.Add(ref baseRef, index); 
            }
            set
            {
                if ((uint)index >= (uint)_count) 
                    SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));
                
                ref var baseRef = ref _buffer[0];
                Unsafe.Add(ref baseRef, index) = value; 
            }
        }
        
        /// <summary>
        /// Gets & Sets the first element of the array
        /// </summary>
        public T First
        {
            get => this[0];
            set => this[0] = value;
        }

        /// <summary>
        /// Gets & Sets the last element of the array
        /// </summary>
        public T Last
        {
            get => this[_count - 1];
            set => this[_count - 1] = value;
        }
        
        #endregion
        
        /// <summary>
        /// Returns the underlying data array as span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => new(_buffer, 0, _count);
        
        /// <summary>
        /// Returns the underlying data array as readonly span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsReadOnlySpan() => new(_buffer, 0, _count);

        /// <summary>
        /// Trims the underlying array if it has excessive capacity,
        /// reducing memory usage without losing elements.
        /// </summary>
        public void TrimExcess()
        {
            const float trimExcessFactor = 1.2f;
            int expected = (int)(trimExcessFactor * _count);
            if (Capacity > expected)
            {
                T[] newArray = new T[expected];
                Array.Copy(_buffer, 0, newArray, 0, _count);
                _buffer = newArray;
            }
        }
        
        /// <summary>
        /// Reverses the order of elements in the list in-place.
        /// </summary>
        public void Reverse() => Array.Reverse(_buffer, 0, _count);
        
        /// <summary>
        /// Determines whether any element matches the specified predicate.
        /// </summary>
        public bool Exists(Predicate<T> match) => FindIndex(match) != -1;
        
        /// <summary>
        /// Returns the first element that matches the specified predicate.
        /// </summary>
        public T Find(Predicate<T> match) => _buffer[FindIndex(match)];
        
        /// <summary>
        /// Returns the index of the first element that matches the specified predicate, or -1 if none.
        /// </summary>
        public int FindIndex(Predicate<T> match)
        {
            for (int i = 0; i < _count; i++)
                if (match(_buffer[i])) return i;
            return -1;
        }
        
        
        /// <summary>
        /// Expands the underlying array if the current capacity is reached.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReGrowthIfNeeded()
        {
            if (Capacity == _count)
            {
                int size = Math.Min(int.MaxValue, (int)(Capacity * DefaultGrowthFactor));
                ReGrow(size);
            }
        }

        /// <summary>
        /// Expands the underlying array to given capacity
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReGrow(int newCapacity)
        {
            newCapacity = Math.Max(newCapacity, _count + 1);
            T[] newBuffer = new T[newCapacity];
            if (_count > 0)
            {
                Array.Copy(_buffer, 0, newBuffer, 0, _count);
            }
            _buffer = newBuffer;
        }
        
        private string DebuggerDisplay => $"SfList<{typeof(T).Name}> (Count = {Count})";
    }

    internal class SfListDebugView<T>
    {
        private readonly SfList<T> _list;
        public SfListDebugView(SfList<T> list) => _list = list;
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _list.ToArray();
    }
}