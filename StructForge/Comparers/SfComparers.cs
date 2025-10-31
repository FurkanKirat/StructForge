#nullable enable
using System;
using System.Collections.Generic;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides generic comparer utilities for any type <typeparamref name="T"/>.
    /// Includes default, reverse, and null-safe comparers.
    /// </summary>
    /// <typeparam name="T">The type of elements to compare.</typeparam>
    public static class SfComparers<T>
    {
        /// <summary>
        /// Gets the default comparer for type <typeparamref name="T"/>.
        /// Equivalent to <see cref="Comparer{T}.Default"/>.
        /// </summary>
        public static IComparer<T> DefaultComparer { get; } = Comparer<T>.Default;

        /// <summary>
        /// Gets a comparer that reverses the order of the <see cref="DefaultComparer"/>.
        /// </summary>
        public static IComparer<T> ReverseComparer { get; } =
            Comparer<T>.Create((a, b) => DefaultComparer.Compare(b, a));
        
        
        /// <summary>
        /// Creates a comparer from a key selector function.
        /// </summary>
        public static IComparer<T> FromSelector<TKey>(Func<T, TKey> selector)
            where TKey : IComparable<TKey>
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return Comparer<T>.Create((a, b) => selector(a).CompareTo(selector(b)));
        }

        /// <summary>
        /// Creates a reversed comparer from a key selector function.
        /// </summary>
        public static IComparer<T> FromSelectorReversed<TKey>(Func<T, TKey> selector)
            where TKey : IComparable<TKey>
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return Comparer<T>.Create((a, b) => selector(b).CompareTo(selector(a)));
        }

        /// <summary>
        /// Gets a comparer that treats <c>null</c> values as greater than all non-null values.
        /// (Nulls appear last when sorting.)
        /// </summary>
        public static IComparer<T?> NullsLastComparer { get; } =
            Comparer<T?>.Create((a, b) =>
            {
                if (a is null && b is null) return 0;
                if (a is null) return 1;
                if (b is null) return -1;
                return DefaultComparer.Compare(a, b);
            });

        /// <summary>
        /// Gets a comparer that treats <c>null</c> values as less than all non-null values.
        /// (Nulls appear first when sorting.)
        /// </summary>
        public static IComparer<T?> NullsFirstComparer { get; } =
            Comparer<T?>.Create((a, b) =>
            {
                if (a is null && b is null) return 0;
                if (a is null) return -1;
                if (b is null) return 1;
                return DefaultComparer.Compare(a, b);
            });
    }
}