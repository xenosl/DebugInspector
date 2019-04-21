using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    [Flags]
    public enum GUILayoutOptionType
    {
        FixedWidth = 0x1,
        FixedHeight = 0x2,
        MinWidth = 0x4,
        MaxWidth = 0x8,
        MinHeight = 0x10,
        MaxHeight = 0x20,
        StretchWidth = 0x40,
        StretchHeight = 0x80,

        FixedSize = FixedWidth | FixedHeight
    }

    public static class GUILayoutOptionTypeEnum
    {
        public static readonly int ValueCount;
        public static IEnumerable<GUILayoutOptionType> Values { get { return values; } }

        public static readonly int BitValueCount;
        public static IEnumerable<GUILayoutOptionType> BitValues { get { return bitValues; } }

        public static readonly int MixedValueCount;

        public static GUILayoutOptionType GetValue(int index) { return values[index]; }

        public static IEnumerable<GUILayoutOptionType> ValuesOf(GUILayoutOptionType types, bool includeMixed = false)
        {
            for (int i = 0; i < ValueCount; ++i)
            {
                var value = values[i];
                if ((types & value) != value)
                    continue;

                if (includeMixed)
                    yield return value;
                else if (!IsMixed(value))
                    yield return value;
            }
        }

        public static bool IsMixed(GUILayoutOptionType type) { return !MathEx.IsPowerOfTwo((int)type); }

        public static int CountOf(GUILayoutOptionType type, bool includeMixed = false)
        {
            return ValuesOf(type, includeMixed).Count();
        }

        public static int IndexOfBit(GUILayoutOptionType type)
        {
            var typeValue = (int)type;
            if (typeValue == 0 || IsMixed(type))
                throw new ArgumentException("Bit value of type expected", "type");
            return Mathf.RoundToInt(Mathf.Log(typeValue, 2));
        }

        private static readonly GUILayoutOptionType[] values;
        private static readonly GUILayoutOptionType[] bitValues;

        static GUILayoutOptionTypeEnum()
        {
            values = (GUILayoutOptionType[])Enum.GetValues(typeof(GUILayoutOptionType));
            ValueCount = values.Length;

            bitValues = values.Where(t => !IsMixed(t)).ToArray();
            BitValueCount = bitValues.Length;

            MixedValueCount = ValueCount - BitValueCount;
        }
    }
}