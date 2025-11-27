namespace StructForge.Comparers
{
    using System;
    using System.Collections.Generic;

    namespace StructForge.Helpers
    {
        internal static class SfEqualityComparer
        {
            public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
            {
                return new SfLambdaEqualityComparer<T>(equals, getHashCode);
            }

            private sealed class SfLambdaEqualityComparer<T> : IEqualityComparer<T>
            {
                private readonly Func<T, T, bool> _equals;
                private readonly Func<T, int> _hash;

                public SfLambdaEqualityComparer(Func<T, T, bool> equals, Func<T, int> hash)
                {
                    _equals = equals ?? throw new ArgumentNullException(nameof(equals));
                    _hash = hash ?? throw new ArgumentNullException(nameof(hash));
                }

                public bool Equals(T x, T y) => _equals(x, y);
                public int GetHashCode(T obj) => _hash(obj);
            }
        }
    }

}