using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    public static class SfStringEqualityComparers
    {
        // --- BACKING FIELDS ---
        private static readonly IEqualityComparer<string> _ordinal = StringComparer.Ordinal;
        private static readonly IEqualityComparer<string> _ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
        private static readonly IEqualityComparer<string> _invariant = StringComparer.InvariantCulture;
        private static readonly IEqualityComparer<string> _invariantIgnoreCase = StringComparer.InvariantCultureIgnoreCase;
        private static readonly IEqualityComparer<string> _length = new SfStringLengthEqualityComparer();

        // --- PROPERTIES ---

        public static IEqualityComparer<string> Ordinal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ordinal;
        }

        public static IEqualityComparer<string> OrdinalIgnoreCase
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ordinalIgnoreCase;
        }

        public static IEqualityComparer<string> InvariantCulture
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _invariant;
        }

        public static IEqualityComparer<string> InvariantCultureIgnoreCase
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _invariantIgnoreCase;
        }

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