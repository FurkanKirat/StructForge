using System;
using System.Collections;
using System.Collections.Generic;
using StructForge.Comparers;

namespace StructForge.Collections
{
    public class SfDictionary<TKey, TValue> : ISfDictionary<TKey, TValue>
    {
        private readonly SfHashSet<SfKeyValue<TKey, TValue>> _set;
        
        /// <inheritdoc/>
        public int Count => _set.Count;
        
        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfDictionary{TKey, TValue}"/> class with the specified capacity and key comparer.
        /// </summary>
        /// <param name="capacity">The initial capacity of the dictionary. Default is 16.</param>
        /// <param name="comparer">The equality comparer to use for comparing keys. If null, the default comparer is used.</param>
        public SfDictionary(int capacity = 16, IEqualityComparer<TKey> comparer = null)
        {
            var keyValueComparer = SfComparerUtils.CreateKeyValueComparer<TKey, TValue>(comparer);
            _set = new SfHashSet<SfKeyValue<TKey, TValue>>(capacity, keyValueComparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfDictionary{TKey, TValue}"/> class with key-value pairs from the specified collection.
        /// </summary>
        /// <param name="collection">The collection of key-value pairs to be copied to the new dictionary.</param>
        /// <param name="comparer">The equality comparer to use for comparing keys. If null, the default comparer is used.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
        public SfDictionary(IEnumerable<SfKeyValue<TKey, TValue>> collection, IEqualityComparer<TKey> comparer = null)
        {
            var keyValueComparer = SfComparerUtils.CreateKeyValueComparer<TKey, TValue>(comparer);
            _set = new SfHashSet<SfKeyValue<TKey, TValue>>(collection, keyValueComparer);
        }
        
        /// <inheritdoc/>
        public IEnumerator<SfKeyValue<TKey, TValue>> GetEnumerator() => _set.GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc/>
        public void ForEach(Action<SfKeyValue<TKey, TValue>> action) => _set.ForEach(action);

        /// <inheritdoc/>
        public bool Contains(SfKeyValue<TKey, TValue> item) => _set.Contains(item);

        /// <inheritdoc/>
        public bool Contains(SfKeyValue<TKey, TValue> item, IEqualityComparer<SfKeyValue<TKey, TValue>> comparer)
            => _set.Contains(item, comparer);

        /// <inheritdoc/>
        public void Clear() => _set.Clear();

        /// <inheritdoc/>
        public void CopyTo(SfKeyValue<TKey, TValue>[] array, int arrayIndex) => _set.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerable<TKey> Keys
        {
            get
            {
                foreach(var item in _set)
                    yield return item.Key;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TValue> Values
        {
            get
            {
                foreach(var item in _set)
                    yield return item.Value;
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            return _set.Contains(dummy);
        }

        /// <inheritdoc/>
        public bool ContainsValue(TValue value)
        {
            foreach (var item in _set)
                if (item.Value != null && Equals(item.Value, value))
                    return true;

            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            if (_set.TryGetValue(dummy, out var sfKeyValue))
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
                if (_set.TryGetValue(dummy, out var sfKeyValue))
                    sfKeyValue.Value = value;
                else
                {
                    sfKeyValue = new SfKeyValue<TKey, TValue>(key, value);
                    _set.Add(sfKeyValue);
                }
            }
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            var dummy = new SfKeyValue<TKey, TValue>(key, default);
            return _set.Remove(dummy);
        }

        /// <inheritdoc/>
        public bool TryAdd(TKey key, TValue value) => _set.TryAdd(new SfKeyValue<TKey, TValue>(key, value));

        /// <inheritdoc/>
        public void Add(TKey key, TValue value) => _set.Add(new SfKeyValue<TKey, TValue>(key, value));
    }
}