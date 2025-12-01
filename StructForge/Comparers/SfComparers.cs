using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides high-performance, allocation-free comparer instances.
    /// Uses cached backing fields with inlined properties for zero-overhead access.
    /// </summary>
    public static class SfComparers<T>
    {
        // --- BACKING FIELDS (Singleton Instances) ---
        private static readonly IComparer<T> _defaultComparer = Comparer<T>.Default;
        
        private static readonly IComparer<T> _reverseComparer = 
            new SfComparerUtils.SfReverseComparerWrapper<T>(_defaultComparer);
            
        private static readonly IComparer<T> _nullsLastComparer = new SfNullsLastComparer();
        private static readonly IComparer<T> _nullsFirstComparer = new SfNullsFirstComparer();

        // --- PUBLIC PROPERTIES (Inlined) ---

        /// <summary>
        /// Gets the default comparer for type <typeparamref name="T"/>.
        /// </summary>
        public static IComparer<T> DefaultComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _defaultComparer;
        }

        /// <summary>
        /// Gets an optimized comparer that reverses the order.
        /// </summary>
        public static IComparer<T> ReverseComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _reverseComparer;
        }

        /// <summary>
        /// Comparer that treats nulls as greater than non-nulls (Nulls at the end).
        /// </summary>
        public static IComparer<T> NullsLastComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _nullsLastComparer;
        }

        /// <summary>
        /// Comparer that treats nulls as less than non-nulls (Nulls at the start).
        /// </summary>
        public static IComparer<T> NullsFirstComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _nullsFirstComparer;
        }
        
        // --- CONCRETE IMPLEMENTATIONS ---

        private sealed class SfNullsLastComparer : IComparer<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
            {
                if (x is null && y is null) return 0;
                if (x is null) return 1;
                if (y is null) return -1;
                return _defaultComparer.Compare(x, y);
            }
        }

        private sealed class SfNullsFirstComparer : IComparer<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
            {
                if (x is null && y is null) return 0;
                if (x is null) return -1;
                if (y is null) return 1;
                return _defaultComparer.Compare(x, y);
            }
        }
    }
}