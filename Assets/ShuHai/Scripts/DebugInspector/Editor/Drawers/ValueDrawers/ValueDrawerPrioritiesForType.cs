using System;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.DebugInspector.Editor
{
    using PriorityPair = KeyValuePair<Type, int>;

    /// <summary>
    ///     Provides management for priority settings of <see cref="ValueDrawer" />s when drawing certain type.
    /// </summary>
    public sealed class ValueDrawerPrioritiesForType
    {
        /// <summary>
        ///     Default priority value, the value is used if priority of drawer is not specified.
        /// </summary>
        public const int DefaultPriority = 0;

        /// <summary>
        ///     Type of value the current instance to apply.
        /// </summary>
        public readonly Type TypeToApply;

        /// <summary>
        ///     Find type of <see cref="ValueDrawer" /> that has the highest priority to draw value typed
        ///     <see cref="TypeToApply" />.
        /// </summary>
        /// <returns> The type of <see cref="ValueDrawer" /> that draws value typed <see cref="TypeToApply" />. </returns>
        public Type FindDrawerTypeOfTopPriority()
        {
            return typeToPriority.OrderByDescending(p => p.Value).FirstOrDefault().Key;
        }

        public void SetPriority(Type drawerType, int value)
        {
            Ensure.Argument.NotNull(drawerType, "drawerType");
            typeToPriority[drawerType] = value;
        }

        public int GetPriority(Type drawerType)
        {
            Ensure.Argument.NotNull(drawerType, "drawerType");

            int priority;
            if (!typeToPriority.TryGetValue(drawerType, out priority))
                priority = DefaultPriority;
            return priority;
        }

        /// <summary>
        ///     Type of drawers and its corresponding typeToPriority.
        /// </summary>
        private readonly Dictionary<Type, int> typeToPriority = new Dictionary<Type, int>();

        private ValueDrawerPrioritiesForType(Type typeToApply)
        {
            TypeToApply = typeToApply;
            instances.Add(typeToApply, this);
        }

        #region Management

        /// <summary>
        ///     Get existing priority settings that apply to specified type of value.
        /// </summary>
        /// <param name="typeToApply"> Type of value the priority settings is applied to. </param>
        /// <returns>
        ///     An instance of <see cref="ValueDrawerPrioritiesForType" /> that contains the priority settings for values
        ///     typed <paramref name="typeToApply" />.
        /// </returns>
        public static ValueDrawerPrioritiesForType Get(Type typeToApply)
        {
            var t = typeToApply;
            while (t != null)
            {
                ValueDrawerPrioritiesForType inst;
                if (instances.TryGetValue(t, out inst))
                    return inst;
                t = t.BaseType;
            }
            return null;
        }

        public static ValueDrawerPrioritiesForType GetOrCreate(Type typeToApply)
        {
            return Get(typeToApply) ?? new ValueDrawerPrioritiesForType(typeToApply);
        }

        /// <summary>
        ///     Remove priority settings for specified type.
        /// </summary>
        /// <param name="typeToApply"> Type of value which the priority settings is applied to remove. </param>
        public static void Remove(Type typeToApply) { instances.Remove(typeToApply); }

        private static readonly Dictionary<Type, ValueDrawerPrioritiesForType> instances =
            new Dictionary<Type, ValueDrawerPrioritiesForType>();

        private static void InitDefaultPriorities()
        {
            var forString = new ValueDrawerPrioritiesForType(typeof(string));
            forString.SetPriority(typeof(StringDrawer<>), 1);
        }

        static ValueDrawerPrioritiesForType() { InitDefaultPriorities(); }

        #endregion
    }
}