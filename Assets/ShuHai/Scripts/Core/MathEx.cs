namespace ShuHai
{
    public static class MathEx
    {
        public static bool IsPowerOfTwo(int value) { return value != 0 && (value & (value - 1)) == 0; }
    }
}