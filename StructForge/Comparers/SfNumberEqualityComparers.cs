using System;
using System.Collections.Generic;
using StructForge.Comparers.StructForge.Helpers;

namespace StructForge.Comparers
{
    public static class SfNumberEqualityComparers
    {
        public static IEqualityComparer<double> Epsilon(double epsilon = 1e-6) =>
            SfEqualityComparer.Create<double>(
                (a, b) => Math.Abs(a - b) <= epsilon,
                a => a.GetHashCode()
            );

        public static IEqualityComparer<float> EpsilonFloat(float epsilon = 1e-5f) =>
            SfEqualityComparer.Create<float>(
                (a, b) => Math.Abs(a - b) <= epsilon,
                a => a.GetHashCode()
            );

        public static IEqualityComparer<int> Sign =>
            SfEqualityComparer.Create<int>(
                (a, b) => Math.Sign(a) == Math.Sign(b),
                a => Math.Sign(a)
            );
    }
}