﻿using Core.Logger;
using Core.Network.Socket;
using Core.Server.Session;
using System;
using NetworkSession = Core.Server.Session.Session;

namespace AccountServer.Session.GameServerSession
{
    public class GameServerSessionManager : SessionManager
    {
        public GameServerSessionManager() 
            : base()
        {
            Info.Log($"Create Game Server Session Manager...");
        }

        protected override NetworkSession CreateSession(ISessionManager manager, NetworkSocket connection)
        {
            return new GameServerSession(manager, connection);
        }
    }
}
