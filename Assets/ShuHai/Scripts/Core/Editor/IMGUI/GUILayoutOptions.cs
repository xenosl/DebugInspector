using System;
using System.Linq;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public static class GUILayoutOptions
    {
        /// <summary>
        ///     Number of type combinations.
        /// </summary>
        public static readonly int Count = Mathf.RoundToInt(Mathf.Pow(2, GUILayoutOptionTypeEnum.BitValueCount));

        /// <summary>
        ///     Get cached an array of <see cref="GUILayoutOption" /> that represents specified option types.
        /// </summary>
        /// <remarks>
        ///     Each time you get the array from certain set of option types is same instance.
        ///     Parameters with different order but same contents result in same array instance.
        /// </remarks>
        public static Instance Get(GUILayoutOptionType types)
        {
            var instance = instances[(int)types];
            if (instance == null)
            {
                instance = new Instance(types);
                instances[(int)types] = instance;
            }
            return instance;
        }

        private static readonly Instance[] instances = new Instance[Count];

        public sealed class Instance
        {
            public static implicit operator GUILayoutOption[](Instance instance) { return instance.rawOptions; }

            public readonly GUILayoutOptionType Types;

            public Instance(GUILayoutOptionType types)
            {
                Types = types;
                var typeValues = GUILayoutOptionTypeEnum.ValuesOf(Types);
                rawOptionIndices = new int[typeValues.Select(GUILayoutOptionTypeEnum.IndexOfBit).Max() + 1];
                rawOptions = typeValues.Select(CreateRawOption).ToArray();
            }

            #region Set Values

            public Instance SetWidth(float value) { return SetValue(GUILayoutOptionType.FixedWidth, value); }
            public Instance SetHeight(float value) { return SetValue(GUILayoutOptionType.FixedHeight, value); }

            public Instance SetMinWidth(float value) { return SetValue(GUILayoutOptionType.MinWidth, value); }
            public Instance SetMaxWidth(float value) { return SetValue(GUILayoutOptionType.MaxWidth, value); }

            public Instance SetMinHeight(float value) { return SetValue(GUILayoutOptionType.MinHeight, value); }
            public Instance SetMaxHeight(float value) { return SetValue(GUILayoutOptionType.MaxHeight, value); }

            public Instance SetStretchWidth(bool value)
            {
                return SetValue(GUILayoutOptionType.StretchWidth, value ? 1 : 0);
            }

            public Instance SetStretchHeight(bool value)
            {
                return SetValue(GUILayoutOptionType.StretchHeight, value ? 1 : 0);
            }

            public Instance SetValue(GUILayoutOptionType type, float value)
            {
                GetRawOption(type).SetValue(value);
                return this;
            }

            #endregion Set Values

            #region Raw Options

            #region Indices

            /// <summary>
            ///     Mapping from <see cref="GUILayoutOptionType" /> index to <see cref="rawOptions" /> index;
            /// </summary>
            private readonly int[] rawOptionIndices;

            private int GetRawOptionIndex(GUILayoutOptionType type)
            {
                int typeIndex = GUILayoutOptionTypeEnum.IndexOfBit(type);
                int rawIndex = rawOptionIndices[typeIndex];
                if (rawIndex == Index.Invalid)
                    throw new ArgumentException("The current instance do not contains specified option type", "type");
                return rawIndex;
            }

            #endregion Indices

            private readonly GUILayoutOption[] rawOptions;

            private GUILayoutOption GetRawOption(GUILayoutOptionType type)
            {
                return rawOptions[GetRawOptionIndex(type)];
            }

            private GUILayoutOption CreateRawOption(GUILayoutOptionType type, int index)
            {
                var option = CreateRawOption(type);
                rawOptionIndices[GUILayoutOptionTypeEnum.IndexOfBit(type)] = index;
                return option;
            }

            private static GUILayoutOption CreateRawOption(GUILayoutOptionType type, float value = default(float))
            {
                switch (type)
                {
                    case GUILayoutOptionType.FixedWidth:
                        return GUILayout.Width(value);
                    case GUILayoutOptionType.FixedHeight:
                        return GUILayout.Height(value);
                    case GUILayoutOptionType.MinWidth:
                        return GUILayout.MinWidth(value);
                    case GUILayoutOptionType.MaxWidth:
                        return GUILayout.MaxWidth(value);
                    case GUILayoutOptionType.MinHeight:
                        return GUILayout.MinHeight(value);
                    case GUILayoutOptionType.MaxHeight:
                        return GUILayout.MaxHeight(value);
                    case GUILayoutOptionType.StretchWidth:
                        return GUILayout.ExpandWidth(value > 0);
                    case GUILayoutOptionType.StretchHeight:
                        return GUILayout.ExpandHeight(value > 0);
                    default:
                        throw new ArgumentOutOfRangeException("type", type, null);
                }
            }

            #endregion Raw Options
        }
    }
}