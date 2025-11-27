using System;
using System.Diagnostics.CodeAnalysis;

namespace StructForge.Helpers
{
    internal static class SfThrowHelper
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        [SuppressMessage("ReSharper", "MultipleEnumeration")]
        // ReSharper disable once PossibleMultipleEnumeration
        
        public static void ThrowIfNull<T>(T value, string paramName = null)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void ThrowIfNonPositive(int value, string paramName = null)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName);
        }
    }
}