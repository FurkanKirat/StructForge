using System;
using System.Collections.Generic;
using StructForge.Collections;

namespace StructForge.Extensions
{
    /// <summary>
    /// A class for shuffle operations
    /// </summary>
    public static class SfShuffleExtensions
    {
        private static readonly Random Random = new Random();
        
        /// <summary>
        /// Shuffles the given SfList
        /// </summary>
        /// <param name="list">Given List</param>
        /// <typeparam name="T">T</typeparam>
        public static void Shuffle<T>(this SfList<T> list)
        {
            Span<T> span = list.AsSpan();
            int n = span.Length;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                (span[k], span[n]) = (span[n], span[k]); // Swap
            }
        }

        /// <summary>
        /// Shuffles the given enumerable and returns the shuffled enumerable
        /// </summary>
        /// <param name="source">Given enumerable</param>
        /// <typeparam name="T">T</typeparam>
        /// <returns>Shuffled enumerable</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            SfList<T> list = new SfList<T>(source);
            list.Shuffle();
            return list;
        }
    }
}