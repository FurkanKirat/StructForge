using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using StructForge.Helpers;

namespace StructForge.Collections
{
        
    /// <summary>
    /// A high-performance enum set implementation, generally performs better than
    /// .net's HashSet for enums
    /// </summary>
    /// <typeparam name="TEnum">The enum type of the enum set</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(SfEnumSetDebuggerView<>))]
    public sealed class SfEnumSet<TEnum> : ISfSet<TEnum>, ICollection<TEnum> where TEnum : Enum
    {
        private readonly SfBitArray _bits;
        private int _count;
        private readonly int _offset;
        
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

        private const int MaxAllowedRange = 262144;
        /// <summary>
        /// Creates an enum set of given enum type
        /// <exception cref="NotSupportedException">Thrown if the enum range is too large.</exception>
        /// </summary>
        public SfEnumSet()
        {
            int range = SfEnumHelper<TEnum>.Range;
            _offset = SfEnumHelper<TEnum>.MinInt;
            if (range > MaxAllowedRange)
                SfThrowHelper.ThrowNotSupported("Enum range too large! Use SfHashSet instead.");
            
            _bits = new SfBitArray(range);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfEnumSet{TEnum}"/> class directly from an ulong buffer.
        /// </summary>
        /// <param name="bits"></param>
        public SfEnumSet(ulong[] bits)
        {
            if (bits is null)
                SfThrowHelper.ThrowArgumentNull(nameof(bits));
            
            int range = SfEnumHelper<TEnum>.Range;
            _offset = SfEnumHelper<TEnum>.MinInt;
            if (range > MaxAllowedRange)
                SfThrowHelper.ThrowNotSupported("Enum range too large! Use SfHashSet instead.");
            
            if (bits.Length == 0)
            {
                int capacity = range > 0 ? range : 1;
                _bits = new SfBitArray(capacity);
                _count = 0;
            }
            else
            {
                _bits = new SfBitArray(bits);
                _count = _bits.CountTrue();
            }
        }

        /// <summary>
        /// Creates an enum set of given enum type
        /// <param name="enumerable">The values given.</param>
        /// <exception cref="NotSupportedException">Thrown if the enum range is too large.</exception>
        /// </summary>
        public SfEnumSet(IEnumerable<TEnum> enumerable)
        {
            if (enumerable == null)
                SfThrowHelper.ThrowArgumentNull(nameof(enumerable));

            if (enumerable is SfEnumSet<TEnum> set)
            {
                _count = set._count;
                _bits = new SfBitArray(set._bits);
            }
            else
            {
                int range = SfEnumHelper<TEnum>.Range;
                if (range > MaxAllowedRange)
                    SfThrowHelper.ThrowNotSupported("Enum range too large! Use SfHashSet instead.");
            
                _bits = new SfBitArray(SfEnumHelper<TEnum>.Range);
                foreach (var item in enumerable)
                    Add(item);
            }
        }
        
        /// <summary>
        /// Returns the underlying data array as span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<ulong> AsSpan() => _bits.AsSpan();
        
        /// <summary>
        /// Returns the underlying data array as readonly span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<ulong> AsReadOnlySpan() =>_bits.AsReadOnlySpan();

        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfEnumSetEnumerator GetEnumerator() => new SfEnumSetEnumerator(this);
        /// <inheritdoc/>
        IEnumerator<TEnum> IEnumerable<TEnum>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TEnum item)
        {
            int index = SfEnumHelper<TEnum>.ToInt(item) - _offset;
            if (index >= 0 && index < _bits.Count && _bits.GetUnchecked(index))
            {
                _bits.SetUnchecked(index, false);
                _count--;
                return true;
            }
            return false;
        }
        
        /// <inheritdoc/>
        public void ForEach(Action<TEnum> action)
        {
            foreach (var item in this)
            {
                action(item);
            }
        }

        /// <inheritdoc cref="ISfDataStructure{T}.Contains(T)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TEnum item)
        {
            int index = SfEnumHelper<TEnum>.ToInt(item) - _offset;
            return (uint)index < (uint)_bits.Count && _bits.GetUnchecked(index);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TEnum item, IEqualityComparer<TEnum> comparer)
        {
            int index = SfEnumHelper<TEnum>.ToInt(item) - _offset;
            return (uint)index < (uint)_bits.Count && _bits.GetUnchecked(index);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TEnum item)
        {
            int index = SfEnumHelper<TEnum>.ToInt(item) - _offset;
            if ((uint)index > (uint)_bits.Count)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(item), "item is out of range.");
            if (_bits.GetUnchecked(index))
                SfThrowHelper.ThrowInvalidOperation("Enum is already in use.");
            
            _bits.SetUnchecked(index, true);
            _count++;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.Clear" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _bits.Clear();
            _count = 0;
        }

        /// <inheritdoc cref="ISfDataStructure{T}.CopyTo" />
        public void CopyTo(TEnum[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");
            
            int limit = _bits.Count;
            for (int i = 0; i < limit; i++)
            {
                if (_bits.GetUnchecked(i))
                {
                    int val = i + _offset;
                    array[arrayIndex++] = Unsafe.As<int, TEnum>(ref val);
                }
            }
        }
        
        /// <inheritdoc/>
        public TEnum[] ToArray()
        {
            var arr = new TEnum[Count];
            CopyTo(arr, 0);
            return arr;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd(TEnum item)
        {
            var value = SfEnumHelper<TEnum>.ToInt(item);
            if (_bits.GetUnchecked(value)) 
                return false;
            _bits.SetUnchecked(value, true);
            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnionWith(IEnumerable<TEnum> other)
        {
            if (other is SfEnumSet<TEnum> otherSet)
            {
                _bits.Or(otherSet._bits);
                _count = _bits.CountTrue();
            }
            else
            {
                foreach (var item in other)
                    TryAdd(item);
            }
            
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IntersectWith(IEnumerable<TEnum> other)
        {
            if (other is SfEnumSet<TEnum> otherSet)
            {
                _bits.And(otherSet._bits);
            }
            else
            {
                var temp = new SfEnumSet<TEnum>(other);
                _bits.And(temp._bits);
            }

            _count = _bits.CountTrue();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExceptWith(IEnumerable<TEnum> other)
        {
            if (other is SfEnumSet<TEnum> otherSet)
            {
                _bits.AndNot(otherSet._bits);
                _count = _bits.CountTrue();
            }
            else
            {
                foreach (var item in other) Remove(item);
            }
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SymmetricExceptWith(IEnumerable<TEnum> other)
        {
            if (other is not SfEnumSet<TEnum> otherSet)
            {
                otherSet = new SfEnumSet<TEnum>(other);
            }

            _bits.Xor(otherSet._bits);
            _count = _bits.CountTrue();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubsetOf(IEnumerable<TEnum> other)
        {
            var otherSet = new SfEnumSet<TEnum>(other);
            return otherSet.IsSupersetOf(this);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSupersetOf(IEnumerable<TEnum> other)
        {
            foreach (var item in other)
            {
                if (_bits.GetUnchecked(SfEnumHelper<TEnum>.ToInt(item)))
                    return false;
            }
            
            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(IEnumerable<TEnum> other)
        {
            foreach (var item in other)
            {
                if (_bits.GetUnchecked(SfEnumHelper<TEnum>.ToInt(item)))
                    return true;
            }
            
            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetEquals(IEnumerable<TEnum> other)
        {
            foreach (var item in other)
            {
                if (!_bits.GetUnchecked(SfEnumHelper<TEnum>.ToInt(item)))
                    return false;
            }
            
            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TEnum equalValue, out TEnum actualValue)
        {
            if (Contains(equalValue))
            {
                actualValue = equalValue;
                return true;
            }
            actualValue = default;
            return false;
        }

       

        /// <inheritdoc/>
        public struct SfEnumSetEnumerator : IEnumerator<TEnum>
        {
            private readonly SfBitArray _bits;
            private readonly int _offset;
            private int _index;

            /// <summary>
            /// Enumerates the SfEnumSet
            /// </summary>
            /// <param name="set"></param>
            public SfEnumSetEnumerator(SfEnumSet<TEnum> set)
            {
                _bits = set._bits;
                _offset = set._offset;
                _index = -1;
            }
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                while (++_index < _bits.Count)
                {
                    if (_bits.GetUnchecked(_index)) return true;
                }
                return false;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _index = -1;
            }

            /// <inheritdoc/>

            public TEnum Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    int value = _index + _offset;
                    return Unsafe.As<int, TEnum>(ref value);
                }
            }

            object IEnumerator.Current => Current;

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }
        }

        private string DebuggerDisplay => $"SfEnumSet<{typeof(TEnum).Name}> (Count = {Count})";
    }

    internal class SfEnumSetDebuggerView<TEnum> where TEnum : Enum
    {
        private readonly SfEnumSet<TEnum> _set;
        public SfEnumSetDebuggerView(SfEnumSet<TEnum> set) => _set = set;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public string[] Items
        {
            get
            {
                return _set.ToArray()
                    .Select(e => $"{e} ({Convert.ToInt32(e)})")
                    .ToArray();
            }
        }
    }
}
