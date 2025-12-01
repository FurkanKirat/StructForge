using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace StructForge.Helpers
{
    internal static class SfThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowArgumentNull(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowArgumentOutOfRange(string paramName, string message = null)
        {
            throw new ArgumentOutOfRangeException(paramName, message);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowArgument(string message, string paramName = null)
        {
            throw new ArgumentException(message, paramName);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowInvalidOperation(string message)
        {
            throw new InvalidOperationException(message);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowKeyNotFound(string message = null)
        {
            throw new KeyNotFoundException(message);
        }
    }
}