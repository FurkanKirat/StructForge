using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    public class SfHashSet<T> : ISfSet<T>, ICollection<T>
    {
        private struct SfHashSetNode
        {
            public T Value;
            public int Next;

            public SfHashSetNode(T value, int next)
            {
                Value = value;
                Next = next;
            }
        }

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count { get; private set; }
        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;
        /// <inheritdoc/>
        public bool IsReadOnly => false;
        
        private int[] _buckets;
        private SfHashSetNode[] _nodes;
        private readonly IEqualityComparer<T> _comparer;
        private const float LoadFactor = 0.75f;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfHashSet{T}"/> class with the specified capacity and comparer.
        /// </summary>
        /// <param name="capacity">The initial capacity of the hash set. Default is 16.</param>
        /// <param name="comparer">The equality comparer to use for comparing elements. If null, the default comparer is used.</param>
        public SfHashSet(int capacity = 16, IEqualityComparer<T> comparer = null)
        {
            _comparer = comparer ?? SfEqualityComparers<T>.Default;
            int size = SfPrimeHelper.GetNextPrime(capacity);
            _buckets = new int[size];
            _nodes = new SfHashSetNode[capacity];
            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i] = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfHashSet{T}"/> class with elements from the specified collection.
        /// </summary>
        /// <param name="enumerable">The collection whose elements are copied to the new hash set.</param>
        /// <param name="comparer">The equality comparer to use for comparing elements. If null, the default comparer is used.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> is null.</exception>
        public SfHashSet(IEnumerable<T> enumerable, IEqualityComparer<T> comparer = null)
        {
            _comparer = comparer ?? SfEqualityComparers<T>.Default;

            T[] arr = enumerable.ToArray();
            int size = Math.Max(4, (int)(arr.Length / LoadFactor));
            _buckets = new int[size];
            _nodes = new SfHashSetNode[size];
            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i] = -1;
            
            for (int i = 0; i < arr.Length; i++)
                TryAdd(arr[i]);
        }
        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return _nodes[i].Value;
        }
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
                action(_nodes[i].Value);
        }

        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            int bucketIndex = GetBucketIndex(item);
            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                if (_comparer.Equals(item, _nodes[i].Value))
                    return true;
            }
            return false;
        }
        
        /// <inheritdoc/>
        /// <summary>
        /// If a different comparer is given than the one entered initially,
        /// this method will be O(n). Use the overload <inheritdoc cref="Contains(T)"/> for performance.
        /// </summary>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            if (Equals(comparer, _comparer))
                return Contains(item);
            
            foreach (T element in this)
                if (comparer.Equals(item, element))
                    return true;
            return false;
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            if (!TryAdd(item))
                throw new InvalidOperationException("Duplicate item.");
        }
        
        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int index = GetBucketIndex(item);
            int prev = -1;

            for (int i = _buckets[index]; i >= 0; i = _nodes[i].Next)
            {
                if (_comparer.Equals(item, _nodes[i].Value))
                {
                    if (prev < 0)
                    {
                        _buckets[index] = _nodes[i].Next;
                    }
                    else
                    {
                        _nodes[prev].Next = _nodes[i].Next;
                    }
                    
                    if (i != Count - 1)
                    {
                        _nodes[i] = _nodes[Count - 1];

                        int movedBucketIndex = GetBucketIndex(_nodes[i].Value);
                        int j = _buckets[movedBucketIndex];
                        int prevJ = -1;
                        while (j != Count - 1)
                        {
                            prevJ = j;
                            j = _nodes[j].Next;
                        }

                        if (prevJ < 0)
                            _buckets[movedBucketIndex] = i;
                        else
                            _nodes[prevJ].Next = i;
                    }
                    Count--;
                    return true;
                }
                prev = i;
            }
            
            return false;
        }


        /// <inheritdoc cref="ICollection{T}.Clear" />
        public void Clear()
        {
            for (int i = 0; i < _buckets.Length; i++)
            {
                _buckets[i] = -1;
            }
            Array.Clear(_nodes, 0, Count);
            Count = 0;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.CopyTo" />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) 
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array is not large enough.");
            
            foreach (T item in this)
                array[arrayIndex++] = item;
        }
        
        /// <inheritdoc/>
        public bool TryAdd(T item)
        {
            if (Count >= _buckets.Length * LoadFactor)
            {
                Resize();
            }
            
            int bucketIndex = GetBucketIndex(item);
            int prev = -1;
            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                if (_comparer.Equals(item, _nodes[i].Value))
                    return false;
                prev = i;
            }
            
            _nodes[Count] = new SfHashSetNode(item, -1);
            if (prev < 0)
                _buckets[bucketIndex] = Count;
            else
                _nodes[prev].Next = Count;
            Count++;
            return true;
        }

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                TryAdd(item);
        }

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other)
        {
            SfHashSet<T> otherSet = new SfHashSet<T>(other, _comparer);

            SfList<T> toRemove = new SfList<T>(Math.Min(otherSet.Count, Count));

            for (int i = 0; i < Count; i++)
            {
                if (!otherSet.Contains(_nodes[i].Value))
                    toRemove.Add(_nodes[i].Value);
            }

            foreach (T item in toRemove)
            {
                Remove(item);
            }
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
        {
            SfHashSet<T> otherSet = new SfHashSet<T>(other, _comparer);

            SfList<T> toRemove = new SfList<T>(Math.Min(otherSet.Count, Count));

            for (int i = 0; i < Count; i++)
            {
                if (otherSet.Contains(_nodes[i].Value))
                    toRemove.Add(_nodes[i].Value);
            }

            foreach (T item in toRemove)
            {
                Remove(item);
            }
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other is not SfHashSet<T> otherSet)
                otherSet = new SfHashSet<T>(other, _comparer);
            
            foreach (T item in otherSet)
            {
                if (!TryAdd(item))
                    Remove(item);
            }
        }

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            SfHashSet<T> otherSet = new SfHashSet<T>(other, _comparer);
            return otherSet.IsSupersetOf(this);
        }

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            foreach (T item in other)
                if (!Contains(item))
                    return false;
            
            return true;
        }

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other)
        {
            SfThrowHelper.ThrowIfNull(other);
            foreach (var item in other)
            {
                if (Contains(item))
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other)
        {
            SfHashSet<T> otherSet = new SfHashSet<T>(other, _comparer);
            
            if (otherSet.Count != Count)
                return false;

            foreach (T item in this)
                if (!otherSet.Contains(item))
                    return false;

            return true;
        }

        /// <inheritdoc/>
        public bool TryGetValue(T equalValue, out T actualValue)
        {
            int index = GetBucketIndex(equalValue);
            for (int i = _buckets[index]; i >= 0; i = _nodes[i].Next)
            {
                T current = _nodes[i].Value;
                if (_comparer.Equals(current, equalValue))
                {
                    actualValue = current;
                    return true;
                }
            }
                
            
            actualValue = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetBucketIndex(T item)
            => (_comparer.GetHashCode(item) & 0x7fffffff) % _buckets.Length;

        private void Resize()
        {
            int newSize = SfPrimeHelper.GetNextPrime(_buckets.Length * 2);
            
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newSize; i++)
                newBuckets[i] = -1;

            SfHashSetNode[] newNodes = new SfHashSetNode[newSize];
            Array.Copy(_nodes, 0, newNodes, 0, _nodes.Length);

            for (int i = 0; i < Count; i++)
            {
                int hash = (_comparer.GetHashCode(newNodes[i].Value) & 0x7fffffff) % newSize;
                newNodes[i].Next = newBuckets[hash];
                newBuckets[hash] = i;
            }
            
            _nodes = newNodes;
            _buckets = newBuckets;
        }
        
        
    }
}