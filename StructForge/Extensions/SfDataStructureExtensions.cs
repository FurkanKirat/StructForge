using System;
using StructForge.Collections;

namespace StructForge.Extensions
{
    public static class SfDataStructureExtensions
    {
        /// <summary>
        /// Returns a new array containing the elements of the collection.
        /// </summary>
        public static T[] ToArray<T>(this ISfDataStructure<T> sfDataStructure)
        {
            ArgumentNullException.ThrowIfNull(sfDataStructure);

            if (sfDataStructure.IsEmpty)
                return Array.Empty<T>();
            
            T[] array = new T[sfDataStructure.Count];
            sfDataStructure.CopyTo(array, 0);
            return array;
        }
    }
}