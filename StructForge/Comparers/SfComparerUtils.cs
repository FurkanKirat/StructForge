using System.Collections.Generic;
using StructForge.Collections;

namespace StructForge.Comparers
{
    public static class SfComparerUtils
    {
        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            comparer ??= SfComparers<T>.DefaultComparer;
            return Comparer<T>.Create((a, b) => comparer.Compare(b, a));
        }

        public static IComparer<SfKeyValue<TKey, TValue>> CreateKeyValueComparer<TKey, TValue>(IComparer<TKey> comparer)
        {
            comparer ??= Comparer<TKey>.Default;
            return Comparer<SfKeyValue<TKey, TValue>>.Create((v1, v2) => comparer.Compare(v1.Key, v2.Key));
        }
    }
}