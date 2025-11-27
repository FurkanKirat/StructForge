using System.Collections.Generic;

namespace StructForge.Collections
{
    /// <summary>
    /// Represents a generic sorted set data structure that provides
    /// efficient element lookup, insertion, and removal with ordering.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public interface ISfSet<T> : ISfDataStructure<T>
    {
        /// <summary>
        /// Attempts to add an element to the set.
        /// Returns <see langword="true"/> if the element was added successfully,
        /// or <see langword="false"/> if it already exists.
        /// </summary>
        bool TryAdd(T item);

        /// <summary>
        /// Modifies the current set to contain all elements that are present in
        /// either the current set or the specified collection.
        /// </summary>
        void UnionWith(IEnumerable<T> other);

        /// <summary>
        /// Modifies the current set to contain only elements that are also
        /// contained in the specified collection.
        /// </summary>
        void IntersectWith(IEnumerable<T> other);

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        void ExceptWith(IEnumerable<T> other);

        /// <summary>
        /// Modifies the current set to contain only elements that are present
        /// in either the current set or the specified collection, but not both.
        /// </summary>
        void SymmetricExceptWith(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set is a subset of a specified collection.
        /// </summary>
        bool IsSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        bool IsSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set and the specified collection share
        /// any common elements.
        /// </summary>
        bool Overlaps(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set contains the same elements as
        /// the specified collection.
        /// </summary>
        bool SetEquals(IEnumerable<T> other);

        /// <summary>
        /// Tries to retrieve the actual stored value that is equal to the specified value
        /// based on the set's equality or comparison logic.
        /// </summary>
        /// <param name="equalValue">The value to search for in the set.</param>
        /// <param name="actualValue">The actual stored value if found; otherwise, the default value of T.</param>
        /// <returns>True if a matching value is found; otherwise, false.</returns>
        bool TryGetValue(T equalValue, out T actualValue);
    }
}
