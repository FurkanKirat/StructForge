namespace StructForge.Collections
{
    /// <summary>
    /// Represents a generic linked list with methods for adding, removing, and finding elements.
    /// </summary>
    /// <typeparam name="T">Type of elements stored in the linked list.</typeparam>
    public interface ISfLinkedList<T> : ISfDataStructure<T>
    {
        /// <summary>
        /// Adds an item at the beginning of the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddFirst(T item);

        /// <summary>
        /// Adds an item at the end of the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddLast(T item);

        /// <summary>
        /// Removes and returns the first element of the list.
        /// </summary>
        /// <returns>The removed element.</returns>
        T RemoveFirst();

        /// <summary>
        /// Removes and returns the last element of the list.
        /// </summary>
        /// <returns>The removed element.</returns>
        T RemoveLast();

        /// <summary>
        /// Inserts an item after the specified node.
        /// </summary>
        /// <param name="node">The node after which to insert.</param>
        /// <param name="item">The item to insert.</param>
        void InsertAfter(SfLinkedListNode<T> node, T item);

        /// <summary>
        /// Inserts an item before the specified node.
        /// </summary>
        /// <param name="node">The node before which to insert.</param>
        /// <param name="item">The item to insert.</param>
        void InsertBefore(SfLinkedListNode<T> node, T item);

        /// <summary>
        /// Removes the first occurrence of the specified item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed; otherwise, false.</returns>
        bool Remove(T item);

        /// <summary>
        /// Reverses the order of the elements in the list.
        /// </summary>
        void Reverse();

        /// <summary>
        /// Finds the first node that contains the specified item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The first node containing the item, or null if not found.</returns>
        SfLinkedListNode<T> Find(T item);

        /// <summary>
        /// Finds the last node that contains the specified item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The last node containing the item, or null if not found.</returns>
        SfLinkedListNode<T> FindLast(T item);
    }
}
