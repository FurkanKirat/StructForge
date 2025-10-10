using System;
using System.Collections.Generic;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a random-access, indexed collection (like a list).
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    public interface ISequence<T> : IDataStructure<T>
    {
        /// <summary>
        /// Adds an item to the end of the sequence.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void Add(T item);

        /// <summary>
        /// Removes the first occurrence of a specific item from the sequence.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; false if not found.</returns>
        bool Remove(T item);

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if index is invalid.</exception>
        T this[int index] { get; set; }
        
        T First { get; set; }
        T Last { get; set; }
        
        /// <summary>
        /// Returns the index of the first occurrence of a specific item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The index of the item if found; otherwise -1.</returns>
        int IndexOf(T item);

        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if index is invalid.</exception>
        void Insert(int index, T item);

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if index is invalid.</exception>
        void RemoveAt(int index);

        /// <summary>
        /// Sorts the array
        /// </summary>
        void Sort();
        
        /// <summary>
        /// Sorts the array according to comparer
        /// </summary>
        void Sort(IComparer<T> comparer);
        
        /// <summary>
        /// Swaps the two variables at given indexes
        /// </summary>
        /// <param name="i">first index</param>
        /// <param name="j">second index</param>
        void Swap(int i, int j);

    }

}