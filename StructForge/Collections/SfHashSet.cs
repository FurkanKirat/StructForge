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
    /// <summary>
    /// Represents a hash-based set of unique elements of type <typeparamref name="T"/>.
    /// Provides fast insertion, removal, and lookup operations while ensuring all elements are unique.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the set.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(SfHashSetDebugView<>))]
    public sealed class SfHashSet<T> : ISfSet<T>, ICollection<T>
    {
        internal struct SfHashSetNode
        {
            public T Value;
            public int Next;
            internal readonly int Hash;
            public SfHashSetNode(T value, int next,  int hash)
            {
                Value = value;
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

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

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
            int targetSize = (int)(capacity / LoadFactor);
            int size = SfPrimeHelper.GetNextPrime(targetSize);
            _buckets = new int[size];
            _nodes = new SfHashSetNode[size];
            Array.Fill(_buckets, -1);
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
            Array.Fill(_buckets, -1);
            
            for (int i = 0; i < arr.Length; i++)
                TryAdd(arr[i]);
        }


        /// <inheritdoc />
        public struct SfHashSetEnumerator : IEnumerator<T>
        {
            private readonly SfHashSetNode[] _nodes;
            private readonly int _count;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfHashSetEnumerator(SfHashSetNode[] nodes, int count)
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
            /// Gives the current element's reference
            /// </summary>
            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _nodes[_index].Value;
            }

            T IEnumerator<T>.Current => _nodes[_index].Value;
            object IEnumerator.Current => _nodes[_index].Value;
            
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
        public SfHashSetEnumerator GetEnumerator() => new(_nodes, _count);

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < _count; i++)
                action(_nodes[i].Value);
        }

        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            int hash = _comparer.GetHashCode(item) & 0x7fffffff;
            int bucketIndex = hash % _buckets.Length;
            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                ref var node = ref _nodes[i];
                if (node.Hash == hash && _comparer.Equals(item, node.Value))
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
            
            comparer ??= SfEqualityComparers<T>.Default;
            foreach (T element in this)
                if (comparer.Equals(item, element))
                    return true;
            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (!TryAdd(item))
                SfThrowHelper.ThrowInvalidOperation("Duplicate item.");
        }
        
        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int hashCode = _comparer.GetHashCode(item) & 0x7fffffff;
            int bucketIndex = hashCode % _buckets.Length;
            int prev = -1;

            for (int i = _buckets[bucketIndex]; i >= 0; i = _nodes[i].Next)
            {
                ref var node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(item, node.Value))
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
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");
            
            foreach (T item in this)
                array[arrayIndex++] = item;
        }

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[_count];
            CopyTo(arr, 0);
            return arr;
        }
        
        /// <inheritdoc/>
        public bool TryAdd(T item)
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
                ref var node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(item, node.Value))
                    return false;
                prev = i;
            }
            
            _nodes[_count] = new SfHashSetNode(item, -1, hashCode);
            if (prev < 0)
                _buckets[bucketIndex] = _count;
            else
                _nodes[prev].Next = _count;
            _count++;
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

            SfList<T> toRemove = new SfList<T>(Math.Min(otherSet._count, _count));

            for (int i = 0; i < _count; i++)
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

            SfList<T> toRemove = new SfList<T>(Math.Min(otherSet._count, _count));

            for (int i = 0; i < _count; i++)
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
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
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
            
            if (otherSet._count != _count)
                return false;

            foreach (T item in this)
                if (!otherSet.Contains(item))
                    return false;

            return true;
        }

        /// <inheritdoc/>
        public bool TryGetValue(T equalValue, out T actualValue)
        {
            int hashCode = _comparer.GetHashCode(equalValue) & 0x7fffffff;
            int index = hashCode % _buckets.Length;
            for (int i = _buckets[index]; i >= 0; i = _nodes[i].Next)
            {
                ref var node = ref _nodes[i];
                if (hashCode == node.Hash && _comparer.Equals(node.Value, equalValue))
                {
                    actualValue = node.Value;
                    return true;
                }
            }
                
            
            actualValue = default;
            return false;
        }
        
        private void Resize()
        {
            int newSize = SfPrimeHelper.GetNextPrime(_buckets.Length * 2);
            
            int[] newBuckets = new int[newSize];
            Array.Fill(newBuckets, -1);
            
            SfHashSetNode[] newNodes = new SfHashSetNode[newSize];
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
        
        private string DebuggerDisplay => $"SfHashSet<{typeof(T).Name}> (Count = {Count})";
    }
    
    internal class SfHashSetDebugView<T>
    {
        private readonly SfHashSet<T> _hashSet;
        public SfHashSetDebugView(SfHashSet<T> hashSet) => _hashSet = hashSet;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _hashSet.ToArray();
    }
}