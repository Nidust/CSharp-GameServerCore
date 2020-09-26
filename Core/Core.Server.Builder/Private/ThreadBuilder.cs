using Core.Logger;
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
            Info.Log($"------ Building Thread ------");
            Info.Log($"Name: {mConfig.Name}");
            Info.Log($"FramePerSecond: {mConfig.FramePerSecond}");
            Info.Log($"Thread Count: {mConfig.WorkerThreads}");

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
