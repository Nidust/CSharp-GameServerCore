using Core.Logger;
using Core.Server.Builder.Configure;
using Core.Server.Builder.Private;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Core.Server.Builder
{
    public class ServerHostBuilder : IDisposable
    {
        #region Properties
        private ManualResetEvent mTerminated;

        private LogConfigure mLogConfig;

        private List<IServerBuilder> mPreBuilder;
        private List<IServerBuilder> mBuilder;

        private IStartup mStartup;
        #endregion

        #region Methods
        public ServerHostBuilder()
        {
            mTerminated = new ManualResetEvent(false);

            mLogConfig = new LogConfigure();
            mPreBuilder = new List<IServerBuilder>();
            mBuilder = new List<IServerBuilder>();
        }

        public ServerHostBuilder ConfigureLogging(Action<LogConfigure> setConfig)
        {
            setConfig(mLogConfig);
            return this;
        }

        public ServerHostBuilder ConfigureThread(Action<ThreadConfigure> setConfig)
        {
            ThreadConfigure config = new ThreadConfigure();
            setConfig(config);

            mPreBuilder.Add(new ThreadBuilder(config));
            return this;
        }

        public ServerHostBuilder ConfigureListeners(Action<ServerListenerConfigure> setConfig)
        {
            ServerListenerConfigure config = new ServerListenerConfigure();
            setConfig(config);

            mBuilder.Add(new ServerListenerBuilder(config));
            return this;
        }

        public ServerHostBuilder ConfigureConnectors(Action<ServerConnectionConfigure> setConfig)
        {
            ServerConnectionConfigure config = new ServerConnectionConfigure();
            setConfig(config);

            mBuilder.Add(new ServerConnectionBuilder(config));
            return this;
        }

        public ServerHostBuilder UseStartup<Type>() where Type : IStartup, new()
        {
            mStartup = new Type();
            return this;
        }

        public ServerHostBuilder Build()
        {
            Logger.Logger.Initialize(mLogConfig.FilePath, mLogConfig.FileName, mLogConfig.LoggingTime, mLogConfig.ConsoleUsed);

            Info.Log($"------ Log Configure ------");
            Info.Log($"File Path: {Path.Combine(mLogConfig.FilePath)}");
            Info.Log($"File Name: {Path.Combine(mLogConfig.FileName)}");
            Info.Log($"Time (miliseconds): {mLogConfig.LoggingTime.Milliseconds}");
            Info.Log($"UseConsole: {mLogConfig.ConsoleUsed}");

            foreach (IServerBuilder builder in mPreBuilder)
            {
                builder.Build();
            }

            mStartup.PreBuild();

            foreach (IServerBuilder builder in mBuilder)
            {
                builder.Build();
            }

            mStartup.PostBuild();

            return this;
        }

        public void Run()
        {
            foreach (IServerBuilder builder in mPreBuilder)
            {
                builder.Run();
            }

            foreach (IServerBuilder builder in mBuilder)
            {
                builder.Run();
            }

            mStartup.Run();

            mTerminated.WaitOne();
        }

        public void Dispose()
        {
            foreach (IServerBuilder builder in mBuilder)
            {
                builder.Dispose();
            }

            // PreBuilder에는 기본 셋팅 작업들이 많기 때문에 맨 마지막에 불러준다
            foreach (IServerBuilder builder in mPreBuilder)
            {
                builder.Dispose();
            }

            Logger.Logger.Uninitialize();
        }
        #endregion
    }
}
