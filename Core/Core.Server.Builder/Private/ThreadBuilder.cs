using Core.Server.Builder.Configure;
using Core.Server.Threaded;

namespace Core.Server.Builder.Private
{
    internal class ThreadBuilder : IServerBuilder
    {
        #region Properties
        private ThreadConfigure mConfig;
        #endregion

        #region Methods
        public ThreadBuilder(ThreadConfigure config)
        {
            mConfig = config;
        }

        public void Build()
        {
            ThreadCoordinator.MaxFramePerSecond = mConfig.FramePerSecond;
            ThreadCoordinator.Initialize(mConfig.Name, mConfig.WorkerThreads);
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
