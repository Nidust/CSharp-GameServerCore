using System;
using System.Collections.Generic;

namespace Core.Server.Threaded
{
    public class ThreadCoordinator
    {
        #region Properties
        private List<WorkerThread> mWorkerThreads;
        #endregion

        #region Methods
        public ThreadCoordinator(int maxWorkerThreads)
        {
            mWorkerThreads = new List<WorkerThread>();

            for (int threadId = 0; threadId < maxWorkerThreads; ++threadId)
            {
                mWorkerThreads.Add(new WorkerThread(threadId));
            }
        }

        public void Start(String threadName)
        {
            foreach (WorkerThread worker in mWorkerThreads)
            {
                worker.Start(threadName);
            }
        }

        public void Stop()
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

        public void AddRunnable(IRunnable runnable)
        {
            lock (mWorkerThreads)
            {
                if (mWorkerThreads.Count == 0)
                    return;

                mWorkerThreads[runnable.GetId() % mWorkerThreads.Count].AddRunnable(runnable);
            }
        }

        public void RemoveRunnable(IRunnable runnable)
        {
            lock (mWorkerThreads)
            {
                if (mWorkerThreads.Count == 0)
                    return;

                mWorkerThreads[runnable.GetId() % mWorkerThreads.Count].RemoveRunnable(runnable);
            }
        }
        #endregion
    }
}
