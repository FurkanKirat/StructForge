using System;
using System.Collections.Generic;

namespace StructForge.Comparers
{
    public static class SfNumberEqualityComparers
    {
        public static IEqualityComparer<double> Epsilon(double epsilon = 1e-6) =>
            EqualityComparer<double>.Create(
                (a, b) => Math.Abs(a - b) <= epsilon,
                a => a.GetHashCode()
            );

        public static IEqualityComparer<float> EpsilonFloat(float epsilon = 1e-5f) =>
            EqualityComparer<float>.Create(
                (a, b) => Math.Abs(a - b) <= epsilon,
                a => a.GetHashCode()
            );

        public static IEqualityComparer<int> Sign =>
            EqualityComparer<int>.Create(
                (a, b) => Math.Sign(a) == Math.Sign(b),
                a => Math.Sign(a)
            );
    }
}