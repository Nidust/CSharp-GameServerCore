using System;
using System.Threading;

namespace Core.Server.Builder.Configure
{
    public class ThreadConfigure
    {
        #region Properties
        public String Name { get; private set; }
        public Int32 FramePerSecond { get; private set; }
        public Int32 WorkerThreads { get; private set; }
        #endregion

        #region Methods
        public ThreadConfigure()
        {
            SetFps(60);
            SetName("WorkerThread");
            SetMaxWorkerThreads();
        }

        public void SetName(String name)
        {
            Name = name;
        }

        public void SetFps(Int32 fps)
        {
            FramePerSecond = fps;
        }

        public void SetMaxWorkerThreads()
        {
            Int32 workerThread;
            Int32 completionPortThread;
            ThreadPool.GetMaxThreads(out workerThread, out completionPortThread);

            WorkerThreads = workerThread;
        }

        public void SetWorkerThreads(Int32 count)
        {
            WorkerThreads = count;
        }
        #endregion
    }
}
