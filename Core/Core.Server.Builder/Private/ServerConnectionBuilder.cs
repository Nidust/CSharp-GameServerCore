using Core.Server.Builder.Configure;
using System;

namespace Core.Server.Builder.Private
{
    internal class ServerConnectionBuilder : IServerBuilder
    {
        #region Properties
        private ServerConnectionConfigure mConfig;
        #endregion

        #region Methods
        public ServerConnectionBuilder(ServerConnectionConfigure config)
        {
            mConfig = config;
        }

        public void Build()
        {
        }

        public void Run()
        {
            foreach (ServerConnection connection in mConfig.ToList())
            {
                String ip = connection.Ip;
                Int32 port = connection.Port;

                connection.Session.Connect(ip, port);
            }
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
