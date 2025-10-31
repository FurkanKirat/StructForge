using System;
using System.Collections.Generic;

namespace StructForge.Searching
{
    public static class SfSearching
    {
        public static int BinarySearch<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            ArgumentNullException.ThrowIfNull(list);
            comparer ??= Comparer<T>.Default;
            
            int left = 0;
            int right = list.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                int cmp = comparer.Compare(list[mid], item);
                
                switch (cmp)
                {
                    case 0:
                        return mid;
                    case > 0:
                        right = mid - 1;
                        break;
                    default:
                        left = mid + 1;
                        break;
                }
            }
            
            return -1;
        }
    }
}