using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides commonly used <see cref="IEqualityComparer{String}"/> instances for different string comparison strategies.
    /// Includes ordinal, invariant culture, case-insensitive, and length-based comparers.
    /// </summary>
    public static class SfStringEqualityComparers
    {
        // --- BACKING FIELDS ---
        private static readonly IEqualityComparer<string> _ordinal = StringComparer.Ordinal;
        private static readonly IEqualityComparer<string> _ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
        private static readonly IEqualityComparer<string> _invariant = StringComparer.InvariantCulture;
        private static readonly IEqualityComparer<string> _invariantIgnoreCase = StringComparer.InvariantCultureIgnoreCase;
        private static readonly IEqualityComparer<string> _length = new SfStringLengthEqualityComparer();

        // --- PROPERTIES ---

        /// <summary>
        /// Gets an ordinal <see cref="IEqualityComparer{String}"/> that performs a case-sensitive comparison.
        /// </summary>
        public static IEqualityComparer<string> Ordinal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ordinal;
        }

        /// <summary>
        /// Gets an ordinal <see cref="IEqualityComparer{String}"/> that performs a case-insensitive comparison.
        /// </summary>
        public static IEqualityComparer<string> OrdinalIgnoreCase
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ordinalIgnoreCase;
        }

        /// <summary>
        /// Gets an invariant culture <see cref="IEqualityComparer{String}"/> that performs a case-sensitive comparison.
        /// </summary>
        public static IEqualityComparer<string> InvariantCulture
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _invariant;
        }

        /// <summary>
        /// Gets an invariant culture <see cref="IEqualityComparer{String}"/> that performs a case-insensitive comparison.
        /// </summary>
        public static IEqualityComparer<string> InvariantCultureIgnoreCase
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _invariantIgnoreCase;
        }

        /// <summary>
        /// Gets an <see cref="IEqualityComparer{String}"/> that considers strings equal if they have the same length.
        /// </summary>
        public static IEqualityComparer<string> Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
        }

        // --- CONCRETE CLASS ---

        private sealed class SfStringLengthEqualityComparer : IEqualityComparer<string>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(string x, string y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Length == y.Length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(string obj) => obj?.Length.GetHashCode() ?? 0;
        }
    }
}