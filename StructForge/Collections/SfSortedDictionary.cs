using System;
using System.Collections;
using System.Collections.Generic;
using StructForge.Comparers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a key-value mapping stored in a self-balancing binary search tree (AVL Tree),
    /// maintaining the elements in sorted order by key.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class SfSortedDictionary<TKey, TValue> : ISfDictionary<TKey, TValue>
    {
        private readonly SfAvlTree<SfKeyValue<TKey, TValue>> _tree;

        /// <inheritdoc/>
        public IEnumerable<TKey> Keys
        {
            get
            {
                foreach (var pair in _tree)
                    yield return pair.Key;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var pair in _tree)
                    yield return pair.Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfSortedDictionary{TKey, TValue}"/> class
        /// using the default key comparer.
        /// </summary>
        public SfSortedDictionary()
            : this(SfComparers<TKey>.DefaultComparer)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfSortedDictionary{TKey, TValue}"/> class
        /// with elements copied from an existing collection of <see cref="SfKeyValue{TKey, TValue}"/> pairs.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new dictionary.</param>
        /// <param name="comparer">An optional key comparer. If null, the default comparer is used.</param>
        public SfSortedDictionary(IEnumerable<SfKeyValue<TKey, TValue>> collection, IComparer<TKey> comparer = null)
            : this(comparer ?? SfComparers<TKey>.DefaultComparer)
        {
            foreach (var kv in collection)
                Add(kv.Key, kv.Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfSortedDictionary{TKey, TValue}"/> class
        /// with elements copied from a collection of <see cref="KeyValuePair{TKey, TValue}"/> pairs.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new dictionary.</param>
        /// <param name="comparer">An optional key comparer. If null, the default comparer is used.</param>
        public SfSortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IComparer<TKey> comparer = null)
            : this(comparer ?? SfComparers<TKey>.DefaultComparer)
        {
            foreach (var kv in collection)
                Add(kv.Key, kv.Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfSortedDictionary{TKey, TValue}"/> class
        /// using the specified key comparer.
        /// </summary>
        /// <param name="comparer">The comparer used to order keys in the dictionary.</param>
        public SfSortedDictionary(IComparer<TKey> comparer)
        {
            var keyValueComparer = SfComparerUtils.CreateKeyValueComparer<TKey, TValue>(comparer);
            _tree = new SfAvlTree<SfKeyValue<TKey, TValue>>(keyValueComparer);
        }

        /// <inheritdoc/>
        public int Count => _tree.Count;

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        /// <inheritdoc/>
        public IEnumerator<SfKeyValue<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _tree)
                yield return item;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<SfKeyValue<TKey, TValue>> action)
        {
            foreach (var item in _tree)
                action(item);
        }

        /// <inheritdoc/>
        public bool Contains(SfKeyValue<TKey, TValue> item) => _tree.Contains(item);

        /// <inheritdoc/>
        public bool Contains(SfKeyValue<TKey, TValue> item, IEqualityComparer<SfKeyValue<TKey, TValue>> comparer)
            => _tree.Contains(item, comparer);

        /// <inheritdoc/>
        public void Clear() => _tree.Clear();

        /// <inheritdoc/>
        public void CopyTo(SfKeyValue<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array is not large enough.");

            foreach (var item in _tree)
                array[arrayIndex++] = item;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            return _tree.Contains(dummy);
        }

        /// <inheritdoc/>
        public bool ContainsValue(TValue value)
        {
            foreach (var item in _tree)
                if (item.Value != null && Equals(item.Value, value))
                    return true;

            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            if (_tree.TryGetValue(dummy, out var sfKeyValue))
            {
                value = sfKeyValue.Value;
                return true;
            }
            value = default;
            return false;
        }

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                    return value;
                throw new KeyNotFoundException();
            }
            set
            {
                var dummy = new SfKeyValue<TKey, TValue>(key, default);
                if (_tree.TryGetValue(dummy, out var sfKeyValue))
                    sfKeyValue.Value = value;
                else
                {
                    sfKeyValue = new SfKeyValue<TKey, TValue>(key, value);
                    _tree.Add(sfKeyValue);
                }
            }
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            return _tree.Remove(dummy);
        }

        /// <inheritdoc/>
        public bool TryAdd(TKey key, TValue value)
        {
            var keyValue = new SfKeyValue<TKey, TValue>(key, value);
            return _tree.TryAdd(keyValue);
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new InvalidOperationException("Duplicate item");
        }
    }
}
