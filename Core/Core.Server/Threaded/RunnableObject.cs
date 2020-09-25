using Core.Server.Job;
using System;
using System.Threading;

namespace Core.Server.Threaded
{
    public abstract class RunnableObject : IRunnable, IWorker
    {
        #region Properties
        private Int32 mObjectId;

        private static Int32 ObjectId;
        #endregion

        #region Abstarct Methods
        public abstract void OnUpdate();
        public abstract void Dispose();
        #endregion

        #region Methods
        static RunnableObject()
        {
            ObjectId = 0;
        }

        public RunnableObject()
        {
            mObjectId = Interlocked.Increment(ref ObjectId);
        }

        public void PushJob(IJob job)
        {
            ThreadCoordinator.PushJob(this, job);
        }

        public void PushDbJob(IDbJob job)
        {
            ThreadCoordinator.PushDbJob(this, job);
        }

        public void PushTimerJob(TimerJob timerJob)
        {
            ThreadCoordinator.PushTimerJob(this, timerJob);
        }

        public Int32 GetId()
        {
            return mObjectId;
        }
        #endregion
    }
}
