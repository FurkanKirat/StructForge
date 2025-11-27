using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a three-dimensional grid that stores elements in a linear array.
    /// Provides fast indexed access and efficient memory layout for 3D data structures.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the grid.</typeparam>
    public class SfGrid3D<T> : ISfDataStructure<T>
    {
        /// <summary>
        /// The underlying linear array that stores the 3D grid data.
        /// </summary>
        private T[] _data;
        
        /// <summary>
        /// Gets the width (X dimension) of the grid.
        /// </summary>
        public int Width { get; }
        
        /// <summary>
        /// Gets the height (Y dimension) of the grid.
        /// </summary>
        public int Height { get; }
        
        /// <summary>
        /// Gets the depth (Z dimension) of the grid.
        /// </summary>
        public int Depth { get; }
        
        /// <summary>
        /// Gets the precalculated product of width and height, used for optimizing index calculations.
        /// </summary>
        public int WidthTimesHeight { get; }
        
        /// <inheritdoc/>
        public int Count => _data.Length;
        
        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid3D{T}"/> class with the specified dimensions.
        /// </summary>
        /// <param name="width">The width (X dimension) of the grid. Must be positive.</param>
        /// <param name="height">The height (Y dimension) of the grid. Must be positive.</param>
        /// <param name="depth">The depth (Z dimension) of the grid. Must be positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="width"/>, <paramref name="height"/>, or <paramref name="depth"/> is less than or equal to zero.
        /// </exception>
        public SfGrid3D(int width, int height, int depth)
        {
            SfThrowHelper.ThrowIfNonPositive(width, nameof(width));
            SfThrowHelper.ThrowIfNonPositive(height, nameof(height));
            SfThrowHelper.ThrowIfNonPositive(depth, nameof(depth));

            Width = width;
            Height = height;
            Depth = depth;
            WidthTimesHeight = Width * Height;
            
            _data = new T[WidthTimesHeight * depth];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid3D{T}"/> class with the specified dimensions and existing data array.
        /// </summary>
        /// <param name="width">The width (X dimension) of the grid. Must be positive.</param>
        /// <param name="height">The height (Y dimension) of the grid. Must be positive.</param>
        /// <param name="depth">The depth (Z dimension) of the grid. Must be positive.</param>
        /// <param name="data">The backing array containing the grid data. Length must equal width × height × depth.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="width"/>, <paramref name="height"/>, or <paramref name="depth"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the data array length does not match width × height × depth.</exception>
        public SfGrid3D(int width, int height, int depth, T[] data)
        {
            SfThrowHelper.ThrowIfNonPositive(width, nameof(width));
            SfThrowHelper.ThrowIfNonPositive(height, nameof(height));
            SfThrowHelper.ThrowIfNonPositive(depth, nameof(depth));
            SfThrowHelper.ThrowIfNull(data, nameof(data));

            Width = width;
            Height = height;
            Depth = depth;
            WidthTimesHeight = Width * Height;
            _data = data;
            
            if (WidthTimesHeight * Depth != data.Length)
                throw new ArgumentException($"{nameof(data)} must have the same size as {nameof(Width)}x{nameof(Height)}x{nameof(Depth)}({Width}, {Height}, {Depth})");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid3D{T}"/> class as a deep copy of another 3D grid.
        /// </summary>
        /// <param name="other">The source 3D grid to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public SfGrid3D(SfGrid3D<T> other)
        {
            SfThrowHelper.ThrowIfNull(other, nameof(other));
            
            Width = other.Width;
            Height = other.Height;
            Depth = other.Depth;
            WidthTimesHeight = Width * Height;
            
            _data = new T[WidthTimesHeight * Depth];
            Array.Copy(other._data, 0, _data, 0, _data.Length);
        }

        /// <summary>
        /// Gets or sets the element at the specified 3D coordinate.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="z">The Z coordinate (depth index).</param>
        /// <returns>The element at the specified coordinate.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any coordinate is outside the valid bounds.</exception>
        public T this[int x, int y, int z]
        {
            get => _data[CheckedIndex(x, y, z)];
            set => _data[CheckedIndex(x, y, z)] = value;
        }

        /// <summary>
        /// Gets or sets the element at the specified linear index.
        /// </summary>
        /// <param name="index">The linear index in the underlying array.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the valid bounds.</exception>
        public T this[int index]
        {
            get => _data[CheckedIndex(index)];
            set => _data[CheckedIndex(index)] = value;
        }

        /// <summary>
        /// Attempts to retrieve the element at the specified 3D coordinate without throwing an exception.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="z">The Z coordinate (depth index).</param>
        /// <param name="value">When this method returns, contains the element at the specified coordinate if found; otherwise, the default value.</param>
        /// <returns><c>true</c> if the coordinates are within bounds; otherwise, <c>false</c>.</returns>
        public bool TryGet(int x, int y, int z, out T value)
        {
            if (IsInBounds(x, y, z))
            {
                value = _data[ToIndex(x, y, z)];
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
        public T[] GetRawData() => _data;

        /// <summary>
        /// Returns an unsafe reference to the element at the specified 3D coordinate without bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="z">The Z coordinate (depth index).</param>
        /// <returns>A reference to the element at the specified coordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetUnsafeRef(int x, int y, int z)
        {
            return ref _data[ToIndex(x,y,z)];
        }
        
        /// <summary>
        /// Sets the element at the specified 3D coordinate without bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="z">The Z coordinate (depth index).</param>
        /// <param name="value">The value to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnchecked(int x, int y, int z, T value)
        {
            _data[ToIndex(x,y,z)] = value;
        }

        /// <summary>
        /// Converts 3D coordinates to a linear array index.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="z">The Z coordinate (depth index).</param>
        /// <returns>The linear index in the backing array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToIndex(int x, int y, int z) => z * WidthTimesHeight + y * Width + x;
        
        /// <summary>
        /// Converts 3D coordinates to a linear array index with bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="z">The Z coordinate (depth index).</param>
        /// <returns>The linear index in the backing array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any coordinate is outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CheckedIndex(int x, int y, int z) {
            if (!IsInBounds(x, y, z)) throw new ArgumentOutOfRangeException();
            return ToIndex(x, y, z);
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
            if (!IsInBounds(index)) throw new ArgumentOutOfRangeException();
            return index;
        }

        /// <summary>
        /// Converts a linear array index to 3D coordinates.
        /// </summary>
        /// <param name="index">The linear index in the backing array.</param>
        /// <returns>A tuple containing the (x, y, z) coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y, int z) ToCoords(int index)
        {
            int z = index / WidthTimesHeight;
            int d2 = index % WidthTimesHeight;
            int x = d2 % Width;
            int y = d2 / Width;
            return (x, y, z);
        }

        /// <summary>
        /// Determines whether the specified 3D coordinates are within the grid bounds.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <param name="z">The Z coordinate (depth index).</param>
        /// <returns><c>true</c> if the coordinates are within bounds; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int x, int y, int z) => x >= 0 && x < Width && y >= 0 && y < Height  && z >= 0 && z < Depth;
        
        /// <summary>
        /// Determines whether the specified linear index is within the grid bounds.
        /// </summary>
        /// <param name="index">The linear index to check.</param>
        /// <returns><c>true</c> if the index is within bounds; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int index) => index >= 0 && index < _data.Length;
        
        /// <summary>
        /// Fills the entire grid with the specified value.
        /// </summary>
        /// <param name="value">The value to fill the grid with.</param>
        public void Fill(T value)
        {
            for (int i = 0; i < _data.Length; i++)
                _data[i] = value;
        }
        
        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(_data[i]);
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(_data[i], item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Clears all elements in the grid (sets them to their default value).
        /// </summary>
        public void Clear() => Array.Clear(_data, 0, _data.Length);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array is not large enough.");

            for (int i = 0; i < Count; i++)
                array[arrayIndex++] = _data[i];
        }
        
        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _data[i];
            }
        }
        
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}