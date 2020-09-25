using Core.Logger;
using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Session;
using System;
using NetworkSession = Core.Server.Session.Session;

namespace AccountServer.Session.GameServerSession
{
    public class GameServerSession : NetworkSession
    {
        public GameServerSession(ISessionManager manager, NetworkSocket socket) 
            : base(manager, socket)
        {
        }

        protected override void OnConnect()
        {
            Info.Log($"Registered Game Server Session...");
        }

        protected override void OnDisconnect()
        {
            Info.Log($"UnRegistered Game Server Session...");
        }

        protected override void OnPacket(IPacket packet)
        {
            Debug.Log($"Ping Packet From Game Server Session");
        }

        protected override void OnSend()
        {
            Debug.Log($"Send Pong Packet To Game Server Session");
        }
    }
}
