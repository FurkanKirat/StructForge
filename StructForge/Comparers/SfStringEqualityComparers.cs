using System;
using System.Collections.Generic;

namespace StructForge.Comparers
{
    public static class SfStringEqualityComparers
    {
        public static IEqualityComparer<string> Ordinal { get; } = StringComparer.Ordinal;
        public static IEqualityComparer<string> OrdinalIgnoreCase { get; } = StringComparer.OrdinalIgnoreCase;
        public static IEqualityComparer<string> InvariantCulture { get; } = StringComparer.InvariantCulture;
        public static IEqualityComparer<string> InvariantCultureIgnoreCase { get; } = StringComparer.InvariantCultureIgnoreCase;
        public static IEqualityComparer<string> Length { get; } =
            EqualityComparer<string>.Create(
                (a, b) => a?.Length == b?.Length,
                a => a?.Length.GetHashCode() ?? 0
            );
    }
}