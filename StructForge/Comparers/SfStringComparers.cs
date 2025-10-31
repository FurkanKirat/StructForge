using System;
using System.Collections.Generic;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides predefined string comparers for various ordering rules,
    /// including ordinal, case-insensitive, and length-based comparisons.
    /// </summary>
    public static class SfStringComparers
    {
        /// <summary>
        /// Compares two strings using ordinal (binary) comparison rules.
        /// Case-sensitive and culture-invariant.
        /// </summary>
        public static IComparer<string> Ordinal { get; } =
            StringComparer.Ordinal;

        /// <summary>
        /// Compares two strings using ordinal (binary) comparison rules,
        /// but ignores case differences.
        /// </summary>
        public static IComparer<string> OrdinalIgnoreCase { get; } =
            StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Compares two strings by their length.
        /// Null values are treated as shorter than any non-null string.
        /// </summary>
        public static IComparer<string> Length { get; } =
            Comparer<string>.Create((a, b) =>
            {
                if (a is null && b is null) return 0;
                if (a is null) return -1;
                if (b is null) return 1;
                return a.Length.CompareTo(b.Length);
            });
    }
}