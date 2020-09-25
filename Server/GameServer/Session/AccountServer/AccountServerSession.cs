using Core.Logger;
using Core.Network.Packet;
using Core.Network.Socket;
using System;
using NetworkSession = Core.Server.Session.Session;

namespace GameServer.Session.AccountServer
{
    public sealed class AccountServerSession : NetworkSession
    {
        public AccountServerSession() 
            : base(65535, 65535, NetworkConnectionType.TryReconnect)
        {
        }

        public void Connect(String ipAddr, Int32 port)
        {
            Info.Log("Try Connect Account Server...");
            mSocket.Connect(ipAddr, port);
        }

        protected override void OnConnect()
        {
            Info.Log("Connect Account Server...");
        }

        protected override void OnDisconnect()
        {
            Info.Log("Disconnect Account Server...");
        }

        protected override void OnPacket(IPacket packet)
        {
            Debug.Log($"Pong From Account Server");
        }

        protected override void OnSend()
        {
            Debug.Log($"Send Packet To Account Server");
        }
    }
}
