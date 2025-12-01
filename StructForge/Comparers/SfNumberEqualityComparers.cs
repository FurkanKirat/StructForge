using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    public static class SfNumberEqualityComparers
    {
        // --- BACKING FIELDS ---
        private static readonly IEqualityComparer<int> _sign = new SfIntSignEqualityComparer();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<double> Epsilon(double epsilon = 1e-6) 
            => new SfDoubleEpsilonComparer(epsilon);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEqualityComparer<float> EpsilonFloat(float epsilon = 1e-5f) 
            => new SfFloatEpsilonComparer(epsilon);

        // --- PROPERTIES (Backing Field + Inline) ---

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