using ServerCore.Network;
using ServerCore.Packet;
using ServerCore.Session;
using System;
using NetworkSession = ServerCore.Session.Session;

namespace AccountServer.Session.GameServerSession
{
    public class GameServerSession : NetworkSession
    {
        public GameServerSession(ISessionManager manager, ClientSocket socket) 
            : base(manager, socket)
        {
        }

        protected override void OnConnect()
        {
            Console.WriteLine($"Registered Game Server Session...");
        }

        protected override void OnDisconnect()
        {
            Console.WriteLine($"UnRegistered Game Server Session...");
        }

        protected override void OnPacket(IPacket packet)
        {
            Console.WriteLine($"Ping Packet From Game Server Session");

            Send(new Pong());
        }

        protected override void OnSend()
        {
            Console.WriteLine($"Send Pong Packet To Game Server Session");
        }
    }
}
