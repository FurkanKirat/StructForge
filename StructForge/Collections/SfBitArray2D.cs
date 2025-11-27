using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a two-dimensional bit array backed by a linear <see cref="SfBitArray"/>.
    /// Provides fast indexed access and efficient bitwise operations in 2D form.
    /// </summary>
    public class SfBitArray2D : ISfDataStructure<bool>
    {
        /// <summary>
        /// The underlying linear bit array that stores the 2D data.
        /// </summary>
        private readonly SfBitArray _buffer;

        /// <summary>
        /// Gets the width (X dimension) of the bit array.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height (Y dimension) of the bit array.
        /// </summary>
        public int Height { get; }

        /// <inheritdoc/>
        public int Count => _buffer.Count;

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SfBitArray2D"/> class with the specified dimensions.
        /// </summary>
        /// <param name="width">The width (X dimension) of the bit array. Must be positive.</param>
        /// <param name="height">The height (Y dimension) of the bit array. Must be positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="width"/> or <paramref name="height"/> is less than or equal to zero.
        /// </exception>
        public SfBitArray2D(int width, int height)
        {
            SfThrowHelper.ThrowIfNonPositive(width, nameof(width));
            SfThrowHelper.ThrowIfNonPositive(height, nameof(height));

            Width = width;
            Height = height;
            _buffer = new SfBitArray(width * height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfBitArray2D"/> class as a deep copy of another 2D bit array.
        /// </summary>
        /// <param name="other">The source 2D bit array to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public SfBitArray2D(SfBitArray2D other)
        {
            SfThrowHelper.ThrowIfNull(other, nameof(other));
            Width = other.Width;
            Height = other.Height;
            _buffer = new SfBitArray(other._buffer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfBitArray2D"/> class using an existing ulong buffer.
        /// The buffer length must match the required size for the given dimensions.
        /// </summary>
        /// <param name="width">The width (X dimension) of the bit array.</param>
        /// <param name="height">The height (Y dimension) of the bit array.</param>
        /// <param name="bits">The backing ulong array containing the bit data.</param>
        /// <exception cref="ArgumentException">Thrown when the buffer size does not match the required size for the dimensions.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bits"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="width"/> or <paramref name="height"/> is non-positive.</exception>
        public SfBitArray2D(int width, int height, ulong[] bits)
        {
            SfThrowHelper.ThrowIfNull(bits, nameof(bits));
            SfThrowHelper.ThrowIfNonPositive(width, nameof(width));
            SfThrowHelper.ThrowIfNonPositive(height, nameof(height));

            int totalBits = width * height;
            int requiredUlongs = (totalBits + 63) / 64;

            if (bits.Length != requiredUlongs)
                throw new ArgumentException(
                    $"Invalid backing buffer size. Expected {requiredUlongs}, got {bits.Length}.");

            Width = width;
            Height = height;
            _buffer = new SfBitArray(bits, totalBits);
        }

        /// <summary>
        /// Returns the underlying ulong buffer used by the 2D bit array.
        /// </summary>
        /// <returns>The internal ulong array containing the bit data.</returns>
        public ulong[] ToULongArray() => _buffer.ToULongArray();

        /// <summary>
        /// Calculates the linear bit index corresponding to a 2D coordinate.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The linear index in the backing buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Index(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y));

            return y * Width + x;
        }

        /// <summary>
        /// Gets or sets the bit value at the specified 2D coordinate.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The boolean value at the specified coordinate.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        public bool this[int x, int y]
        {
            get => _buffer[Index(x, y)];
            set => _buffer[Index(x, y)] = value;
        }

        /// <inheritdoc/>
        public IEnumerator<bool> GetEnumerator() => _buffer.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void ForEach(Action<bool> action) => _buffer.ForEach(action);

        /// <inheritdoc/>
        public bool Contains(bool item) => _buffer.Contains(item);

        /// <inheritdoc/>
        public bool Contains(bool item, IEqualityComparer<bool> comparer) => _buffer.Contains(item, comparer);

        /// <summary>
        /// Clears all bits in the array (sets them to false).
        /// </summary>
        public void Clear() => _buffer.Clear();

        /// <inheritdoc/>
        public void CopyTo(bool[] array, int arrayIndex) => _buffer.CopyTo(array, arrayIndex);

        /// <summary>
        /// Sets all bits in the array to the specified value.
        /// </summary>
        /// <param name="value">The value to set all bits to.</param>
        public void SetAll(bool value) => _buffer.SetAll(value);

        /// <summary>
        /// Counts the number of bits set to true.
        /// </summary>
        /// <returns>The number of true bits in the array.</returns>
        public int CountTrue() => _buffer.CountTrue();

        /// <summary>
        /// Counts the number of bits set to false.
        /// </summary>
        /// <returns>The number of false bits in the array.</returns>
        public int CountFalse() => _buffer.CountFalse();

        /// <summary>
        /// Toggles the bit at the specified 2D coordinate (true becomes false, false becomes true).
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        public void Toggle(int x, int y) => _buffer.Toggle(Index(x, y));

        /// <summary>
        /// Inverts all bits in the array (true becomes false, false becomes true).
        /// </summary>
        public void Not() => _buffer.Not();

        /// <summary>
        /// Performs a bitwise AND operation with another 2D bit array.
        /// Both arrays must have the same dimensions.
        /// </summary>
        /// <param name="other">The bit array to AND with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the dimensions of the arrays do not match.</exception>
        public void And(SfBitArray2D other)
        {
            SfThrowHelper.ThrowIfNull(other, nameof(other));

            if (Width != other.Width || Height != other.Height)
                throw new ArgumentException("BitArray2D sizes must match.");

            _buffer.And(other._buffer);
        }

        /// <summary>
        /// Performs a bitwise OR operation with another 2D bit array.
        /// Both arrays must have the same dimensions.
        /// </summary>
        /// <param name="other">The bit array to OR with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the dimensions of the arrays do not match.</exception>
        public void Or(SfBitArray2D other)
        {
            SfThrowHelper.ThrowIfNull(other, nameof(other));

            if (Width != other.Width || Height != other.Height)
                throw new ArgumentException("BitArray2D sizes must match.");

            _buffer.Or(other._buffer);
        }

        /// <summary>
        /// Performs a bitwise XOR operation with another 2D bit array.
        /// Both arrays must have the same dimensions.
        /// </summary>
        /// <param name="other">The bit array to XOR with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the dimensions of the arrays do not match.</exception>
        public void Xor(SfBitArray2D other)
        {
            SfThrowHelper.ThrowIfNull(other, nameof(other));

            if (Width != other.Width || Height != other.Height)
                throw new ArgumentException("BitArray2D sizes must match.");

            _buffer.Xor(other._buffer);
        }
    }
}