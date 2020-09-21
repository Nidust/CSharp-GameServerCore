﻿using Core.Network.Packet;
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
            Console.WriteLine("Try Connect Account Server...");
            mSocket.Connect(ipAddr, port);
        }

        protected override void OnConnect()
        {
            Console.WriteLine("Connect Account Server...");
        }

        protected override void OnDisconnect()
        {
            Console.WriteLine("Disconnect Account Server...");
        }

        protected override void OnPacket(IPacket packet)
        {
            Console.WriteLine($"Pong From Account Server");
        }

        protected override void OnSend()
        {
            Console.WriteLine($"Send Packet To Account Server");
        }
    }
}
