using System;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a generic heap (priority queue) data structure.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the heap.</typeparam>
    public interface IHeap<T> : IDataStructure<T>
    {
        /// <summary>
        /// Adds an item to the heap while maintaining the heap property.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void Add(T item);

        /// <summary>
        /// Returns the element at the top of the heap without removing it.
        /// </summary>
        /// <returns>The top element of the heap.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the heap is empty.</exception>
        T Peek();

        /// <summary>
        /// Removes and returns the element at the top of the heap.
        /// </summary>
        /// <returns>The removed top element.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the heap is empty.</exception>
        T Pop();
    }

}