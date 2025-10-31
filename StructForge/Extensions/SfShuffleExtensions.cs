using System;
using System.Collections.Generic;
using StructForge.Collections;

namespace StructForge.Extensions
{
    public static class SfShuffleExtensions
    {
        private static readonly Random Random = new Random();
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int rand = Random.Next(i + 1);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            SfList<T> list = source.ToSfList();
            list.Shuffle();
            return list;
        }
    }
}