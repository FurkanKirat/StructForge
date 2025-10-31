using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StructForge.Extensions;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a sorted set data structure backed by an AVL tree.
    /// Maintains unique elements in a sorted order defined by a comparer.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SfTreeDebugView<>))]
    public class SfSortedSet<T> : ISfSet<T>, ICollection<T>
    {
        /// <summary>
        /// The underlying AVL tree that stores the elements.
        /// </summary>
        private readonly SfAvlTree<T> _tree;

        /// <summary>
        /// The comparer used for sorting elements.
        /// </summary>
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// Gets the number of elements in the set.
        /// </summary>
        public int Count => _tree.Count;

        /// <summary>
        /// Always false — this collection is mutable.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Indicates whether the set contains no elements.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Gets the minimum element in the set.
        /// </summary>
        public T Min => _tree.FindMin();

        /// <summary>
        /// Gets the maximum element in the set.
        /// </summary>
        public T Max => _tree.FindMax();

        /// <summary>
        /// Initializes an empty sorted set with an optional comparer.
        /// </summary>
        public SfSortedSet(IComparer<T> comparer = null)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _tree = new SfAvlTree<T>(_comparer);
        }

        /// <summary>
        /// Initializes the set with elements from the given collection.
        /// Duplicates will be ignored.
        /// </summary>
        public SfSortedSet(IEnumerable<T> collection, IComparer<T> comparer = null)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _tree = new SfAvlTree<T>(collection, _comparer);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => _tree.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Adds the specified item to the set.
        /// Throws an exception if the item already exists.
        /// </summary>
        public void Add(T item)
        {
            if (!TryAdd(item))
                throw new InvalidOperationException("Duplicate item");
        }

        /// <summary>
        /// Attempts to add the specified item.
        /// Returns true if added successfully, false if duplicate.
        /// </summary>
        public bool TryAdd(T item) => _tree.TryAdd(item);

        /// <summary>
        /// Removes the specified item from the set.
        /// Returns true if the item was found and removed.
        /// </summary>
        public bool Remove(T item) => _tree.Remove(item);

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public void Clear() => _tree.Clear();

        /// <summary>
        /// Checks whether the set contains the specified item.
        /// </summary>
        public bool Contains(T item) => _tree.Contains(item);

        /// <summary>
        /// Checks whether the set contains the specified item
        /// using a custom equality comparer.
        /// </summary>
        public bool Contains(T item, IEqualityComparer<T> comparer) => _tree.Contains(item, comparer);

        /// <summary>
        /// Copies the elements of the set to the specified array starting at the given index.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex) => _tree.CopyTo(array, arrayIndex);

        /// <summary>
        /// Applies the given action to each element in the set.
        /// </summary>
        public void ForEach(Action<T> action) => _tree.ForEach(action);

        /// <summary>
        /// Attempts to find an existing element equal to the given value.
        /// Returns true and outputs the actual value if found.
        /// </summary>
        public bool TryGetValue(T equalValue, out T actualValue) => _tree.TryGetValue(equalValue, out actualValue);

        // -------------------------------------------------------------------
        //  SET OPERATIONS
        // -------------------------------------------------------------------

        /// <summary>
        /// Adds all elements from the specified collection to the set.
        /// Duplicates are ignored.
        /// </summary>
        public void UnionWith(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);
            foreach (var item in other)
                _tree.TryAdd(item);
        }

        /// <summary>
        /// Modifies the current set to contain only elements that are also in the specified collection.
        /// </summary>
        public void IntersectWith(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);

            var otherSet = new HashSet<T>(other, EqualityComparer<T>.Default);
            var toRemove = new List<T>();

            foreach (var item in _tree)
            {
                if (!otherSet.Contains(item))
                    toRemove.Add(item);
            }

            foreach (var item in toRemove)
                _tree.Remove(item);
        }

        /// <summary>
        /// Removes all elements that are also contained in the specified collection.
        /// </summary>
        public void ExceptWith(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);
            foreach (var item in other)
                _tree.Remove(item);
        }

        /// <summary>
        /// Modifies the current set to contain elements that are present
        /// in either the current set or the specified collection, but not both.
        /// </summary>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);
            foreach (var item in other)
            {
                if (!_tree.Remove(item))
                    _tree.TryAdd(item);
            }
        }

        /// <summary>
        /// Determines whether the current set is a subset of the specified collection.
        /// </summary>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);
            var otherSet = new HashSet<T>(other);
            foreach (var item in _tree)
            {
                if (!otherSet.Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the current set is a superset of the specified collection.
        /// </summary>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);
            foreach (var item in other)
            {
                if (!_tree.Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the current set and the specified collection share any common elements.
        /// </summary>
        public bool Overlaps(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);
            foreach (var item in other)
            {
                if (_tree.Contains(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        public bool SetEquals(IEnumerable<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);
            var otherArr = other.OrderBy(x => x, _comparer).ToArray();
            if (otherArr.Length != Count)
                return false;

            var thisArr = this.ToArray();
            for (int i = 0; i < Count; i++)
            {
                if (_comparer.Compare(thisArr[i], otherArr[i]) != 0)
                    return false;
            }
            return true;
        }
    }
}
