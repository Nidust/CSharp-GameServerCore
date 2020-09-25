using Core.Server.Job;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Server.Threaded
{
    public class WorkerThread : IWorker
    {
        #region Properties
        private Thread mThread;

        private List<IRunnable> mRunnables;
        private JobExecuter mJobExecutor;

        private Int32 mMaxPerFrameSecond;
        
        private Boolean mRunning;
        #endregion

        #region Methods
        public WorkerThread(Int32 fps)
        {
            mRunnables = new List<IRunnable>();
            mJobExecutor = new JobExecuter();

            mMaxPerFrameSecond = fps;
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
            FpsWatch fpsWatch = new FpsWatch();

            while (mRunning)
            {
                fpsWatch.Begin();

                OnUpdate();
                
                mJobExecutor.DoDbJob();

                mJobExecutor.DoJob();
                mJobExecutor.DoTimer();

                mJobExecutor.DoDbJob();

                fpsWatch.End();
                SleepOnFrameRate(fpsWatch);
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

        private void SleepOnFrameRate(FpsWatch fpsWatch)
        {
            Int32 sleepTimeMilliseconds = (Int32)((1.0f / Math.Max(mMaxPerFrameSecond, 1)) * 1000);
            Int32 remainTimeMilliseconds = sleepTimeMilliseconds - fpsWatch.FramePerMilliseconds;

            if (remainTimeMilliseconds <= 0)
            {
                return;
            }

            Thread.Sleep(remainTimeMilliseconds);
        }
        #endregion
    }
}
