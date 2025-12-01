using System;
using System.Collections;
using System.Collections.Generic;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfArrayEnumerator<T> GetEnumerator() => new(_buffer, _count);
        
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

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            ReGrowthIfNeeded();
            _buffer[_count++] = item;
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
        public bool Contains(T item) => Array.IndexOf(_buffer, item, 0, _count) >= 0;

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
        
        /// <inheritdoc/>
        public bool Remove(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (item?.Equals(_buffer[i]) == true)
                {
                    // Shift all elements left
                    for (int j = i; j < _count - 1; j++)
                        _buffer[j] = _buffer[j + 1];

                    _buffer[--_count] = default; // Clear last slot
                    return true;
                }
            }
            return false;
        }


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            return Array.IndexOf(_buffer, item, 0, _count);
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)_count) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));

            ReGrowthIfNeeded();

            // Shift elements right
            if (index < _count)
            {
                Array.Copy(_buffer, index, _buffer, index + 1, _count - index);
            }

            _buffer[index] = item;
            _count++;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_count)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));

            _count--;
            if (index < _count)
                Array.Copy(_buffer, index + 1, _buffer, index, _count - index);
            

            _buffer[_count] = default;
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
            
            if (index < _count)
            {
                _buffer[index] = _buffer[_count];
            }

            _buffer[_count] = default;
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
            return Array.BinarySearch(_buffer, 0, _count, item, null);
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
            
            (_buffer[i], _buffer[j]) = (_buffer[j], _buffer[i]);
        }

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count) 
                    SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));
                
                return _buffer[index];
            }
            set
            {
                if ((uint)index >= (uint)_count) 
                    SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));
                
                _buffer[index] = value;
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
        /// Returns a Span covering the valid elements of the list.
        /// Allows for zero-copy access and extremely fast iteration.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => new(_buffer, 0, _count);

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
        public void Reverse()
        {
            for (int i = 0; i < _count / 2; i++)
            {
                int  j = _count - i - 1;
                (_buffer[i],  _buffer[j]) = (_buffer[j], _buffer[i]);
            }
        }
        
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
    }
}