using UnityEngine;

namespace ShuHai.Coroutines
{
    public abstract class CustomYieldInstructionYield<T> : EmptyYield
        where T : CustomYieldInstruction
    {
        public override bool IsDone { get { return !YieldObject.keepWaiting; } }

        public readonly T YieldObject;

        protected CustomYieldInstructionYield(T yieldObject) { YieldObject = yieldObject; }
    }

    // TODO: Compatible with old unity versions (which WWW not inherited CustomYieldInstruction).
    //public sealed class WaitWWW : CustomYieldInstructionYield<WWW>
    //{
    //    public WaitWWW(WWW yieldObject) : base(yieldObject) { }
    //}

    public sealed class WaitAsyncOperation : EmptyYield
    {
        public override bool IsDone { get { return AsyncOperation.isDone; } }

        public readonly AsyncOperation AsyncOperation;

        public WaitAsyncOperation(AsyncOperation asyncOperation) { AsyncOperation = asyncOperation; }
    }
}