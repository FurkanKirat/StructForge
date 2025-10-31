using System;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) collection of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    public interface ISfStack<T> : ISfDataStructure<T>
    {
        /// <summary>
        /// Inserts an item at the top of the stack.
        /// </summary>
        /// <param name="item">The item to push onto the stack.</param>
        void Push(T item);

        /// <summary>
        /// Removes and returns the item at the top of the stack.
        /// </summary>
        /// <returns>The item removed from the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
        T Pop();

        /// <summary>
        /// Returns the item at the top of the stack without removing it.
        /// </summary>
        /// <returns>The item at the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
        T Peek();

        /// <summary>
        /// Attempts to remove and return the item at the top of the stack.
        /// </summary>
        /// <param name="item">When this method returns, contains the object removed, if successful; otherwise, the default value of T.</param>
        /// <returns>True if an item was removed; false if the stack was empty.</returns>
        bool TryPop(out T item);

        /// <summary>
        /// Attempts to return the item at the top of the stack without removing it.
        /// </summary>
        /// <param name="item">When this method returns, contains the object at the top, if successful; otherwise, the default value of T.</param>
        /// <returns>True if an item was retrieved; false if the stack was empty.</returns>
        bool TryPeek(out T item);

        /// <summary>
        /// Reduces the capacity of the stack to fit its current count, if there is excessive unused space.
        /// </summary>
        void TrimExcess();
    }
}
