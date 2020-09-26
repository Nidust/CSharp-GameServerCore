using System;
using System.Threading;

namespace Core.Server.Builder
{
    public class ServerHostBuilder : IServerBuilder
    {
        #region Properties
        private ManualResetEvent mTerminated;

        private LoggingBuilder mLoggingBuilder;
        private ThreadBuilder mThreadBuilder;
        private ServerListenerBuilder mListenerBuilder;
        private ServerConnectionBuilder mConnectionBuilder;
        #endregion

        #region Methods
        public ServerHostBuilder()
        {
            mTerminated = new ManualResetEvent(false);

            mLoggingBuilder = new LoggingBuilder();
            mThreadBuilder = new ThreadBuilder();
            mListenerBuilder = new ServerListenerBuilder();
            mConnectionBuilder = new ServerConnectionBuilder();
        }

        public ServerHostBuilder ConfigureLogging(Action<LoggingBuilder> build)
        {
            build.Invoke(mLoggingBuilder);
            return this;
        }

        public ServerHostBuilder ConfigureThread(Action<ThreadBuilder> build)
        {
            build.Invoke(mThreadBuilder);
            return this;
        }

        public ServerHostBuilder ConfigureListeners(Action<ServerListenerBuilder> build)
        {
            build.Invoke(mListenerBuilder);
            return this;
        }

        public ServerHostBuilder ConfigureConnectors(Action<ServerConnectionBuilder> build)
        {
            build.Invoke(mConnectionBuilder);
            return this;
        }

        public void Build()
        {
            mLoggingBuilder.Build();
            mThreadBuilder.Build();

            mListenerBuilder.Build();
            mConnectionBuilder.Build();
        }

        public void Run()
        {
            mLoggingBuilder.Run();
            mThreadBuilder.Run();

            mListenerBuilder.Run();
            mConnectionBuilder.Run();

            mTerminated.WaitOne();

            mLoggingBuilder.Dispose();
            mThreadBuilder.Dispose();
        }
        #endregion
    }
}
