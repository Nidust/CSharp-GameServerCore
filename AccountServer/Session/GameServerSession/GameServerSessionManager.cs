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
            Console.WriteLine($"Create Game Server Session Manager...");
        }

        protected override NetworkSession CreateSession(ISessionManager manager, ClientSocket connection)
        {
            return new GameServerSession(manager, connection);
        }
    }
}
