using System;

namespace ShuHai
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     Get a value indicating whether current <see langword="enum" /> instance has any flag of specific
        ///     <see langword="enum" /> value.
        /// </summary>
        /// <param name="self"> The current <see langword="enum" /> instance. </param>
        /// <param name="flags"> The flag value to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> contains any flag in <see langword="enum" /> value
        ///     <paramref name="flags" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool HasAnyFlag(this Enum self, Enum flags)
        {
            long selfValue = Convert.ToInt64((object)self);
            long flagValue = Convert.ToInt64((object)flags);
            return (selfValue & flagValue) > 0;
        }

        /// <summary>
        ///     Get a value indicating whether current <see langword="enum" /> instance has flag of specific
        ///     <see langword="enum" /> value.
        /// </summary>
        /// <param name="self"> The current <see langword="enum" /> instance. </param>
        /// <param name="flag"> The flag value to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> contains flag in <see langword="enum" /> value
        ///     <paramref name="flag" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool HasFlag(this Enum self, Enum flag)
        {
            long selfValue = Convert.ToInt64((object)self);
            long flagValue = Convert.ToInt64((object)flag);
            return (selfValue & flagValue) == flagValue;
        }
    }
}