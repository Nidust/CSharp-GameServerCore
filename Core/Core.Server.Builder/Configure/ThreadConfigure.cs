using System;

namespace Core.Server.Builder.Configure
{
    public class ThreadConfigure
    {
        #region Properties
        public String Name { get; private set; }
        public Int32 WorkerThreadCount { get; private set; }
        #endregion

        #region Methods
        public ThreadConfigure()
        {
            Name = string.Empty;
            WorkerThreadCount = 0;
        }

        public void SetName(String name)
        {
            Name = name;
        }

        public void SetWorkerCount(Int32 count)
        {
            WorkerThreadCount = count;
        }
        #endregion
    }
}
