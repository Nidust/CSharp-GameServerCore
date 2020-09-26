using Core.Server.Session;
using System;
using System.Collections.Generic;

namespace Core.Server.Builder.Configure
{
    public class ServerListener
    {
        public Int32 Port;
        public SessionManager Manager;
    }

    public class ServerListenerConfigure
    {
        #region Properties
        private List<ServerListener> mListeners;
        #endregion

        #region Methods
        public ServerListenerConfigure()
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

        public IReadOnlyList<ServerListener> ToList()
        {
            return mListeners.AsReadOnly();
        }
        #endregion
    }
}
