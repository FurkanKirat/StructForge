using System;
using System.Collections.Generic;
using StructForge.Collections;
using StructForge.Helpers;

namespace StructForge.Extensions
{
    public static class SfDataStructureExtensions
    {
        /// <summary>
        /// Returns a new array containing the elements of the collection.
        /// </summary>
        public static T[] ToArray<T>(this ISfDataStructure<T> sfDataStructure)
        {
            SfThrowHelper.ThrowIfNull(sfDataStructure);

            if (sfDataStructure.IsEmpty)
                return Array.Empty<T>();
            
            T[] array = new T[sfDataStructure.Count];
            sfDataStructure.CopyTo(array, 0);
            return array;
        }
        
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
            => collection == null || collection.Count == 0;
    }
}