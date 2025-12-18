using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructForge.Comparers
{
    /// <summary>
    /// Provides utility comparers for custom sorting scenarios, such as reversing the natural order of elements.
    /// Can be used to create reverse, inverted, or otherwise customized comparers for collections.
    /// </summary>
    public static class SfComparerUtils
    {
        /// <summary>
        /// Reverses and returns a new instance of the given comparer
        /// </summary>
        /// <param name="comparer">Given Comparer</param>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <returns>Reversed Comparer</returns>
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