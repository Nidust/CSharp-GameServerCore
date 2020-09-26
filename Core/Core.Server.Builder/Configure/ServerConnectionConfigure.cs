using System;
using System.Collections.Generic;

namespace Core.Server.Builder.Configure
{
    public class ServerConnection
    {
        public String Ip;
        public Int32 Port;
        public Session.Session Session;
    }

    public class ServerConnectionConfigure
    {
        #region Properteis
        private List<ServerConnection> mConnections;
        #endregion

        #region Methods
        public ServerConnectionConfigure()
        {
            mConnections = new List<ServerConnection>();
        }

        public void AddConnection(String ip, Int32 port, Session.Session session)
        {
            mConnections.Add(new ServerConnection()
            {
                Ip = ip,
                Port = port,
                Session = session
            });
        }

        public IReadOnlyList<ServerConnection> ToList()
        {
            return mConnections.AsReadOnly();
        }
        #endregion
    }
}
