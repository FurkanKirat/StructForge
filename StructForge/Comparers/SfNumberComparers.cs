using System;
using System.Collections.Generic;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides numeric comparers for common number types
    /// such as <see cref="double"/>, <see cref="float"/>, <see cref="int"/>, and <see cref="long"/>.
    /// Includes absolute value and sign-based comparisons.
    /// </summary>
    public static class SfNumberComparers
    {
        /// <summary>
        /// Compares two <see cref="double"/> values by their absolute values.
        /// </summary>
        public static IComparer<double> DoubleAbsolute { get; } =
            Comparer<double>.Create((a, b) => Math.Abs(a).CompareTo(Math.Abs(b)));

        /// <summary>
        /// Compares two <see cref="double"/> values by their signs (-1, 0, 1).
        /// </summary>
        public static IComparer<double> DoubleSignComparer { get; } =
            Comparer<double>.Create((a, b) => Math.Sign(a).CompareTo(Math.Sign(b)));

        /// <summary>
        /// Compares two <see cref="float"/> values by their absolute values.
        /// </summary>
        public static IComparer<float> FloatAbsolute { get; } =
            Comparer<float>.Create((a, b) => Math.Abs(a).CompareTo(Math.Abs(b)));

        /// <summary>
        /// Compares two <see cref="float"/> values by their signs (-1, 0, 1).
        /// </summary>
        public static IComparer<float> FloatSignComparer { get; } =
            Comparer<float>.Create((a, b) => Math.Sign(a).CompareTo(Math.Sign(b)));

        /// <summary>
        /// Compares two <see cref="int"/> values by their absolute values.
        /// </summary>
        public static IComparer<int> IntAbsolute { get; } =
            Comparer<int>.Create((a, b) => Math.Abs(a).CompareTo(Math.Abs(b)));

        /// <summary>
        /// Compares two <see cref="int"/> values by their signs (-1, 0, 1).
        /// </summary>
        public static IComparer<int> IntSignComparer { get; } =
            Comparer<int>.Create((a, b) => Math.Sign(a).CompareTo(Math.Sign(b)));

        /// <summary>
        /// Compares two <see cref="long"/> values by their absolute values.
        /// </summary>
        public static IComparer<long> LongAbsolute { get; } =
            Comparer<long>.Create((a, b) => Math.Abs(a).CompareTo(Math.Abs(b)));

        /// <summary>
        /// Compares two <see cref="long"/> values by their signs (-1, 0, 1).
        /// </summary>
        public static IComparer<long> LongSignComparer { get; } =
            Comparer<long>.Create((a, b) => Math.Sign(a).CompareTo(Math.Sign(b)));
    }
}
