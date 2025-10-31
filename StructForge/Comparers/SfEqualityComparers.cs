using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    public static class SfEqualityComparers<T> 
    {
        /// <summary>Default equality comparer for type T.</summary>
        public static IEqualityComparer<T> Default { get; } = EqualityComparer<T>.Default;

        /// <summary>Compares by object reference (address equality).</summary>
        public static IEqualityComparer<T> Reference { get; } =
            EqualityComparer<T>.Create((a, b) => ReferenceEquals(a, b),
                a => RuntimeHelpers.GetHashCode(a));

        /// <summary>Creates a comparer that compares by a key selector.</summary>
        public static IEqualityComparer<T> ByKey<TKey>(Func<T, TKey> selector, IEqualityComparer<TKey> keyComparer = null)
        {
            keyComparer ??= EqualityComparer<TKey>.Default;
            return EqualityComparer<T>.Create(
                (a, b) => keyComparer.Equals(selector(a), selector(b)),
                a => keyComparer.GetHashCode(selector(a))
            );
        }
    }
}