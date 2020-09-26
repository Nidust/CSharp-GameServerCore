using System;
using System.Collections.Generic;

namespace Core.Server.Builder
{
    public class ServerConnectionBuilder : IServerBuilder
    {
        internal class ServerConnection
        {
            public String Ip;
            public Int32 Port;
            public Session.Session Session;
        }

        #region Properties
        private List<ServerConnection> mConnections;
        #endregion

        #region Methods
        public void AddConnection(String ip, Int32 port, Session.Session session)
        {
            mConnections.Add(new ServerConnection()
            {
                Ip = ip,
                Port = port,
                Session = session
            });
        }

        public void Build()
        {
        }

        public void Run()
        {
            foreach (ServerConnection connection in mConnections)
            {
                String ip = connection.Ip;
                Int32 port = connection.Port;

                connection.Session.Connect(ip, port);
            }
        }
        #endregion
    }
}
