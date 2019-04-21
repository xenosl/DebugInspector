namespace ShuHai
{
    public static class Index
    {
        public const int Invalid = -1;

        /// <summary>
        ///     Check if <paramref name="index" /> is a valid index.
        /// </summary>
        /// <param name="index"> Value to check. </param>
        /// <param name="length"> Range for checking. </param>
        /// <returns> True if <paramref name="index" /> is valid as index; otherwise, false. </returns>
        public static bool IsValid(int index, int length) { return index >= 0 && index < length; }

        /// <returns>
        ///     The restricted index between 0 and <paramref name="length" /> - 1 if <paramref name="length" /> is not less than 0;
        ///     otherwise <see cref="Invalid" /> if <paramref name="length" /> is lees than 0.
        /// </returns>
        public static int Clamp(int index, int length) { return length <= 0 ? Invalid : index.Clamp(0, length - 1); }

        /// <summary>
        ///     Get an index value that keeps <paramref name="index" /> in a valid range for loops.
        /// </summary>
        /// <param name="index"> Value to loop. </param>
        /// <param name="length"> Range for looping. </param>
        /// <returns> A looped index value of <paramref name="index" />. </returns>
        /// <remarks>
        ///     There are situations that you want to keep index looping in a certain range while the index value is out of range.
        ///     For example, when getting an edge vector of a polygon, you may write code like this:
        ///     <code>
        /// public static Vector2 GetEdgeVector(IList&gt;Vector2&lt; points, int index)
        /// {
        ///     int count = points.Count;
        ///     int i0 = Index.Loop(index, count), i1 = Index.Loop(index + 1, count);
        ///     return points[i1] - points[i0];
        /// }
        /// </code>
        ///     Thus you don't have to worry about <see cref="System.IndexOutOfRangeException" />.
        /// </remarks>
        public static int Loop(int index, int length)
        {
            if (length == 0)
                return 0;
            if (index < 0)
                index += (-index / length + 1) * length;
            index = index % length;
            return index;
        }
    }
}