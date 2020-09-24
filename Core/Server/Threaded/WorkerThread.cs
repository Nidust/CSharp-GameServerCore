using Core.Server.Job;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Server.Threaded
{
    public class WorkerThread
    {
        #region Properties
        private Thread mThread;
        private Int32 mThreadId;

        private List<IRunnable> mRunnables;
        private JobExecuter mJobExecutor;

        private Boolean mRunning;
        #endregion

        #region Methods
        public WorkerThread(int threadId)
        {
            mThreadId = threadId;
            mRunnables = new List<IRunnable>();
            mJobExecutor = new JobExecuter();

            mRunning = false;
        }

        public void Start(String threadName = "")
        {
            mThread = new Thread(DoWork);
            mThread.Name = threadName;

            mRunning = true;
            mThread.Start();
        }

        public void Stop()
        {
            mRunning = false;
            mThread.Join();
            mRunnables.Clear();
        }

        public void PushJob(IJob job)
        {
            mJobExecutor.PushJob(job);
        }

        public void PushDbJob(IDbJob job)
        {
            mJobExecutor.PushDbJob(job);
        }

        public void PushTimerJob(TimerJob timerJob)
        {
            mJobExecutor.PushTimerJob(timerJob);
        }

        public void AddRunnable(IRunnable runnable)
        {
            lock (mRunnables)
            {
                runnable.SetWorker(this);
                mRunnables.Add(runnable);
            }
        }

        public void RemoveRunnable(IRunnable runnable)
        {
            lock (mRunnables)
            {
                runnable.Dispose();
                mRunnables.Remove(runnable);
            }
        }
        #endregion

        #region Private
        private void DoWork()
        {
            while (mRunning)
            {
                OnUpdate();

                mJobExecutor.DoDbJob();

                mJobExecutor.Do();
                mJobExecutor.DoTimer();

                mJobExecutor.DoDbJob();
            }
        }

        private void OnUpdate()
        {
            lock (mRunnables)
            {
                foreach (IRunnable runnable in mRunnables)
                {
                    runnable.OnUpdate();
                }
            }
        }
        #endregion
    }
}
