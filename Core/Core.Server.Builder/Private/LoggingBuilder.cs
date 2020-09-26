using Core.Logger;
using Core.Server.Builder.Configure;
using System.IO;

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
            Logger.Logger.Initialize(mConfig.FilePath, mConfig.FileName, mConfig.LoggingTime, mConfig.ConsoleUsed);

            Info.Log($"------ Logging Configure ------");
            Info.Log($"Path: {Path.Combine(mConfig.FilePath, mConfig.FileName)}");
            Info.Log($"Time (miliseconds): {mConfig.LoggingTime.Milliseconds}");
            Info.Log($"UseConsole: {mConfig.ConsoleUsed}");
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
