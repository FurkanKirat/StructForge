using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides commonly used <see cref="IEqualityComparer{T}"/> instances for numeric types.
    /// Supports epsilon-based comparison for floating point numbers and sign-based comparison for integers.
    /// </summary>
    public static class SfNumberEqualityComparers
    {
        // --- BACKING FIELDS ---
        private static readonly IEqualityComparer<int> _sign = new SfIntSignEqualityComparer();
        
        // --- PROPERTIES (Backing Field + Inline) ---
        
        /// <summary>
        /// Returns an <see cref="IEqualityComparer{Double}"/> that compares doubles using a specified epsilon tolerance.
        /// </summary>
        /// <param name="epsilon">The maximum difference allowed for two doubles to be considered equal. Default is 1e-6.</param>
        /// <returns>A comparer that considers doubles equal if their difference is less than or equal to <paramref name="epsilon"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<double> Epsilon(double epsilon = 1e-6) 
            => new SfDoubleEpsilonComparer(epsilon);

        /// <summary>
        /// Returns an <see cref="IEqualityComparer{Single}"/> that compares floats using a specified epsilon tolerance.
        /// </summary>
        /// <param name="epsilon">The maximum difference allowed for two floats to be considered equal. Default is 1e-5f.</param>
        /// <returns>A comparer that considers floats equal if their difference is less than or equal to <paramref name="epsilon"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<float> EpsilonFloat(float epsilon = 1e-5f) 
            => new SfFloatEpsilonComparer(epsilon);
        
        /// <summary>
        /// Gets an <see cref="IEqualityComparer{Int32}"/> that considers integers equal if they have the same sign.
        /// </summary>
        public static IEqualityComparer<int> Sign
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _sign;
        }
        
        // --- CONCRETE CLASSES ---

        private sealed class SfDoubleEpsilonComparer : IEqualityComparer<double>
        {
            private readonly double _epsilon;
            public SfDoubleEpsilonComparer(double epsilon) => _epsilon = epsilon;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(double x, double y) => Math.Abs(x - y) <= _epsilon;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(double obj) => obj.GetHashCode();
        }

        private sealed class SfFloatEpsilonComparer : IEqualityComparer<float>
        {
            private readonly float _epsilon;
            public SfFloatEpsilonComparer(float epsilon) => _epsilon = epsilon;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(float x, float y) => Math.Abs(x - y) <= _epsilon;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(float obj) => obj.GetHashCode();
        }

        private sealed class SfIntSignEqualityComparer : IEqualityComparer<int>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(int x, int y) => Math.Sign(x) == Math.Sign(y);
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(int obj) => Math.Sign(obj);
        }
    }
}