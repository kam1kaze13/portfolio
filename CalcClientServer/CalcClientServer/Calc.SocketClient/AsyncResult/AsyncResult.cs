namespace Calc.SocketClient.AsyncResult
{
    using System;

    internal class AsyncResult<TResult> : AsyncResultNoResult
    {
        // Field set when operation completes
        private TResult result;

        public AsyncResult(AsyncCallback asyncCallback, object state)
            : base(asyncCallback, state)
        {
        }

        public void SetAsCompleted(TResult r)
        {
            this.SetAsCompleted(r, false);
        }

        public void SetAsCompleted(TResult r, bool completedSynchronously)
        {
            // Save the asynchronous operation's result
            this.result = r;

            // Tell the base class that the operation completed 
            // sucessfully (no exception)
            base.SetAsCompleted(null, completedSynchronously);
        }

        public new TResult EndInvoke()
        {
            base.EndInvoke(); // Wait until operation has completed 
            return this.result;  // Return the result (if above didn't throw)
        }
    }
}
