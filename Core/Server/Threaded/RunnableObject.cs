using Core.Server.Job;

namespace Core.Server.Threaded
{
    public abstract class RunnableObject : IRunnable
    {
        #region Properties
        private WorkerThread mWorker;
        #endregion

        #region Abstarct Methods
        public abstract void OnUpdate();
        #endregion

        #region Methods
        public void SetWorker(WorkerThread worker)
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
        #endregion
    }
}
