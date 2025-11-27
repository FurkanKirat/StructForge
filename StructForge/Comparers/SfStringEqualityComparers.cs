using System;
using System.Collections.Generic;
using StructForge.Comparers.StructForge.Helpers;

namespace StructForge.Comparers
{
    public static class SfStringEqualityComparers
    {
        public static IEqualityComparer<string> Ordinal { get; } = StringComparer.Ordinal;
        public static IEqualityComparer<string> OrdinalIgnoreCase { get; } = StringComparer.OrdinalIgnoreCase;
        public static IEqualityComparer<string> InvariantCulture { get; } = StringComparer.InvariantCulture;
        public static IEqualityComparer<string> InvariantCultureIgnoreCase { get; } = StringComparer.InvariantCultureIgnoreCase;
        public static IEqualityComparer<string> Length { get; } =
            SfEqualityComparer.Create<string>(
                (a, b) => a?.Length == b?.Length,
                a => a?.Length.GetHashCode() ?? 0
            );
    }
}