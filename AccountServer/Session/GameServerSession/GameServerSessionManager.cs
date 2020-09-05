using ServerCore.Network;
using ServerCore.Session;
using System;
using NetworkSession = ServerCore.Session.Session;

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
