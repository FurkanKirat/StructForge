using System.Collections.Generic;
using StructForge.Collections;
using StructForge.Comparers;

namespace StructForge.Sorting
{
    public static class SfSorting
    {
        #region QuickSort
        public static void QuickSort<T>(IList<T> sequence, IComparer<T> comparer = null)
        {
            if (sequence.Count <= 1)
                return;
            
            comparer ??= SfComparers<T>.DefaultComparer;
            QuickSort(sequence, comparer, 0, sequence.Count - 1);
        }

        private static void QuickSort<T>(IList<T> sequence, IComparer<T> comparer, int left, int right)
        {
            if (left >= right)
                return;
            int pivot = Partition(sequence, comparer, left, right);
            
            QuickSort(sequence, comparer, left, pivot - 1);
            QuickSort(sequence, comparer, pivot + 1, right);
        }

        private static int Partition<T>(IList<T> sequence, IComparer<T> comparer, int left, int right)
        {
            int mid = left + (right - left) / 2;
            int pivotIndex = MedianOfThree(sequence, left, mid, right, comparer);
            (sequence[pivotIndex], sequence[right]) = (sequence[right], sequence[pivotIndex]); //Carrying pivot to the end
            T pivot = sequence[right]; 
            int i = left - 1;
            for (int j = left; j <= right - 1; j++)
            {
                if (comparer.Compare(sequence[j], pivot) < 0)
                {
                    i++;
                    (sequence[j], sequence[i]) = (sequence[i], sequence[j]);
                }
            }
            (sequence[i + 1], sequence[right]) = (sequence[right], sequence[i + 1]);
            return i + 1;
        }
        
        private static int MedianOfThree<T>(IList<T> list, int a, int b, int c, IComparer<T> comparer)
        {
            if (comparer.Compare(list[a], list[b]) > 0) (a, b) = (b, a);
            if (comparer.Compare(list[a], list[c]) > 0) (_, c) = (c, a);
            if (comparer.Compare(list[b], list[c]) > 0) (b, _) = (c, b);
            return b;
        }
        #endregion
        
        #region TreeSort
        /// <summary>
        /// TreeSort uses a simple BST and does not support duplicate values.
        /// Not safe to use with sequences containing duplicates.
        /// </summary>
        public static void TreeSort<T>(IList<T> sequence, IComparer<T> comparer = null)
        {
            if (sequence.Count <= 1)
                return;
            
            comparer ??= SfComparers<T>.DefaultComparer;
            SfAvlTree<T> tree = new SfAvlTree<T>(sequence, comparer);
            int i = 0;
            foreach (T item in tree)
            {
                sequence[i] = item;
                i++;
            }
        }
        #endregion

        #region HeapSort
        public static void HeapSort<T>(IList<T> sequence, IComparer<T> comparer = null)
        {
            if (sequence.Count <= 1)
                return;
            
            comparer ??= SfComparers<T>.DefaultComparer;
            SfMaxHeap<T> maxHeap = new SfMaxHeap<T>(sequence, comparer);

            while (maxHeap.Count > 0)
            {
                T item = maxHeap.Pop();
                sequence[maxHeap.Count] = item;
            }
        }
        #endregion
    }
}