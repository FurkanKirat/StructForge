using System.Collections.Generic;
using StructForge.Collections;

namespace StructForge.Extensions
{
    /// <summary>
    /// Provides extension methods to convert any <see cref="IEnumerable{T}"/> to Sf collections.
    /// </summary>
    public static class SfEnumerableExtensions
    {
        /// <summary>
        /// Converts the enumerable to an <see cref="SfList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to convert.</param>
        /// <returns>A new <see cref="SfList{T}"/> containing the elements from the enumerable.</returns>
        public static SfList<T> ToSfList<T>(this IEnumerable<T> enumerable) => new SfList<T>(enumerable);

        /// <summary>
        /// Converts the enumerable to an <see cref="SfLinkedList{T}"/>.
        /// </summary>
        public static SfLinkedList<T> ToSfLinkedList<T>(this IEnumerable<T> enumerable) => new SfLinkedList<T>(enumerable);

        /// <summary>
        /// Converts the enumerable to an <see cref="SfQueue{T}"/>.
        /// </summary>
        public static SfQueue<T> ToSfQueue<T>(this IEnumerable<T> enumerable) => new SfQueue<T>(enumerable);

        /// <summary>
        /// Converts the enumerable to an <see cref="SfStack{T}"/>.
        /// </summary>
        public static SfStack<T> ToSfStack<T>(this IEnumerable<T> enumerable) => new SfStack<T>(enumerable);

        /// <summary>
        /// Converts the enumerable to an <see cref="SfAvlTree{T}"/>.
        /// </summary>
        public static SfAvlTree<T> ToSfAvlTree<T>(this IEnumerable<T> enumerable) => new SfAvlTree<T>(enumerable);

        /// <summary>
        /// Converts the enumerable to an <see cref="SfBinaryHeap{T}"/>.
        /// </summary>
        public static SfBinaryHeap<T> ToSfBinaryHeap<T>(this IEnumerable<T> enumerable) => new SfBinaryHeap<T>(enumerable);

        /// <summary>
        /// Converts the enumerable to an <see cref="SfSortedSet{T}"/>.
        /// </summary>
        public static SfSortedSet<T> ToSfSortedSet<T>(this IEnumerable<T> enumerable) => new SfSortedSet<T>(enumerable);
    }
}
