using Core.Logger;
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
            Info.Log($"------ Server Listener Configure ------");

            foreach (ServerListener listener in mConfig.ToList())
            {
                Info.Log($"Listener - Port:{listener.Port}, Acceptor:{listener.Manager.GetType().Name}");

                listener.Manager.StartListen(listener.Port);
            }
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
