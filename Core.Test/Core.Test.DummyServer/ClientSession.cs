using Core.Logger;
using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Session;
using System;

namespace Core.Test.DummyServer
{
    public class ClientSession : Session
    {
        public ClientSession(ISessionManager manager, NetworkSocket socket) 
            : base(manager, socket)
        {
        }

        protected override void OnConnect()
        {
            Debug.Log($"Connect Client Index:{GetId()}");
        }

        protected override void OnDisconnect()
        {
            Debug.Log($"Disconnect Client Index:{GetId()}");
        }

        protected override void OnPacket(IPacket packet)
        {
            throw new NotImplementedException();
        }

        protected override void OnSend()
        {
            throw new NotImplementedException();
        }

        public override void OnUpdate()
        {
            
        }
    }
}
