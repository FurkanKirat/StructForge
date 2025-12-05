using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Extensions;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a sorted set data structure backed by an AVL tree.
    /// Maintains unique elements in a sorted order defined by a comparer.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SfTreeDebugView<>))]
    public sealed class SfSortedSet<T> : ISfSet<T>, ICollection<T>
    {
        /// <summary>
        /// The underlying AVL tree that stores the elements.
        /// </summary>
        private readonly SfAvlTree<T> _tree;

        /// <summary>
        /// The comparer used for sorting elements.
        /// </summary>
        private readonly IComparer<T> _comparer;

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _tree.Count;
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
            get => Count == 0;
        }

        /// <summary>
        /// Gets the minimum element in the set.
        /// </summary>
        public T Min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _tree.FindMin();
        }

        /// <summary>
        /// Gets the maximum element in the set.
        /// </summary>
        public T Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _tree.FindMax();
        }

        /// <summary>
        /// Initializes an empty sorted set with an optional comparer.
        /// </summary>
        public SfSortedSet(IComparer<T> comparer = null)
        {
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
            _tree = new SfAvlTree<T>(_comparer);
        }

        /// <summary>
        /// Initializes the set with elements from the given collection.
        /// Duplicates will be ignored.
        /// </summary>
        public SfSortedSet(IEnumerable<T> collection, IComparer<T> comparer = null)
        {
            _comparer = comparer ?? SfComparers<T>.DefaultComparer;
            _tree = new SfAvlTree<T>(collection, _comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfAvlTree<T>.SfAvlTreeInOrderEnumerator GetEnumerator() => new(_tree);
        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (!TryAdd(item))
                SfThrowHelper.ThrowInvalidOperation("Duplicate item");
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd(T item) => _tree.TryAdd(item);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item) => _tree.Remove(item);

        /// <inheritdoc cref="ISfDataStructure{T}.Clear" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => _tree.Clear();

        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => _tree.Contains(item);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item, IEqualityComparer<T> comparer) => _tree.Contains(item, comparer);

        /// <inheritdoc cref="ISfDataStructure{T}.CopyTo" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex) => _tree.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[Count];
            CopyTo(arr, 0);
            return arr;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<T> action) => _tree.ForEach(action);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(T equalValue, out T actualValue) => _tree.TryGetValue(equalValue, out actualValue);

        // -------------------------------------------------------------------
        //  SET OPERATIONS
        // -------------------------------------------------------------------

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
            foreach (var item in other)
                _tree.TryAdd(item);
        }

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            var otherSet = new HashSet<T>(other, SfEqualityComparers<T>.Default);
            var toRemove = new List<T>();

            foreach (var item in _tree)
            {
                if (!otherSet.Contains(item))
                    toRemove.Add(item);
            }

            foreach (var item in toRemove)
                _tree.Remove(item);
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
            foreach (var item in other)
                _tree.Remove(item);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
            foreach (var item in other)
            {
                if (!_tree.Remove(item))
                    _tree.TryAdd(item);
            }
        }

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
            var otherSet = new HashSet<T>(other);
            foreach (var item in _tree)
            {
                if (!otherSet.Contains(item))
                    return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
            foreach (var item in other)
            {
                if (!_tree.Contains(item))
                    return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
            foreach (var item in other)
            {
                if (_tree.Contains(item))
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
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
