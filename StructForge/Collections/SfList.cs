using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StructForge.Comparers;
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
        
        /// <summary>
        /// Gets the number of elements currently stored.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the total capacity of the underlying array.
        /// </summary>
        public int Capacity => _array.Length;

        /// <summary>
        /// Returns true if the list is read-only. Always false in this implementation.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns true if the list contains no elements.
        /// </summary>
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
            ArgumentNullException.ThrowIfNull(items);
            
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

        /// <summary>
        /// Returns an enumerator that iterates over the list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return _array[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <summary>
        /// Executes the specified action on each element of the list.
        /// </summary>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
                action(_array[i]);
        }

        #endregion
        #region Core Methods

        /// <summary>
        /// Adds an item to the end of the list.
        /// </summary>
        public void Add(T item)
        {
            ReGrowthIfNeeded();
            _array[Count++] = item;
        }
        
        /// <summary>
        /// Adds all elements from the given collection to the end of the list.
        /// </summary>
        /// <param name="collection">Collection to add</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            T[] collectionArray = collection.ToArray();
            int addCount = collectionArray.Length;

            if (Count + addCount > Capacity)
            {
                int newCapacity = (int)((Count + addCount) * _growthFactor);
                ReGrow(newCapacity);
            }

            for (int i = 0; i < addCount; i++)
            {
                _array[Count + i] = collectionArray[i];
            }
            Count += addCount;
        }


        /// <summary>
        /// Clears the list. Resets the underlying array elements to default.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Count; i++) 
                _array[i] = default;

            Count = 0;
        }

        /// <summary>
        /// Checks whether the list contains a given item.
        /// </summary>
        public bool Contains(T item) => _array.Contains(item);

        public bool Contains(T item, IEqualityComparer<T> comparer) 
            => _array.Contains(item, comparer);

        /// <summary>
        /// Copies the elements of the list to a destination array starting at specified index.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) throw new ArgumentException("Destination array is not large enough.");

            Array.Copy(_array, 0, array, arrayIndex, Count);
        }

        /// <summary>
        /// Removes the first occurrence of an item. Returns true if removed, false if not found.
        /// </summary>
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

        

        /// <summary>
        /// Returns the index of the first occurrence of an item, or -1 if not found.
        /// </summary>
        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (item?.Equals(_array[i]) == true)
                    return i;

            return -1;
        }

        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
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
        
        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) 
                throw new ArgumentOutOfRangeException(nameof(index));

            // Shift elements left
            for (int i = index; i < Count - 1; i++)
                _array[i] = _array[i + 1];

            _array[Count - 1] = default;
            Count--;
        }

        public void Sort() => Sort(SfComparers<T>.DefaultComparer);

        public void Sort(IComparer<T> comparer) => SfSorting.QuickSort(this, comparer);
        public void Swap(int i, int j)
        {
            if (i < 0 || i >= Count)
                throw new ArgumentOutOfRangeException(nameof(i));
            
            if (j < 0 || j >= Count)
                throw new ArgumentOutOfRangeException(nameof(j));
            
            (_array[i], _array[j]) = (_array[j], _array[i]);
        }

        /// <summary>
        /// Indexer for accessing elements by position.
        /// </summary>
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

        public T First
        {
            get => this[0];
            set => this[0] = value;
        }

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
        private void ReGrow(int newCapacity)
        {
            newCapacity = Math.Max(newCapacity, Count + 1);
            T[] newArray = new T[newCapacity];
            Array.Copy(_array, newArray, _array.Length);
            _array = newArray;
        }
    }
}