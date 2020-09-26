using Core.Server.Builder.Configure;

namespace Core.Server.Builder.Private
{
    internal class LoggingBuilder : IServerBuilder
    {
        #region Properties
        private LogConfigure mConfig;
        #endregion

        #region Methods
        public LoggingBuilder(LogConfigure config)
        {
            mConfig = config;
        }

        public void Build()
        {
            Logger.Logger.Initialize(mConfig.FilePath, mConfig.FileName, mConfig.ConsoleUsed);
        }

        public void Run()
        {
        }

        public void Dispose()
        {
            Logger.Logger.Uninitialize();
        }
        #endregion
    }
}
