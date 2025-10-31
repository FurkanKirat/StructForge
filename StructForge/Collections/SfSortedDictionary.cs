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

        /// <summary>
        /// Gets an enumerable collection containing all keys in the dictionary, in sorted order.
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                foreach (var pair in _tree)
                    yield return pair.Key;
            }
        }

        /// <summary>
        /// Gets an enumerable collection containing all values in the dictionary,
        /// corresponding to the order of their keys.
        /// </summary>
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
            : this(comparer ?? Comparer<TKey>.Default)
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
            : this(comparer ?? Comparer<TKey>.Default)
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

        /// <summary>
        /// Gets the number of key/value pairs contained in the dictionary.
        /// </summary>
        public int Count => _tree.Count;

        /// <summary>
        /// Gets a value indicating whether the dictionary contains no elements.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Returns an enumerator that iterates through the key/value pairs in sorted order.
        /// </summary>
        public IEnumerator<SfKeyValue<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _tree)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Executes a specified action for each key/value pair in the dictionary, in sorted order.
        /// </summary>
        public void ForEach(Action<SfKeyValue<TKey, TValue>> action)
        {
            foreach (var item in _tree)
                action(item);
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific key/value pair.
        /// </summary>
        public bool Contains(SfKeyValue<TKey, TValue> item) => _tree.Contains(item);

        /// <summary>
        /// Determines whether the dictionary contains a specific key/value pair,
        /// using the specified equality comparer.
        /// </summary>
        public bool Contains(SfKeyValue<TKey, TValue> item, IEqualityComparer<SfKeyValue<TKey, TValue>> comparer)
            => _tree.Contains(item, comparer);

        /// <summary>
        /// Removes all elements from the dictionary.
        /// </summary>
        public void Clear() => _tree.Clear();

        /// <summary>
        /// Copies all key/value pairs to the specified array starting at the given index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> where copying begins.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="arrayIndex"/> is negative.</exception>
        /// <exception cref="ArgumentException">Thrown when the destination array is not large enough.</exception>
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

        /// <summary>
        /// Determines whether the dictionary contains an element with the specified key.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            return _tree.Contains(dummy);
        }

        /// <summary>
        /// Determines whether the dictionary contains at least one element with the specified value.
        /// </summary>
        public bool ContainsValue(TValue value)
        {
            foreach (var item in _tree)
                if (item.Value != null && item.Value.Equals(value))
                    return true;

            return false;
        }

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type.
        /// </param>
        /// <returns><see langword="true"/> if the key was found; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when attempting to get a value for a key that does not exist.
        /// </exception>
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

        /// <summary>
        /// Removes the element with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element was found and removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey key)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            return _tree.Remove(dummy);
        }

        /// <summary>
        /// Attempts to add a new key/value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns><see langword="true"/> if the key/value pair was added successfully; otherwise, <see langword="false"/>.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            var keyValue = new SfKeyValue<TKey, TValue>(key, value);
            return _tree.TryAdd(keyValue);
        }

        /// <summary>
        /// Adds a new key/value pair to the dictionary.
        /// Throws an exception if the key already exists.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <exception cref="InvalidOperationException">Thrown when a duplicate key is added.</exception>
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new InvalidOperationException("Duplicate item");
        }
    }
}
