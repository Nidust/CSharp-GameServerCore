using System;
using System.IO;

namespace Core.Server.Builder.Configure
{
    public class LogConfigure
    {
        #region Properties
        public String FilePath { get; private set; }
        public String FileName { get; private set; }
        public TimeSpan LoggingTime { get; private set; }
        public Boolean ConsoleUsed { get; private set; }
        #endregion

        #region Methods
        public LogConfigure()
        {
            SetPath(Environment.CurrentDirectory);
            SetFileName("Undefined");
            SetLoggingTime(new TimeSpan(0, 0, 0, 0, 200));

            UseConsole(true);
        }

        public void SetPath(String path)
        {
            FilePath = Path.GetDirectoryName(path);
        }

        public void SetFileName(String name)
        {
            FileName = Path.GetFileName(name);
        }

        public void SetLoggingTime(TimeSpan loggingTime)
        {
            LoggingTime = loggingTime;
        }

        public void UseConsole(Boolean used = true)
        {
            ConsoleUsed = used;
        }
        #endregion
    }
}
