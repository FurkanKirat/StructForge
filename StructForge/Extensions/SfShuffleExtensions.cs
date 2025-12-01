using System;
using System.Collections.Generic;
using StructForge.Collections;

namespace StructForge.Extensions
{
    public static class SfShuffleExtensions
    {
        private static readonly Random Random = new Random();
        
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

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            SfList<T> list = new SfList<T>(source);
            list.Shuffle();
            return list;
        }
    }
}