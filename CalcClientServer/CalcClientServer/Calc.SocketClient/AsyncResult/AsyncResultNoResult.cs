namespace Calc.SocketClient.AsyncResult
{
    using System;
    using System.Threading;

    internal class AsyncResultNoResult : IAsyncResult
    {
        // Fields set at construction which never change while 
        // operation is pending
        private readonly AsyncCallback asyncCallback;
        private readonly object asyncState;

        // Fields set at construction which do change after 
        // operation completes
        private enum OperationState
        {
            Pending,
            CompletedSynchronously,
            CompletedAsynchronously,
        }

        private int operationState = (int)OperationState.Pending;

        // Field that may or may not get set depending on usage
        private ManualResetEvent asyncWaitHandle;

        // Fields set when operation completes
        private Exception exception;

        public AsyncResultNoResult(AsyncCallback asyncCallback, object state)
        {
            this.asyncCallback = asyncCallback;
            this.asyncState = state;
        }

        public void SetAsCompleted(Exception e, bool completedSynchronously)
        {
            // Passing null for exception means no error occurred. 
            // This is the common case
            this.exception = e;

            // The operationState field MUST be set prior calling the callback
            int prevState = Interlocked.Exchange(
                ref this.operationState,
                completedSynchronously
                    ? (int)OperationState.CompletedSynchronously
                    : (int)OperationState.CompletedAsynchronously);

            if (prevState != (int)OperationState.Pending)
                throw new InvalidOperationException("You can set a result only once");

            // If the event exists, set it
            if (this.asyncWaitHandle != null)
                this.asyncWaitHandle.Set();

            // If a callback method was set, call it
            if (this.asyncCallback != null)
                this.asyncCallback(this);
        }

        public void SetAsCompleted()
        {
            this.SetAsCompleted(null, false);
        }

        public void SetAsCompleted(Exception e)
        {
            this.SetAsCompleted(e, false);
        }


        public void EndInvoke()
        {
            // This method assumes that only 1 thread calls EndInvoke 
            // for this object
            if (!this.IsCompleted)
            {
                // If the operation isn't done, wait for it
                this.AsyncWaitHandle.WaitOne();
                this.AsyncWaitHandle.Close();
                this.asyncWaitHandle = null;  // Allow early GC
            }

            // Operation is done: if an exception occured, throw it
            if (this.exception != null)
                throw this.exception;
        }

        public void ExecuteInContext(Action action, bool completedSynchronously)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                this.SetAsCompleted(e);
            }
        }

        public void ExecuteInContext(Action action)
        {
            this.ExecuteInContext(action, false);
        }

        #region Implementation of IAsyncResult

        public object AsyncState
        {
            get { return this.asyncState; }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return Thread.VolatileRead(ref this.operationState) ==
                    (int)OperationState.CompletedSynchronously;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (this.asyncWaitHandle == null)
                {
                    bool done = this.IsCompleted;
                    var mre = new ManualResetEvent(done);
                    if (Interlocked.CompareExchange(ref this.asyncWaitHandle, mre, null) != null)
                    {
                        // Another thread created this object's event; dispose 
                        // the event we just created
                        mre.Close();
                    }
                    else
                    {
                        if (!done && this.IsCompleted)
                        {
                            // If the operation wasn't done when we created 
                            // the event but now it is done, set the event
                            this.asyncWaitHandle.Set();
                        }
                    }
                }
                return this.asyncWaitHandle;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Thread.VolatileRead(ref this.operationState) != (int)OperationState.Pending;
            }
        }

        #endregion
    }
}
