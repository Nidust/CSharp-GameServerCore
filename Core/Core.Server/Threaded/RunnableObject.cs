using Core.Server.Job;
using System;
using System.Threading;

namespace Core.Server.Threaded
{
    public abstract class RunnableObject : IRunnable
    {
        #region Properties
        private IWorkThread mWorker;

        private static Int32 ObjectId;
        #endregion

        #region Abstarct Methods
        public abstract void OnUpdate();
        #endregion

        #region Methods
        static RunnableObject()
        {
            ObjectId = 0;
        }

        public void SetWorker(IWorkThread worker)
        {
            mWorker = worker;
        }

        public void PushJob(IJob job)
        {
            mWorker.PushJob(job);
        }

        public void PushDbJob(IDbJob job)
        {
            mWorker.PushDbJob(job);
        }

        public void PushTimerJob(TimerJob timerJob)
        {
            mWorker.PushTimerJob(timerJob);
        }

        public virtual void Dispose()
        {
            mWorker = null;
        }

        public Int32 GetId()
        {
            return Interlocked.Increment(ref ObjectId);
        }
        #endregion
    }
}
