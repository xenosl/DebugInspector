namespace ShuHai.Coroutines
{
    public interface IYieldAdapter
    {
        IYield ToYield(object yieldObject);
    }
}