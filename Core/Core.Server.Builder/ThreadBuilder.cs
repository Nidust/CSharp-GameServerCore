using Core.Server.Threaded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server.Builder
{
    public class ThreadBuilder : IServerBuilder, IDisposable
    {
        #region Properties
        private String mName;
        private Int32 mWorkerThreadCount;
        #endregion

        #region Methods
        public ThreadBuilder()
        {
            mName = String.Empty;
            mWorkerThreadCount = 0;
        }

        public void SetName(String name)
        {
            mName = name;
        }

        public void SetWorkerCount(Int32 count)
        {
            mWorkerThreadCount = count;
        }

        public void Build()
        {
            ThreadCoordinator.Initialize(mName, mWorkerThreadCount);
        }

        public void Run()
        {
        }

        public void Dispose()
        {
            ThreadCoordinator.Uninitialize();
        }
        #endregion
    }
}
