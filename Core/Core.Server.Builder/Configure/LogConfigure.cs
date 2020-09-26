using System;
using System.IO;

namespace Core.Server.Builder.Configure
{
    public class LogConfigure
    {
        #region Properties
        public String FilePath { get; private set; }
        public String FileName { get; private set; }
        public Boolean ConsoleUsed { get; private set; }
        #endregion

        #region Methods
        public LogConfigure()
        {
            FilePath = String.Empty;
            FileName = String.Empty;
            ConsoleUsed = false;
        }

        public void SetPath(String path)
        {
            FilePath = Path.GetDirectoryName(path);
        }

        public void SetFileName(String name)
        {
            FileName = Path.GetFileName(name);
        }

        public void UseConsole()
        {
            ConsoleUsed = true;
        }
        #endregion
    }
}
