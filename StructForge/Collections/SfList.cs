using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;
using StructForge.Sorting;

namespace StructForge.Collections
{
    /// <summary>
    /// A dynamic array (List) implementation similar to System.Collections.Generic.List{T}.
    /// Supports Add, Insert, Remove, RemoveAt, Indexer access, CopyTo, and enumeration.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the list.</typeparam>
    public class SfList<T> : IList<T>, ISfList<T>
    {
        private T[] _array;
        private readonly float _growthFactor;

        // Default initial capacity
        internal const int DefaultCapacity = 16;

        // Default growth factor when array needs expansion
        internal const float DefaultGrowthFactor = 2;

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count { get; private set; }

        /// <summary>
        /// Gets the total capacity of the underlying array.
        /// </summary>
        public int Capacity => _array.Length;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        #region Constructors

        /// <summary>
        /// Creates a list with default capacity.
        /// </summary>
        public SfList(int capacity = DefaultCapacity, float growthFactor = DefaultGrowthFactor)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _array = new T[capacity];
            _growthFactor = Math.Max(growthFactor, 1.5f);
        }
        /// <summary>
        /// Creates a list with given items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="extraCapacity">extra capacity if needed</param>
        /// <param name="growthFactor">growth factor of the list</param>
        public SfList(IEnumerable<T> items, int extraCapacity = 0, float growthFactor = DefaultGrowthFactor)
        {
            SfThrowHelper.ThrowIfNull(items);
            
            if (items is ICollection<T> collection)
            {
                _array = new T[collection.Count + extraCapacity];
                Count = collection.Count;
                collection.CopyTo(_array, 0);
            }
            else
            {
                var enumerable = items.ToArray();
                _array = new T[enumerable.Length + extraCapacity];
                foreach (var item in enumerable)
                    Add(item);
            }
            
            _growthFactor = Math.Max(growthFactor, 1.5f);
        }
        
        /// <summary>
        /// Copy constructor of SfList{T} class
        /// </summary>
        /// <param name="other">Source List</param>
        public SfList(SfList<T> other)
        {
            _array = new T[other.Capacity];
            Array.Copy(other._array, _array, other.Count);
            Count = other.Count;
            _growthFactor = other._growthFactor;
        }
        #endregion

        #region Enumeration

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return _array[i];
        }
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
                action(_array[i]);
        }

        #endregion
        #region Core Methods

        /// <inheritdoc cref="ISfList{T}.Add" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            ReGrowthIfNeeded();
            _array[Count++] = item;
        }
        
        /// <summary>
        /// Adds all elements from the given collection to the end of the list.
        /// </summary>
        /// <param name="enumerable">Collection to add</param>
        public void AddRange(IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            if (enumerable is ICollection<T> collection)
            {
                int addCount = collection.Count;
                ReGrow(Count + addCount);
                collection.CopyTo(_array, Count);
                Count += addCount;
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
            Array.Clear(_array, 0, Count);
            Count = 0;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => _array.Contains(item);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            comparer ??= SfEqualityComparers<T>.Default;
            for (int i = 0; i < Count; i++)
                if (comparer.Equals(_array[i], item))
                    return true;
            return false;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.CopyTo" />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array is not large enough.");

            Array.Copy(_array, 0, array, arrayIndex, Count);
        }

        /// <inheritdoc cref="ISfList{T}.Remove" />
        public bool Remove(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (item?.Equals(_array[i]) == true)
                {
                    // Shift all elements left
                    for (int j = i; j < Count - 1; j++)
                        _array[j] = _array[j + 1];

                    _array[Count - 1] = default; // Clear last slot
                    Count--;
                    return true;
                }
            }
            return false;
        }


        /// <inheritdoc cref="IList{T}.IndexOf" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (item?.Equals(_array[i]) == true)
                    return i;

            return -1;
        }

        /// <inheritdoc cref="ISfList{T}.Insert" />
        public void Insert(int index, T item)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException(nameof(index));

            ReGrowthIfNeeded();

            // Shift elements right
            for (int i = Count; i > index; i--)
                _array[i] = _array[i - 1];

            _array[index] = item;
            Count++;
        }

        /// <inheritdoc cref="ISfList{T}.RemoveAt" />
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) 
                throw new ArgumentOutOfRangeException(nameof(index));

            // Shift elements left
            for (int i = index; i < Count - 1; i++)
                Array.Copy(_array, i + 1, _array, i, Count - i - 1);

            _array[Count - 1] = default;
            Count--;
        }

        /// <inheritdoc/>
        public void Sort() => Sort(SfComparers<T>.DefaultComparer);
        
        /// <inheritdoc/>
        public void Sort(IComparer<T> comparer) => SfSorting.QuickSort(this, comparer);
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap(int i, int j)
        {
            if (i < 0 || i >= Count)
                throw new ArgumentOutOfRangeException(nameof(i));
            
            if (j < 0 || j >= Count)
                throw new ArgumentOutOfRangeException(nameof(j));
            
            (_array[i], _array[j]) = (_array[j], _array[i]);
        }

        /// <inheritdoc cref="ISfList{T}.this" />
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
                return _array[index];
            }
            set
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
                _array[index] = value;
            }
        }

        /// <inheritdoc/>
        public T First
        {
            get => this[0];
            set => this[0] = value;
        }

        /// <inheritdoc/>
        public T Last
        {
            get => this[Count - 1];
            set => this[Count - 1] = value;
        }
        
        #endregion

        /// <summary>
        /// Trims the underlying array if it has excessive capacity,
        /// reducing memory usage without losing elements.
        /// </summary>
        public void TrimExcess()
        {
            const float trimExcessFactor = 1.2f;
            int expected = (int)(trimExcessFactor * Count);
            if (Capacity > expected)
            {
                T[] newArray = new T[expected];
                Array.Copy(_array, 0, newArray, 0, Count);
                _array = newArray;
            }
        }
        
        /// <summary>
        /// Reverses the order of elements in the list in-place.
        /// </summary>
        public void Reverse()
        {
            for (int i = 0; i < Count / 2; i++)
            {
                int  j = Count - i - 1;
                (_array[i],  _array[j]) = (_array[j], _array[i]);
            }
        }
        
        /// <summary>
        /// Determines whether any element matches the specified predicate.
        /// </summary>
        public bool Exists(Predicate<T> match) => FindIndex(match) != -1;
        
        /// <summary>
        /// Returns the first element that matches the specified predicate.
        /// </summary>
        public T Find(Predicate<T> match) => _array[FindIndex(match)];
        
        /// <summary>
        /// Returns the index of the first element that matches the specified predicate, or -1 if none.
        /// </summary>
        public int FindIndex(Predicate<T> match)
        {
            for (int i = 0; i < Count; i++)
                if (match(_array[i])) return i;
            return -1;
        }
        
        
        /// <summary>
        /// Expands the underlying array if the current capacity is reached.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReGrowthIfNeeded()
        {
            if (Capacity == Count)
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
            newCapacity = Math.Max(newCapacity, Count + 1);
            T[] newArray = new T[newCapacity];
            Array.Copy(_array, newArray, Count);
            _array = newArray;
        }
    }
}