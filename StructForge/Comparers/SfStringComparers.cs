using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    public static class SfStringComparers
    {
        // --- BACKING FIELDS ---
        private static readonly IComparer<string> _ordinal = StringComparer.Ordinal;
        private static readonly IComparer<string> _ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
        private static readonly IComparer<string> _length = new SfStringLengthComparer();

        // --- PROPERTIES ---

        public static IComparer<string> Ordinal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ordinal;
        }

        public static IComparer<string> OrdinalIgnoreCase
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ordinalIgnoreCase;
        }

        public static IComparer<string> Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
        }

        // --- CONCRETE CLASS ---

        private sealed class SfStringLengthComparer : IComparer<string>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(string x, string y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x is null) return -1;
                if (y is null) return 1;
                return x.Length.CompareTo(y.Length);
            }
        }
    }
}