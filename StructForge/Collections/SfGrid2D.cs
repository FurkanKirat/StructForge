using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a two-dimensional grid that stores elements in a linear array.
    /// Provides fast indexed access and efficient memory layout for 2D data structures.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the grid.</typeparam>
    public class SfGrid2D<T> : ISfDataStructure<T>
    {
        /// <summary>
        /// Gets the width (X dimension) of the grid.
        /// </summary>
        public int Width { get; }
        
        /// <summary>
        /// Gets the height (Y dimension) of the grid.
        /// </summary>
        public int Height { get; }
        
        /// <inheritdoc/>
        public int Count => Data.Length;
        
        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;
        
        /// <summary>
        /// The underlying linear array that stores the 2D grid data.
        /// </summary>
        private T[] Data { get; }

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
            SfThrowHelper.ThrowIfNonPositive(width, nameof(width));
            SfThrowHelper.ThrowIfNonPositive(height, nameof(height));

            Width = width;
            Height = height;
            Data = new T[width * height];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid2D{T}"/> class with the specified dimensions and existing data array.
        /// </summary>
        /// <param name="width">The width (X dimension) of the grid. Must be positive.</param>
        /// <param name="height">The height (Y dimension) of the grid. Must be positive.</param>
        /// <param name="data">The backing array containing the grid data. Length must equal width × height.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="width"/> or <paramref name="height"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the data array length does not match width × height.</exception>
        public SfGrid2D(int width, int height, T[] data)
        {
            SfThrowHelper.ThrowIfNonPositive(width, nameof(width));
            SfThrowHelper.ThrowIfNonPositive(height, nameof(height));
            SfThrowHelper.ThrowIfNull(data, nameof(data));

            Width = width;
            Height = height;
            Data = data;
            
            if (Width * Height != data.Length)
                throw new ArgumentException($"{nameof(data)} must have the same size as {nameof(Width)}x{nameof(Height)}({Width}, {Height})");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfGrid2D{T}"/> class as a deep copy of another 2D grid.
        /// </summary>
        /// <param name="other">The source 2D grid to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public SfGrid2D(SfGrid2D<T> other)
        {
            SfThrowHelper.ThrowIfNull(other, nameof(other));
            
            Width = other.Width;
            Height = other.Height;
            Data = new T[Width * Height];
            
            Array.Copy(other.Data, 0, Data, 0, Data.Length);
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
            get => Data[CheckedIndex(x, y)];
            set => Data[CheckedIndex(x, y)] = value;
        }

        /// <summary>
        /// Gets or sets the element at the specified linear index.
        /// </summary>
        /// <param name="index">The linear index in the underlying array.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the valid bounds.</exception>
        public T this[int index]
        {
            get => Data[CheckedIndex(index)];
            set => Data[CheckedIndex(index)] = value;
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
                value = Data[ToIndex(x, y)];
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
        public T[] GetRawData() => Data;

        /// <summary>
        /// Returns an unsafe reference to the element at the specified 2D coordinate without bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>A reference to the element at the specified coordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetUnsafeRef(int x, int y)
        {
            return ref Data[y * Width + x];
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
            Data[y * Width + x] = value;
        }

        /// <summary>
        /// Converts 2D coordinates to a linear array index.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The linear index in the backing array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToIndex(int x, int y) => y * Width + x;
        
        /// <summary>
        /// Converts 2D coordinates to a linear array index with bounds checking.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns>The linear index in the backing array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are outside the valid bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CheckedIndex(int x, int y) {
            if (!IsInBounds(x, y)) throw new ArgumentOutOfRangeException();
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
            if (!IsInBounds(index)) throw new ArgumentOutOfRangeException();
            return index;
        }

        /// <summary>
        /// Converts a linear array index to 2D coordinates.
        /// </summary>
        /// <param name="index">The linear index in the backing array.</param>
        /// <returns>A tuple containing the (x, y) coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y) ToCoords(int index) => (index % Width, index / Width);

        /// <summary>
        /// Determines whether the specified 2D coordinates are within the grid bounds.
        /// </summary>
        /// <param name="x">The X coordinate (column index).</param>
        /// <param name="y">The Y coordinate (row index).</param>
        /// <returns><c>true</c> if the coordinates are within bounds; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        
        /// <summary>
        /// Determines whether the specified linear index is within the grid bounds.
        /// </summary>
        /// <param name="index">The linear index to check.</param>
        /// <returns><c>true</c> if the index is within bounds; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int index) => index >= 0 && index < Data.Length;
        
        /// <summary>
        /// Fills the entire grid with the specified value.
        /// </summary>
        /// <param name="value">The value to fill the grid with.</param>
        public void Fill(T value)
        {
            for (int i = 0; i < Data.Length; i++)
                Data[i] = value;
        }
        
        /// <inheritdoc/>
        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(Data[i]);
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => Contains(item, SfEqualityComparers<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(Data[i], item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Clears all elements in the grid (sets them to their default value).
        /// </summary>
        public void Clear() => Array.Clear(Data, 0, Data.Length);

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
                array[arrayIndex++] = Data[i];
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return Data[i];
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}