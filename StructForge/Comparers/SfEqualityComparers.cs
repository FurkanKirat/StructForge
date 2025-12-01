using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides high-performance equality comparers.
    /// Uses cached backing fields with inlined properties for zero-overhead access.
    /// </summary>
    public static class SfEqualityComparers<T> 
    {
        // --- BACKING FIELDS ---
        private static readonly IEqualityComparer<T> _default = EqualityComparer<T>.Default;
        private static readonly IEqualityComparer<T> _reference = new SfReferenceEqualityComparer();

        // --- PUBLIC PROPERTIES ---

        /// <summary>
        /// Default equality comparer using <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        public static IEqualityComparer<T> Default
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _default;
        }

        /// <summary>
        /// Highly optimized reference equality comparer.
        /// Ignores <see cref="object.Equals(object)"/> overrides.
        /// </summary>
        public static IEqualityComparer<T> Reference
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _reference;
        }

        // --- CONCRETE IMPLEMENTATION ---

        private sealed class SfReferenceEqualityComparer : IEqualityComparer<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(T x, T y) => ReferenceEquals(x, y);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}