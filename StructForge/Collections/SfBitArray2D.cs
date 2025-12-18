using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a two-dimensional bit array backed by a linear <see cref="SfBitArray"/>.
    /// Provides fast indexed access and efficient bitwise operations in 2D form.
    /// </summary>
    [DebuggerDisplay("SfBitArray2D(Width = {Width}, Height = {Height}, Count = {Count})")]
    [DebuggerTypeProxy(typeof(SfBitArray2DDebugView))]
    public sealed class SfBitArray2D : ISfDataStructure<bool>
    {
        private readonly int _width, _height;
        /// <summary>
        /// The underlying linear bit array that stores the 2D data.
        /// </summary>
        private readonly SfBitArray _buffer;

        /// <summary>
        /// Gets the width (X dimension) of the bit array.
        /// </summary>
        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _width;
        }

        /// <summary>
        /// Gets the height (Y dimension) of the bit array.
        /// </summary>
        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _height;
        }

        /// <inheritdoc/>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer.Count;
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count == 0; 
        }

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
            if (width <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(width));
            
            if (height <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(height));

            _width = width;
            _height = height;
            _buffer = new SfBitArray(width * height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfBitArray2D"/> class as a deep copy of another 2D bit array.
        /// </summary>
        /// <param name="other">The source 2D bit array to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public SfBitArray2D(SfBitArray2D other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            _width = other._width;
            _height = other._height;
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
            if (bits is null)
                SfThrowHelper.ThrowArgumentNull(nameof(bits));
            
            if (width <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(width));
            if (height <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(height));

            int totalBits = width * height;
            int requiredUlongs = (totalBits + 63) / 64;

            if (bits.Length != requiredUlongs)
                SfThrowHelper.ThrowArgument(
                    $"Invalid backing buffer size. Expected {requiredUlongs}, got {bits.Length}.");

            _width = width;
            _height = height;
            _buffer = new SfBitArray(bits, totalBits);
        }
        
        /// <summary>
        /// Returns the underlying data array as span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<ulong> AsSpan() => _buffer.AsSpan();
        
        /// <summary>
        /// Returns the underlying data array as readonly span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<ulong> AsReadOnlySpan() => _buffer.AsReadOnlySpan();

        /// <summary>
        /// Calculates the linear bit index corresponding to a 2D coordinate.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The linear index in the backing buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexSafe(int x, int y)
        {
            if ((uint)x >= (uint)_width)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(x));
            if ((uint)y >= (uint)_height)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(y));

            return y * _width + x;
        }
        
        /// <summary>
        /// Calculates the linear bit index corresponding to a 2D coordinate without index check.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The linear index in the backing buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexUnsafe(int x, int y) => y * _width + x;
        

        /// <summary>
        /// Gets or sets the bit value at the specified 2D coordinate.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The boolean value at the specified coordinate.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        public bool this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer[IndexSafe(x, y)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _buffer[IndexSafe(x, y)] = value;
        }

        /// <summary>
        /// Returns the bit value at the specified index without bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The boolean value at the specified index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetUnchecked(int x, int y) => _buffer.GetUnchecked(IndexUnsafe(x,y));
        
        /// <summary>
        /// Sets the bit value at the specified index without bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="value">The value to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnchecked(int x, int y, bool value) => _buffer.SetUnchecked(IndexUnsafe(x,y), value);

        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfBitArray.SfBitArrayEnumerator GetEnumerator() => new(_buffer);
        /// <inheritdoc/>
        IEnumerator<bool> IEnumerable<bool>.GetEnumerator() => _buffer.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<bool> action) => _buffer.ForEach(action);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(bool item) => _buffer.Contains(item);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(bool item, IEqualityComparer<bool> comparer) => _buffer.Contains(item, comparer);

        /// <summary>
        /// Clears all bits in the array (sets them to false).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => _buffer.Clear();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(bool[] array, int arrayIndex) => _buffer.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public bool[] ToArray()
        {
            bool[] arr = new bool[Count];
            CopyTo(arr, 0);
            return arr;
        }

        /// <summary>
        /// Sets all bits in the array to the specified value.
        /// </summary>
        /// <param name="value">The value to set all bits to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAll(bool value) => _buffer.SetAll(value);

        /// <summary>
        /// Counts the number of bits set to true.
        /// </summary>
        /// <returns>The number of true bits in the array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountTrue() => _buffer.CountTrue();

        /// <summary>
        /// Counts the number of bits set to false.
        /// </summary>
        /// <returns>The number of false bits in the array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountFalse() => _buffer.CountFalse();

        /// <summary>
        /// Toggles the bit at the specified 2D coordinate (true becomes false, false becomes true).
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Toggle(int x, int y) => _buffer.Toggle(IndexSafe(x, y));

        /// <summary>
        /// Inverts all bits in the array (true becomes false, false becomes true).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Not() => _buffer.Not();

        /// <summary>
        /// Performs a bitwise AND operation with another 2D bit array.
        /// Both arrays must have the same dimensions.
        /// </summary>
        /// <param name="other">The bit array to AND with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the dimensions of the arrays do not match.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void And(SfBitArray2D other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            if (_width != other._width || _height != other._height)
                SfThrowHelper.ThrowArgument("BitArray2D sizes must match.");

            _buffer.And(other._buffer);
        }
        
        /// <summary>
        /// Performs a bitwise AND NOT operation with another 2D bit array.
        /// Both arrays must have the same dimensions.
        /// </summary>
        /// <param name="other">The bit array to AND NOT with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the dimensions of the arrays do not match.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AndNot(SfBitArray2D other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            if (_width != other._width || _height != other._height)
                SfThrowHelper.ThrowArgument("BitArray2D sizes must match.");

            _buffer.AndNot(other._buffer);
        }

        /// <summary>
        /// Performs a bitwise OR operation with another 2D bit array.
        /// Both arrays must have the same dimensions.
        /// </summary>
        /// <param name="other">The bit array to OR with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the dimensions of the arrays do not match.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Or(SfBitArray2D other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            if (_width != other._width || _height != other._height)
                SfThrowHelper.ThrowArgument("BitArray2D sizes must match.");

            _buffer.Or(other._buffer);
        }

        /// <summary>
        /// Performs a bitwise XOR operation with another 2D bit array.
        /// Both arrays must have the same dimensions.
        /// </summary>
        /// <param name="other">The bit array to XOR with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the dimensions of the arrays do not match.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Xor(SfBitArray2D other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));

            if (_width != other._width || _height != other._height)
                SfThrowHelper.ThrowArgument("BitArray2D sizes must match.");

            _buffer.Xor(other._buffer);
        }
        
        /// <summary>
        /// Provides a custom debugger view for <see cref="SfBitArray2D"/> displaying elements.
        /// </summary>
        private sealed class SfBitArray2DDebugView
        {
            private readonly SfBitArray2D _array;
            public SfBitArray2DDebugView(SfBitArray2D array) { _array = array; }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public bool[][] Items
            {
                get
                {
                    int sizeX = _array.Width;
                    int sizeY = _array.Height;

                    bool[][] view = new bool[sizeX][];
                    for (int x = 0; x < sizeX; x++)
                    {
                        view[x] = new bool[sizeY];
                        for (int y = 0; y < sizeY; y++)
                        {
                            view[x][y] = _array[x, y];
                        }
                    }
                    return view;
                }
            }
        }
    }
}