using Core.Server.Job;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Server.Threaded
{
    public static class ThreadCoordinator
    {
        #region Properties
        public static Int32 MaxFramePerSecond { get; set; }

        private static List<WorkerThread> mWorkerThreads;
        #endregion

        #region Methods
        static ThreadCoordinator()
        {
            mWorkerThreads = new List<WorkerThread>();
        }

        public static void Initialize(String threadName, int maxWorkerThreads)
        {
            lock (mWorkerThreads)
            {
                for (int threadId = 0; threadId < maxWorkerThreads; ++threadId)
                {
                    WorkerThread worker = new WorkerThread(MaxFramePerSecond);
                    mWorkerThreads.Add(worker);

                    worker.Start(threadName);
                }
            }
        }

        public static void Uninitialize()
        {
            lock (mWorkerThreads)
            {
                foreach (WorkerThread worker in mWorkerThreads)
                {
                    worker.Stop();
                }

                mWorkerThreads.Clear();
            }
        }

        public static void AddRunnable(IRunnable runnable)
        {
            if (mWorkerThreads.Count == 0)
                return;

            mWorkerThreads[runnable.GetId() % mWorkerThreads.Count].AddRunnable(runnable);
        }

        public static void RemoveRunnable(IRunnable runnable)
        {
            if (mWorkerThreads.Count == 0)
                return;

            mWorkerThreads[runnable.GetId() % mWorkerThreads.Count].RemoveRunnable(runnable);
        }

        public static void PushJob(IRunnable runnable, IJob job)
        {
            if (mWorkerThreads.Count == 0)
                return;

            mWorkerThreads[runnable.GetId() % mWorkerThreads.Count].PushJob(job);
        }

        public static void PushDbJob(IRunnable runnable, IDbJob job)
        {
            if (mWorkerThreads.Count == 0)
                return;

            mWorkerThreads[runnable.GetId() % mWorkerThreads.Count].PushDbJob(job);
        }

        public static void PushTimerJob(IRunnable runnable, TimerJob job)
        {
            if (mWorkerThreads.Count == 0)
                return;

            mWorkerThreads[runnable.GetId() % mWorkerThreads.Count].PushTimerJob(job);
        }
        #endregion
    }
}
