using System;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a first-in-first-out (FIFO) collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queue.</typeparam>
    public interface ISfQueue<T> : ISfDataStructure<T>
    {
        /// <summary>
        /// Adds an item to the end of the queue.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void Enqueue(T item);

        /// <summary>
        /// Removes and returns the item at the front of the queue.
        /// </summary>
        /// <returns>The item removed from the front.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        T Dequeue();

        /// <summary>
        /// Returns the item at the front of the queue without removing it.
        /// </summary>
        /// <returns>The item at the front.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        T Peek();

        /// <summary>
        /// Attempts to remove and return the item at the front of the queue.
        /// </summary>
        /// <param name="item">When this method returns, contains the removed item if successful; otherwise, default(T).</param>
        /// <returns>True if an item was removed; false if the queue was empty.</returns>
        bool TryDequeue(out T item);

        /// <summary>
        /// Attempts to return the item at the front without removing it.
        /// </summary>
        /// <param name="item">When this method returns, contains the item at the front if successful; otherwise, default(T).</param>
        /// <returns>True if an item was retrieved; false if the queue was empty.</returns>
        bool TryPeek(out T item);

        /// <summary>Returns the last item without removing it. Throws if empty.</summary>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        T PeekLast();

        /// <summary>Attempts to return the last item without removing it. Returns false if empty.</summary>
        bool TryPeekLast(out T item);

    }
}