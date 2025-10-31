using System.Collections.Generic;
using StructForge.Collections;

namespace StructForge.Extensions
{
    public static class SfEnumerableExtensions
    {
        public static SfList<T> ToSfList<T>(this IEnumerable<T> enumerable) => new SfList<T>(enumerable);
        public static SfLinkedList<T> ToSfLinkedList<T>(this IEnumerable<T> enumerable) => new SfLinkedList<T>(enumerable);
        public static SfQueue<T> ToSfQueue<T>(this IEnumerable<T> enumerable) => new SfQueue<T>(enumerable);
        public static SfStack<T> ToSfStack<T>(this IEnumerable<T> enumerable) => new SfStack<T>(enumerable);
    }
}