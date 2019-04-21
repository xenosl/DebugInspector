namespace ShuHai
{
    public struct Interval
    {
        public int Min;
        public int Max;

        public Interval(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        ///     Test specific value if it is inside the closed interval of current instance.
        /// </summary>
        /// <param name="value">The value to test.</param>
        public bool ClosedContains(int value) { return value >= Min && value <= Max; }

        /// <summary>
        ///     Test specific value if it is inside the open interval of current instance.
        /// </summary>
        /// <param name="value">The value to test.</param>
        public bool OpenContains(int value) { return value > Min && value < Max; }
    }
}