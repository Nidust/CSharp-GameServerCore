using System;
using System.IO;

namespace Core.Server.Builder
{
    public class LoggingBuilder : IServerBuilder, IDisposable
    {
        #region Properties
        private String mFilePath;
        private String mFileName;
        private Boolean mUseConsole;
        #endregion

        #region Methods
        public LoggingBuilder()
        {
            mFilePath = String.Empty;
            mFileName = String.Empty;
            mUseConsole = false;
        }

        public void SetPath(String path)
        {
            mFilePath = Path.GetDirectoryName(path);
        }

        public void SetFileName(String name)
        {
            mFileName = Path.GetFileName(name);
        }

        public void UseConsole()
        {
            mUseConsole = true;
        }

        public void Build()
        {
            Logger.Logger.Initialize(mFilePath, mFileName, mUseConsole);
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
