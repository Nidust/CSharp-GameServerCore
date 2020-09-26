using Core.Logger;
using Core.Server.Builder.Configure;
using System;
using System.Net;
using System.Net.Sockets;

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
            Info.Log($"------ Building Server Listener ------");
            Info.Log($"Listen IP: {GetLocalIP()}");

            foreach (ServerListener listener in mConfig.ToList())
            {
                Info.Log($"Port:{listener.Port}, Listener:{listener.Manager.GetType().Name}");

                listener.Manager.StartListen(listener.Port);
            }
        }

        public void Dispose()
        {
        }
        #endregion

        #region Private
        private String GetLocalIP()
        {
            String localIP = "Not available, please check your network seetings!";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            return localIP;
        }
        #endregion
    }
}
