using Core.Server.Builder.Configure;

namespace Core.Server.Builder.Private
{
    internal class ServerListenerBuilder : IServerBuilder
    {
        #region Properties
        private ServerListenerConfigure mConfig;
        #endregion

        #region Methods
        public ServerListenerBuilder(ServerListenerConfigure config)
        {
            mConfig = config;
        }

        public void Build()
        {
        }

        public void Run()
        {
            foreach (ServerListener listener in mConfig.ToList())
            {
                listener.Manager.StartListen(listener.Port);
            }
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
