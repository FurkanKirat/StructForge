using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// A compact boolean array implemented using packed 64-bit blocks.
    /// Efficient for memory usage and bitwise operations.
    /// </summary>
    public sealed class SfBitArray : ISfDataStructure<bool>
    {
        /// <summary>
        /// Internal bit storage: each ulong holds 64 bits.
        /// </summary>
        private readonly ulong[] _bits;
        private readonly int _count;

        /// <inheritdoc/>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SfBitArray"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The number of bits in the array. Must be positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity"/> is less than or equal to zero.</exception>
        public SfBitArray(int capacity)
        {
            if (capacity <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(capacity));

            _count = capacity;
            _bits = new ulong[(_count + 63) / 64]; // Allocate enough 64-bit blocks
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfBitArray"/> class as a deep copy of another bit array.
        /// </summary>
        /// <param name="other">The bit array to copy.</param>
        public SfBitArray(SfBitArray other)
        {
            _count = other._count;
            _bits = new ulong[other._bits.Length];
            Array.Copy(other._bits, _bits, _bits.Length);
        }
        
        /// <summary>
        /// Initializes a new instance with an ulong buffer using full capacity (bits.Length * 64).
        /// </summary>
        public SfBitArray(ulong[] bits) 
            : this(bits, bits?.Length * 64 ?? 0)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfBitArray"/> class directly from an ulong buffer.
        /// </summary>
        /// <param name="bits">The ulong array containing the bit data. Each ulong represents 64 bits.</param>
        /// <param name="count">The actual number of bits to use. Must be positive and not exceed bits.Length * 64.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bits"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is invalid.</exception>
        public SfBitArray(ulong[] bits, int count)
        {
            if (bits is null)
                SfThrowHelper.ThrowArgumentNull(nameof(bits));
            
            if (count <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(count));
    
            if (count > bits.Length * 64)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(count), 
                    "Count cannot exceed the capacity of the provided bit array.");

            _bits = bits;
            _count = count;
    
            ClearUnusedBits();
        }

        /// <summary>
        /// Returns the internal ulong buffer (shared reference).
        /// </summary>
        /// <returns>The internal ulong array containing the bit data.</returns>
        public ulong[] ToULongArray() => _bits;

        /// <summary>
        /// Gets or sets the boolean value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the bit to get or set.</param>
        /// <returns>The boolean value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative or greater than or equal to <see cref="_count"/>.</exception>
        public bool this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                    SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));

                return GetUnchecked(index);
            }

            set
            {
                if ((uint)index >= (uint)_count)
                    SfThrowHelper.ThrowArgumentOutOfRange(nameof(index));

                SetUnchecked(index, value);
            }
        }

        /// <summary>
        /// Toggles the bit at the specified index (true becomes false, false becomes true).
        /// </summary>
        /// <param name="index">The zero-based index of the bit to toggle.</param>
        public void Toggle(int index) => this[index] = !this[index];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfBitArrayEnumerator GetEnumerator() => new(this);

        /// <inheritdoc/>
        IEnumerator<bool> IEnumerable<bool>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<bool> action)
        {
            for (int i = 0; i < _count; i++)
                action(GetUnchecked(i));
        }

        /// <inheritdoc/>
        public bool Contains(bool item) =>
            Contains(item, SfEqualityComparers<bool>.Default);

        /// <inheritdoc/>
        public bool Contains(bool item, IEqualityComparer<bool> comparer)
        {
            comparer ??= SfEqualityComparers<bool>.Default;

            foreach (bool element in this)
            {
                if (comparer.Equals(item, element))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sets all bits in the array to the specified value.
        /// </summary>
        /// <param name="value">The value to set all bits to.</param>
        public void SetAll(bool value)
        {
            if (value)
            {
                // Set every block to all 1s
                Array.Fill(_bits, ulong.MaxValue);
                ClearUnusedBits();
            }
            else
            {
                Clear();
            }
        }

        /// <summary>
        /// Clears all bits in the array (sets them to false).
        /// </summary>
        public void Clear() =>
            Array.Clear(_bits, 0, _bits.Length);

        /// <summary>
        /// Inverts all bits in the array (true becomes false, false becomes true).
        /// </summary>
        public void Not()
        {
            for (int i = 0; i < _bits.Length; i++)
                _bits[i] = ~_bits[i];
            ClearUnusedBits();
        }

        /// <summary>
        /// Performs a bitwise AND operation with another bit array.
        /// Bits beyond the shorter array become false.
        /// </summary>
        /// <param name="other">The bit array to AND with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public void And(SfBitArray other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            int length = Math.Min(_bits.Length, other._bits.Length);

            int i = 0;
            for (; i <= length - 4; i += 4)
            {
                _bits[i] &= other._bits[i];
                _bits[i + 1] &= other._bits[i + 1];
                _bits[i + 2] &= other._bits[i + 2];
                _bits[i + 3] &= other._bits[i + 3];
            }
            
            for (; i < length; i++)
            {
                _bits[i] &= other._bits[i];
            }

            // Zero out extra blocks (AND identity)
            for (i = length; i < _bits.Length; i++)
                _bits[i] = 0;
        }

        /// <summary>
        /// Performs a bitwise OR operation with another bit array.
        /// </summary>
        /// <param name="other">The bit array to OR with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public void Or(SfBitArray other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            int length = Math.Min(_bits.Length, other._bits.Length);

            for (int i = 0; i < length; i++)
                _bits[i] |= other._bits[i];
            ClearUnusedBits();
        }

        /// <summary>
        /// Performs a bitwise XOR operation with another bit array.
        /// </summary>
        /// <param name="other">The bit array to XOR with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public void Xor(SfBitArray other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            int length = Math.Min(_bits.Length, other._bits.Length);

            for (int i = 0; i < length; i++)
                _bits[i] ^= other._bits[i];
            ClearUnusedBits();
        }

        /// <summary>
        /// Counts how many bits are set to true using fast bit manipulation.
        /// </summary>
        /// <returns>The number of true bits in the array.</returns>
        public int CountTrue()
        {
            ulong count = 0;
            for (int i = 0; i < _bits.Length; i++)
            {
                count += PopCount(_bits[i]);
            }
            return (int)count;
        }

        /// <summary>
        /// Counts how many bits are set to false using fast bit manipulation.
        /// </summary>
        /// <returns>The number of false bits in the array.</returns>
        public int CountFalse()
        {
            return _count - CountTrue();
        }

        /// <summary>
        /// Calculates the population count (number of set bits) of a 32-bit value.
        /// This is also known as the Hamming Weight.
        /// Uses the SWAR (SIMD Within A Register) algorithm for efficient bit counting.
        /// </summary>
        /// <param name="value">The 32-bit value to count bits in.</param>
        /// <returns>The number of bits set to 1 in the value (0-32).</returns>
        /// <remarks>
        /// Algorithm breakdown (works on pairs of bits in parallel):
        /// 
        /// Step 1: value -= (value >> 1) & 0x55555555
        ///   - Groups bits into pairs and counts bits in each pair
        ///   - 0x55555555 = 01010101... (alternating bits)
        ///   - Converts: 00->00, 01->01, 10->01, 11->10
        ///   - Result: Each 2-bit group now contains count of original bits
        /// 
        /// Step 2: value = (value & 0x33333333) + ((value >> 2) & 0x33333333)
        ///   - Combines pairs into 4-bit groups
        ///   - 0x33333333 = 00110011... (alternating 2-bit groups)
        ///   - Result: Each 4-bit group contains count of bits from two pairs (0-4)
        /// 
        /// Step 3: value = (((value + (value >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24
        ///   - First part: Combines 4-bit groups into 8-bit groups (0-8 per byte)
        ///   - 0x0F0F0F0F = 00001111... (masks lower 4 bits of each byte)
        ///   - Multiply by 0x01010101: Cleverly sums all bytes into highest byte
        ///   - >> 24: Extracts the final sum from the highest byte
        /// 
        /// Example for value = 0b10110101 (181):
        ///   Initial:  10110101
        ///   Step 1:   01 01 01 01  (pairs: 1,1,1,1)
        ///   Step 2:   0010 0010    (fours: 2,2)
        ///   Step 3:   00000100     (total: 4)
        ///   Result: 4
        /// 
        /// Time complexity: O(1) - constant time, only 5 operations
        /// Space complexity: O(1) - no extra memory needed
        /// 
        /// Performance: ~5-10x faster than naive bit-by-bit counting for 32-bit values
        /// </remarks>
        private static ulong PopCount(ulong value)
        {
            value -= (value >> 1) & 0x55555555;
            value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
            value = (((value + (value >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
            return value;
        }

        /// <inheritdoc/>
        public void CopyTo(bool[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + _count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");

            foreach (bool item in this)
                array[arrayIndex++] = item;
        }

        /// <inheritdoc/>
        public bool[] ToArray()
        {
            bool[] arr = new bool[_count];
            CopyTo(arr, 0);
            return arr;
        }

        /// <summary>
        /// Returns the bit value at the specified index without bounds checking.
        /// </summary>
        /// <param name="index">The zero-based index of the bit.</param>
        /// <returns>The boolean value at the specified index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetUnchecked(int index)
        {
            int arrayIndex = index >> 6;
            int bitIndex = index & 63;

            ulong mask = 1UL << bitIndex;
            return (_bits[arrayIndex] & mask) != 0;
        }

        /// <summary>
        /// Sets the bit value at the specified index without bounds checking.
        /// </summary>
        /// <param name="index">The zero-based index of the bit.</param>
        /// <param name="value">The value to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnchecked(int index, bool value)
        {
            int arrayIndex = index >> 6;
            int bitIndex = index & 63;

            ulong mask = 1UL << bitIndex;

            if (value)
                _bits[arrayIndex] |= mask;
            else
                _bits[arrayIndex] &= ~mask;
        }
        
        /// <summary>
        /// Clears unused bits in the last block to maintain invariant.
        /// Call this after operations that might set unused bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearUnusedBits()
        {
            int remaining = _count & 63;
            if (remaining > 0)
            {
                ulong mask = (1UL << remaining) - 1;
                _bits[_bits.Length - 1] &= mask;
            }
        }
        
        public struct SfBitArrayEnumerator : IEnumerator<bool>
        {
            private readonly SfBitArray _bitArray;
            private int _index;
            private int _wordIndex;
            private ulong _currentWord;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal SfBitArrayEnumerator(SfBitArray bitArray)
            {
                _bitArray = bitArray;
                _index = -1;
                _wordIndex = 0;
                _currentWord = 0;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (_index + 1 >= _bitArray._count)
                {
                    _index = _bitArray._count;
                    return false;
                }

                _index++;
                if ((_index & 63) == 0)
                    _currentWord = _bitArray.ToULongArray()[_wordIndex++];
                
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _index = -1;
                _wordIndex = 0;
                _currentWord = 0;
            }
            
            public bool Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => (_currentWord & (1UL << (_index & 63))) != 0;
            }

            object IEnumerator.Current => Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() { }
        }
    }
}