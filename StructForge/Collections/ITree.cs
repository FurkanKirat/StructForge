using System;
using System.Collections.Generic;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a tree data structure with traversal capabilities.
    /// </summary>
    public interface ITree<T> : IDataStructure<T>
    {
        /// <summary>
        /// Gets the height of the tree.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Attempts to add an item to the tree.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if added; false if duplicate or insertion failed.</returns>
        bool TryAdd(T item);

        /// <summary>
        /// Returns the minimum value in the tree.
        /// </summary>
        /// <returns>The minimum item.</returns>
        /// <exception cref="InvalidOperationException">Thrown if tree is empty.</exception>
        T FindMin();

        /// <summary>
        /// Returns the maximum value in the tree.
        /// </summary>
        /// <returns>The maximum item.</returns>
        /// <exception cref="InvalidOperationException">Thrown if tree is empty.</exception>
        T FindMax();

        /// <summary>
        /// Traverses the tree in-order.
        /// </summary>
        IEnumerable<T> InOrder();

        /// <summary>
        /// Traverses the tree pre-order.
        /// </summary>
        IEnumerable<T> PreOrder();

        /// <summary>
        /// Traverses the tree post-order.
        /// </summary>
        IEnumerable<T> PostOrder();
    }

}