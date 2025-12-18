using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <inheritdoc cref="ISfDictionary{TKey,TValue}" />
    [DebuggerTypeProxy(typeof(SfDictionaryDebugView<,>))]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SfDictionary<TKey, TValue> : 
        ISfDictionary<TKey, TValue>, 
        ISfDataStructure<SfDictionary<TKey,TValue>.SfDictionaryNode>
    {
        /// <summary>
        /// Node struct for SfDictionary (struct for no allocation)
        /// </summary>
        public struct SfDictionaryNode
        {
            internal TKey SfKey;
            internal TValue SfValue;
            internal int Next;
            internal readonly int Hash;
            
            /// <summary>
            /// Returns Key of node
            /// </summary>
            public TKey Key => SfKey;
            
            /// <summary>
            /// Returns Value of node
            /// </summary>
            public TValue Value => SfValue;

            /// <summary>
            /// Constructor for dictionary node
            /// </summary>
            public SfDictionaryNode(TKey key, TValue value, int next, int hash)
            {
                SfKey = key;
                SfValue = value;
                Next = next;
                Hash = hash;
            }
        }

        private int _count;

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
        }
        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count == 0;
        }

        /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        private int[] _buckets;
        private SfDictionaryNode[] _nodes;
        private readonly IEqualityComparer<TKey> _comparer;
        private const float LoadFactor = 0.75f;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfHashSet{T}"/> class with the specified capacity and comparer.
        /// </summary>
        /// <param name="capacity">The initial capacity of the hash set. Default is 16.</param>
        /// <param name="comparer">The equality comparer to use for comparing elements. If null, the default comparer is used.</param>
        public SfDictionary(int capacity = 16, IEqualityComparer<TKey> comparer = null)
        {
            _comparer = comparer ?? SfEqualityComparers<TKey>.Default;
            int targetSize = (int)(capacity / LoadFactor);
    
            int size = SfPrimeHelper.GetNextPrime(targetSize);
            _buckets = new int[size];
            _nodes = new SfDictionaryNode[size];
            Array.Fill(_buckets, -1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfHashSet{T}"/> class with elements from the specified collection.
        /// </summary>
        /// <param name="enumerable">The collection whose elements are copied to the new hash set.</param>
        /// <param name="comparer">The equality comparer to use for comparing elements. If null, the default comparer is used.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> is null.</exception>
        public SfDictionary(IEnumerable<SfDictionaryNode> enumerable, IEqualityComparer<TKey> comparer = null)
        {
            _comparer = comparer ?? SfEqualityComparers<TKey>.Default;

            SfDictionaryNode[] arr = enumerable.ToArray();
            int size = Math.Max(4, (int)(arr.Length / LoadFactor));
            _buckets = new int[size];
            _nodes = new SfDictionaryNode[size];
            Array.Fill(_buckets, -1);
            
            for (int i = 0; i < arr.Length; i++)
                TryAdd(arr[i].SfKey, arr[i].SfValue);
        }

        
        /// <inheritdoc />
        public struct SfDictionaryEnumerator : IEnumerator<SfDictionaryNode>
        {
            private readonly SfDictionaryNode[] _nodes;
            private readonly int _count;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfDictionaryEnumerator(SfDictionaryNode[] nodes, int count)
            {
                _nodes = nodes;
                _count = count;
                _index = -1;
            }

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int next = _index + 1;
                if (next < _count)
                {
                    _index = next;
                    return true;
                }
                return false;
            }
            
            /// <summary>
            /// Returns current ref (for performance)
            /// </summary>
            public ref SfDictionaryNode Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _nodes[_index];
            }

            SfDictionaryNode IEnumerator<SfDictionaryNode>.Current => _nodes[_index];
            object IEnumerator.Current => _nodes[_index].SfKey;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }
        }
        
        /// <inheritdoc cref="IEnumerator{TValue}" />
        public struct SfKeyCollection : IEnumerable<TKey>, IEnumerator<TKey>
        {
            private readonly SfDictionaryNode[] _nodes;
            private readonly int _count;
            private int _index;
            
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfKeyCollection(SfDictionaryNode[] nodes, int count)
            {
                _nodes = nodes;
                _count = count;
                _index = -1;
            }
            
            /// <summary>
            /// Returns an enumerator for iterating over the key collection.
            /// Can be used by <c>foreach</c> loops.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfKeyCollection GetEnumerator() => this;
            
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => this;
            IEnumerator  IEnumerable.GetEnumerator() => this;
            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int next = _index + 1;
                if (next < _count)
                {
                    _index = next;
                    return true;
                }
                return false;
            }
            
            /// <summary>
            /// Returns current ref (for performance)
            /// </summary>
            public ref TKey Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _nodes[_index].SfKey;
            }

            TKey IEnumerator<TKey>.Current => _nodes[_index].SfKey;
            object IEnumerator.Current => _nodes[_index].SfKey;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }
        }


        /// <inheritdoc cref="IEnumerator{TValue}" />
        public struct SfValueCollection : IEnumerable<TValue>, IEnumerator<TValue>
        {
            private readonly SfDictionaryNode[] _nodes;
            private readonly int _count;
            private int _index;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfValueCollection(SfDictionaryNode[] nodes, int count)
            {
                _nodes = nodes;
                _count = count;
                _index = -1;
            }
            
            /// <summary>
            /// Returns an enumerator for iterating over the value collection.
            /// Can be used by <c>foreach</c> loops.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SfValueCollection GetEnumerator() => this;
            
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
            IEnumerator  IEnumerable.GetEnumerator() => this;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int next = _index + 1;
                if (next < _count)
                {
                    _index = next;
                    return true;
                }
                return false;
            }
            
            /// <summary>
            /// Returns current ref (for performance)
            /// </summary>
            public ref TValue Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _nodes[_index].SfValue;
            }

            TValue IEnumerator<TValue>.Current => _nodes[_index].SfValue;
            object IEnumerator.Current => _nodes[_index].SfKey;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }
        }

        
        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfDictionaryEnumerator GetEnumerator() => new(_nodes, _count);

        /// <inheritdoc/>
        IEnumerator<SfDictionaryNode> IEnumerable<SfDictionaryNode>.GetEnumerator() => GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc/>
        public void ForEach(Action<SfDictionaryNode> action)
        {
            for (int i = 0; i < _count; i++)
                action(_nodes[i]);
        }

        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(SfDictionaryNode item)
        {
            int hashCode = _comparer.GetHashCode(item.SfKey) & 0x7fffffff;
            int bucketIndex = hashCode % _buckets.Length;
            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                ref var node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(item.SfKey, node.SfKey))
                    return true;
            }
            return false;
        }
        
        /// <inheritdoc/>
        /// <summary>
        /// If a different comparer is given than the one entered initially,
        /// this method will be O(n). Use the overload <inheritdoc cref="Contains(SfDictionaryNode)"/> for performance.
        /// </summary>
        public bool Contains(SfDictionaryNode item, IEqualityComparer<SfDictionaryNode> comparer)
        {
            if (Equals(comparer, _comparer))
                return Contains(item);
            
            comparer ??= SfEqualityComparers<SfDictionaryNode>.Default;
            foreach (var element in this)
                if (comparer.Equals(item, element))
                    return true;
            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey item, TValue value)
        {
            if (!TryAdd(item, value))
                SfThrowHelper.ThrowInvalidOperation("Duplicate item.");
        }
        
        /// <inheritdoc />
        public ref TValue GetValueRefOrAddDefault(TKey key, out bool exists)
        {
            if (_count >= _buckets.Length * LoadFactor || _count == _nodes.Length) Resize();

            int hashCode = _comparer.GetHashCode(key) & 0x7fffffff;
            int bucketIndex = hashCode % _buckets.Length;
    
            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                ref SfDictionaryNode node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(key, node.SfKey))
                {
                    exists = true;
                    return ref node.SfValue;
                }
            }

            int index = _count;
            _nodes[index] = new SfDictionaryNode(key, default, _buckets[bucketIndex], hashCode);
            _buckets[bucketIndex] = index;
            _count++;
    
            exists = false;
            return ref _nodes[index].SfValue;
        }

        /// <inheritdoc/>
        public bool Remove(TKey item)
        {
            int hashCode = _comparer.GetHashCode(item) & 0x7fffffff;
            int bucketIndex = hashCode % _buckets.Length;
            int prev = -1;

            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                ref SfDictionaryNode node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(item, node.SfKey))
                {
                    if (prev < 0)
                    {
                        _buckets[bucketIndex] = node.Next;
                    }
                    else
                    {
                        _nodes[prev].Next = node.Next;
                    }
                    
                    if (i != _count - 1)
                    {
                        node = _nodes[_count - 1];
                        int movedBucketIndex = node.Hash % _buckets.Length;
                        
                        int j = _buckets[movedBucketIndex];
                        int prevJ = -1;
                        while (j != _count - 1)
                        {
                            prevJ = j;
                            j = _nodes[j].Next;
                        }

                        if (prevJ < 0)
                            _buckets[movedBucketIndex] = i;
                        else
                            _nodes[prevJ].Next = i;
                    }
                    _count--;
                    return true;
                }
                prev = i;
            }
            
            return false;
        }


        /// <inheritdoc cref="ICollection{T}.Clear" />
        public void Clear()
        {
            Array.Fill(_buckets, -1);
            Array.Clear(_nodes, 0, _count);
            _count = 0;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.CopyTo" />
        public void CopyTo(SfDictionaryNode[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");
            
            foreach (SfDictionaryNode item in this)
                array[arrayIndex++] = item;
        }

        /// <inheritdoc/>
        public SfDictionaryNode[] ToArray()
        {
            SfDictionaryNode[] arr = new SfDictionaryNode[_count];
            CopyTo(arr, 0);
            return arr;
        }
        
        /// <inheritdoc/>
        public bool TryAdd(TKey item, TValue value)
        {
            if (_count >= _buckets.Length * LoadFactor)
            {
                Resize();
            }
            
            int hashCode = _comparer.GetHashCode(item) & 0x7fffffff;
            int bucketIndex = hashCode % _buckets.Length;
            int prev = -1;
            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                ref SfDictionaryNode node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(item, node.SfKey))
                    return false;
                prev = i;
            }
            
            _nodes[_count] = new SfDictionaryNode(item, value, -1, hashCode);
            if (prev < 0)
                _buckets[bucketIndex] = _count;
            else
                _nodes[prev].Next = _count;
            _count++;
            return true;
        }

        /// <summary>
        /// Returns an enumerator for Key Collection
        /// </summary>
        public SfKeyCollection Keys => new SfKeyCollection(_nodes, _count);

        /// <summary>
        /// Returns an enumerator for Value Collection
        /// </summary>
        public SfValueCollection Values => new SfValueCollection(_nodes, _count);
        
        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            int hashCode = _comparer.GetHashCode(key) & 0x7fffffff;
            int index = hashCode % _buckets.Length;
            for (int i = _buckets[index]; i >= 0; i = _nodes[i].Next)
            {
                ref SfDictionaryNode node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(node.SfKey, key))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <inheritdoc/>
        public bool ContainsValue(TValue value)
        {
            foreach (SfDictionaryNode item in this)
                if (Equals(item.SfValue, value))
                    return true;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int hashCode = _comparer.GetHashCode(key) & 0x7fffffff;
            int index = hashCode % _buckets.Length;
            for (int i = _buckets[index]; i >= 0; i = _nodes[i].Next)
            {
                ref SfDictionaryNode node = ref _nodes[i];
                if (node.Hash == hashCode && _comparer.Equals(node.SfKey, key))
                {
                    value = node.SfValue;
                    return true;
                }
            }
            
            value = default;
            return false;
        }

        /// <inheritdoc />
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
                int hash = _comparer.GetHashCode(key) & 0x7fffffff;
                int index = hash % _buckets.Length;
                for (int i = _buckets[index]; i >= 0; i = _nodes[i].Next)
                {
                    ref SfDictionaryNode node = ref _nodes[i];
                    if (hash == node.Hash && _comparer.Equals(node.SfKey, key))
                    {
                        node.SfValue = value;
                        return;
                    }
                }
            
                Add(key, value);
            }
        }
        
        private void Resize()
        {
            int newSize = SfPrimeHelper.GetNextPrime(_buckets.Length * 2);
            
            int[] newBuckets = new int[newSize];
            Array.Fill(newBuckets, -1);

            SfDictionaryNode[] newNodes = new SfDictionaryNode[newSize];
            Array.Copy(_nodes, 0, newNodes, 0, _count);

            for (int i = 0; i < _count; i++)
            {
                int bucketIndex = newNodes[i].Hash % newSize;
                newNodes[i].Next = newBuckets[bucketIndex];
                newBuckets[bucketIndex] = i;
            }
            
            _nodes = newNodes;
            _buckets = newBuckets;
        }
        
        private string DebuggerDisplay => $"SfDictionary<{typeof(TKey).Name}, {typeof(TValue).Name}> (Count = {Count})";
    }
    
    internal sealed class SfDictionaryDebugView<TKey, TValue>
    {
        private readonly SfDictionary<TKey, TValue> _dictionary;

        public SfDictionaryDebugView(SfDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                var items = new KeyValuePair<TKey, TValue>[_dictionary.Count];
                int i = 0;
                foreach (var item in _dictionary)
                {
                    items[i++] = new KeyValuePair<TKey, TValue>(item.SfKey, item.SfValue);
                }
                return items;
            }
        }
    }
}