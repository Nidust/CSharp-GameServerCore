using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Core.Logger
{
    public static class Logger
    {
        #region Properties
        private static Thread LoggingThread;

        private static String FilePath;
        private static String FileName;

        private static Queue<Log> Logs;
        private static StreamWriter mFileWritter;

        private static Boolean mWriteConsole;
        private static Boolean mRunning;
        #endregion

        #region Methods
        static Logger()
        {
            Logs = new Queue<Log>();
            mRunning = false;
        }

        public static void Initialize(String filePath, String fileName, Boolean writeConsole = true)
        {
            mRunning = true;
            mWriteConsole = writeConsole;

            FilePath = Path.GetDirectoryName(filePath);
            FileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_{fileName}.log";

            mFileWritter = new StreamWriter(Path.Combine(FilePath, FileName), true);

            LoggingThread = new Thread(DoLogging);
            LoggingThread.Name = "Logging Thread";
            LoggingThread.Start();
        }

        public static void Uninitialize()
        {
            mRunning = false;
            LoggingThread.Join();

            Flush();

            mFileWritter.Dispose();
        }

        public static void WriteLine(ConsoleColor color, String log)
        {
            lock (Logs)
            {
                Logs.Enqueue(new Log()
                {
                    Timestamp = DateTime.Now,
                    Color = color,
                    Contents = log
                });
            }
        }
        #endregion

        #region Private
        private static void DoLogging()
        {
            while (mRunning)
            {
                Flush();

                Thread.Sleep(200);
            }
        }

        private static void Flush()
        {
            lock (Logs)
            {
                if (Logs.Count <= 0)
                    return;

                while (Logs.Count > 0)
                {
                    Log log = Logs.Dequeue();

                    String formatLog = $"{log.Timestamp} {log.Contents}";

                    if (mWriteConsole)
                    {
                        Console.ForegroundColor = log.Color;
                        Console.WriteLine(formatLog);
                    }

                    mFileWritter.WriteLine(formatLog);
                }

                mFileWritter.Flush();
            }
        }
        #endregion
    }
}
