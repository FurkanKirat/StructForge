using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides high-performance numeric comparers for common types.
    /// Uses "Backing Field + Inlined Property" pattern to eliminate access overhead.
    /// </summary>
    public static class SfNumberComparers
    {
        // ============================================================
        // INT (Int32)
        // ============================================================
        
        private static readonly IComparer<int> _intAbsolute = new SfIntAbsComparer();
        private static readonly IComparer<int> _intSign = new SfIntSignComparer();

        public static IComparer<int> IntAbsolute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _intAbsolute;
        }

        public static IComparer<int> IntSign
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _intSign;
        }

        private sealed class SfIntAbsComparer : IComparer<int>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(int x, int y) => Math.Abs(x).CompareTo(Math.Abs(y));
        }

        private sealed class SfIntSignComparer : IComparer<int>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(int x, int y) => Math.Sign(x).CompareTo(Math.Sign(y));
        }

        // ============================================================
        // DOUBLE
        // ============================================================

        private static readonly IComparer<double> _doubleAbsolute = new SfDoubleAbsComparer();
        private static readonly IComparer<double> _doubleSign = new SfDoubleSignComparer();

        public static IComparer<double> DoubleAbsolute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _doubleAbsolute;
        }

        public static IComparer<double> DoubleSign
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _doubleSign;
        }

        private sealed class SfDoubleAbsComparer : IComparer<double>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(double x, double y) => Math.Abs(x).CompareTo(Math.Abs(y));
        }

        private sealed class SfDoubleSignComparer : IComparer<double>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(double x, double y) => Math.Sign(x).CompareTo(Math.Sign(y));
        }

        // ============================================================
        // FLOAT (Single)
        // ============================================================

        private static readonly IComparer<float> _floatAbsolute = new SfFloatAbsComparer();
        private static readonly IComparer<float> _floatSign = new SfFloatSignComparer();

        public static IComparer<float> FloatAbsolute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _floatAbsolute;
        }

        public static IComparer<float> FloatSign
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _floatSign;
        }

        private sealed class SfFloatAbsComparer : IComparer<float>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(float x, float y) => Math.Abs(x).CompareTo(Math.Abs(y));
        }

        private sealed class SfFloatSignComparer : IComparer<float>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(float x, float y) => Math.Sign(x).CompareTo(Math.Sign(y));
        }

        // ============================================================
        // LONG (Int64)
        // ============================================================

        private static readonly IComparer<long> _longAbsolute = new SfLongAbsComparer();
        private static readonly IComparer<long> _longSign = new SfLongSignComparer();

        public static IComparer<long> LongAbsolute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _longAbsolute;
        }

        public static IComparer<long> LongSign
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _longSign;
        }

        private sealed class SfLongAbsComparer : IComparer<long>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(long x, long y) => Math.Abs(x).CompareTo(Math.Abs(y));
        }

        private sealed class SfLongSignComparer : IComparer<long>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(long x, long y) => Math.Sign(x).CompareTo(Math.Sign(y));
        }
    }
}