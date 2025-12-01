using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Enumerators;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a two-dimensional grid that stores elements in a linear array.
    /// Provides fast indexed access and efficient memory layout for 2D data structures.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the grid.</typeparam>
    public sealed class SfGrid2D<T> : ISfDataStructure<T>
    {
        private readonly int _width, _height;

        /// <summary>
        /// The underlying linear array that stores the 2D grid data.
        /// </summary>
        private T[] _buffer;
        
        /// <summary>
        /// Gets the width (X dimension) of the grid.
        /// </summary>
        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _width;
        }
        
        /// <summary>
        /// Gets the height (Y dimension) of the grid.
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
            get => _buffer.Length;
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count == 0;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid2D{T}"/> class with the specified dimensions.
        /// </summary>
        /// <param name="width">The width (X dimension) of the grid. Must be positive.</param>
        /// <param name="height">The height (Y dimension) of the grid. Must be positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="width"/> or <paramref name="height"/> is less than or equal to zero.
        /// </exception>
        public SfGrid2D(int width, int height)
        {
            if (width <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(width));
            if (height <= 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(height));

            _width = width;
            _height = height;
            _buffer = new T[width * height];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid2D{T}"/> class with the specified dimensions and existing data array.
        /// </summary>
        /// <param name="width">The width (X dimension) of the grid. Must be positive.</param>
        /// <param name="height">The height (Y dimension) of the grid. Must be positive.</param>
        /// <param name="buffer">The backing array containing the grid data. Length must equal width × height.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="width"/> or <paramref name="height"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the data array length does not match width × height.</exception>
        public SfGrid2D(int width, int height, T[] buffer)
        {
            if (width <= 0) 
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(width));
            if (height <= 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(height));
            
            if (buffer is null)
                SfThrowHelper.ThrowArgumentNull(nameof(buffer));

            _width = width;
            _height = height;
            _buffer = buffer;
            
            if (_width * _height != buffer.Length)
                SfThrowHelper.ThrowArgument($"{nameof(buffer)} must have the same size as {nameof(Width)}x{nameof(Height)}({_width}, {_height})");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid2D{T}"/> class as a deep copy of another 2D grid.
        /// </summary>
        /// <param name="other">The source 2D grid to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public SfGrid2D(SfGrid2D<T> other)
        {
            if (other is null)
                SfThrowHelper.ThrowArgumentNull(nameof(other));
            
            _width = other._width;
            _height = other._height;
            _buffer = new T[_width * _height];
            
            Array.Copy(other._buffer, 0, _buffer, 0, _buffer.Length);
        }
        
        /// <summary>
        /// Gets or sets the element at the specified 2D coordinate.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The element at the specified coordinate.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        public T this[int x, int y]
        {
            get => _buffer[CheckedIndex(x, y)];
            set => _buffer[CheckedIndex(x, y)] = value;
        }

        /// <summary>
        /// Gets or sets the element at the specified linear index.
        /// </summary>
        /// <param name="index">The linear index in the underlying array.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the valid bounds.</exception>
        public T this[int index]
        {
            get => _buffer[CheckedIndex(index)];
            set => _buffer[CheckedIndex(index)] = value;
        }

        /// <summary>
        /// Attempts to retrieve the element at the specified 2D coordinate without throwing an exception.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="value">When this method returns, contains the element at the specified coordinate if found; otherwise, the default value.</param>
        /// <returns><c>true</c> if the coordinates are within bounds; otherwise, <c>false</c>.</returns>
        public bool TryGet(int x, int y, out T value)
        {
            if (IsInBounds(x, y))
            {
                value = _buffer[ToIndex(x, y)];
                return true;
            }

            value = default;
            return false;
        }
        
        /// <summary>
        /// Returns the underlying data array (shared reference).
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] GetRawData() => _buffer;
        
        /// <summary>
        /// Returns the underlying data array as span.
        /// </summary>
        /// <returns>The internal array containing the grid data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => _buffer.AsSpan();

        /// <summary>
        /// Returns an unsafe reference to the element at the specified 2D coordinate without bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>A reference to the element at the specified coordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetUnsafeRef(int x, int y)
        {
            return ref _buffer[y * _width + x];
        }
        
        /// <summary>
        /// Sets the element at the specified 2D coordinate without bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="value">The value to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnchecked(int x, int y, T value)
        {
            _buffer[y * _width + x] = value;
        }

        /// <summary>
        /// Converts 2D coordinates to a linear array index.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The linear index in the backing array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToIndex(int x, int y) => y * _width + x;
        
        /// <summary>
        /// Converts 2D coordinates to a linear array index with bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The linear index in the backing array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CheckedIndex(int x, int y) {
            if (!IsInBounds(x, y)) 
                SfThrowHelper.ThrowArgumentOutOfRange("index", "index is out of range.");
            return ToIndex(x, y);
        }
        
        /// <summary>
        /// Validates a linear index with bounds checking.
        /// </summary>
        /// <param name="index">The linear index to validate.</param>
        /// <returns>The validated index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CheckedIndex(int index)
        {
            if (!IsInBounds(index)) 
                SfThrowHelper.ThrowArgumentOutOfRange("index", "index is out of range.");
            return index;
        }

        /// <summary>
        /// Converts a linear array index to 2D coordinates.
        /// </summary>
        /// <param name="index">The linear index in the backing array.</param>
        /// <returns>A tuple containing the (x, y) coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y) ToCoords(int index) => (index % _width, index / _width);

        /// <summary>
        /// Determines whether the specified 2D coordinates are within the grid bounds.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns><c>true</c> if the coordinates are within bounds; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int x, int y) => (uint)x < _width && (uint)y < _height;
        
        /// <summary>
        /// Determines whether the specified linear index is within the grid bounds.
        /// </summary>
        /// <param name="index">The linear index to check.</param>
        /// <returns><c>true</c> if the index is within bounds; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int index) => (uint)index < _buffer.Length;
        
        /// <summary>
        /// Fills the entire grid with the specified value.
        /// </summary>
        /// <param name="value">The value to fill the grid with.</param>
        public void Fill(T value)
        {
            for (int i = 0; i < _buffer.Length; i++)
                _buffer[i] = value;
        }
        
        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(_buffer[i]);
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(_buffer[i], item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Clears all elements in the grid (sets them to their default value).
        /// </summary>
        public void Clear() => Array.Clear(_buffer, 0, _buffer.Length);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");

            for (int i = 0; i < Count; i++)
                array[arrayIndex++] = _buffer[i];
        }

        /// <inheritdoc/>
        public T[] ToArray()
        {
            T[] arr = new T[Count];
            CopyTo(arr, 0);
            return arr;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfArrayEnumerator<T> GetEnumerator() => new(_buffer, _buffer.Length);
        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}