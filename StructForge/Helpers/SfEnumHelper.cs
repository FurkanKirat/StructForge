using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StructForge.Helpers
{
    /// <summary>
    /// Helper class for enums
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public static class SfEnumHelper<TEnum> where TEnum : Enum
    {
        private static readonly TEnum[] Values = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .OrderBy(ToInt)
            .ToArray();
        internal static int Count => Values.Length;
    
        internal static TEnum Min => Values[0];
        internal static TEnum Max => Values[^1];
        internal static int MinInt => ToInt(Values[0]);
        internal static int MaxInt => ToInt(Values[^1]);
        internal static IReadOnlyList<TEnum> AllValues => Values;
        internal static int Range => MaxInt - MinInt + 1;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ToInt(TEnum value)
        {
            return Unsafe.SizeOf<TEnum>() switch
            {
                sizeof(int) => Unsafe.As<TEnum, int>(ref value),
                sizeof(byte) => Unsafe.As<TEnum, byte>(ref value),
                sizeof(ushort) => Unsafe.As<TEnum, ushort>(ref value),
                sizeof(long) => (int)Unsafe.As<TEnum, long>(ref value),
                _ => throw new NotSupportedException($"Enum size {Unsafe.SizeOf<TEnum>()} not supported")
            };
        }
    }
}

