using System.Collections.Generic;
using StructForge.Collections;
using StructForge.Comparers.StructForge.Helpers;

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

        public static IEqualityComparer<SfKeyValue<TKey, TValue>> CreateKeyValueComparer<TKey, TValue>(IEqualityComparer<TKey> comparer)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            return SfEqualityComparer.Create<SfKeyValue<TKey, TValue>>(
                equals:(v1, v2) => comparer.Equals(v1.Key, v2.Key),
                getHashCode:(v1) => comparer.GetHashCode(v1.Key));
        }
    }
}