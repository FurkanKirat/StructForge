using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    public static class SfComparerUtils
    {
        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            comparer ??= SfComparers<T>.DefaultComparer;
            
            if (comparer is SfReverseComparerWrapper<T> wrapper)
            {
                return wrapper.Original;
            }
            
            return new SfReverseComparerWrapper<T>(comparer);
        }
        
        internal sealed class SfReverseComparerWrapper<T> : IComparer<T>
        {
            internal readonly IComparer<T> Original;

            public SfReverseComparerWrapper(IComparer<T> original)
            {
                Original = original;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
            {
                return Original.Compare(y, x);
            }
        }
    }
}