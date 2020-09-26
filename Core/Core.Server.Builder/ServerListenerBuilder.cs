using Core.Server.Session;
using System;
using System.Collections.Generic;

namespace Core.Server.Builder
{
    public class ServerListenerBuilder : IServerBuilder
    {
        internal class ServerListener
        {
            public Int32 Port;
            public SessionManager Manager;
        }

        #region Properties
        private List<ServerListener> mListeners;
        #endregion

        #region Methods
        public ServerListenerBuilder()
        {
            mListeners = new List<ServerListener>();
        }

        public void AddListener(Int32 port, SessionManager manager)
        {
            mListeners.Add(new ServerListener()
            {
                Port = port,
                Manager = manager
            });
        }

        public void Build()
        {
        }

        public void Run()
        {
            foreach (ServerListener listener in mListeners)
            {
                listener.Manager.StartListen(listener.Port);
            }
        }
        #endregion
    }
}
