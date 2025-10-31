using System;
using System.Collections.Generic;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a generic data structure.
    /// </summary>
    /// <typeparam name="T">The type of elements in the data structure.</typeparam>
    public interface ISfDataStructure<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Returns true if the collection contains no elements.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Executes the specified action on each element of the collection.
        /// </summary>
        /// <param name="action">The action to perform on each element.</param>
        void ForEach(Action<T> action);

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>True if item exists; otherwise false.</returns>
        bool Contains(T item);

        /// <summary>
        /// Determines whether the collection contains a specific value using a custom comparer.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <returns>True if item exists; otherwise false.</returns>
        bool Contains(T item, IEqualityComparer<T> comparer);

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Copies the elements of the collection to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">Destination array.</param>
        /// <param name="arrayIndex">Zero-based index at which copying begins.</param>
        void CopyTo(T[] array, int arrayIndex);
    }
}