using Core.Logger;
using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Session;
using System;
using System.Threading;

namespace Core.Test.DummyServer
{
    public class ClientSession : Session
    {
        private Int32 mIndex;
        private static Int32 Index = 0;

        public ClientSession(ISessionManager manager, NetworkSocket socket) 
            : base(manager, socket)
        {
            mIndex = Interlocked.Increment(ref Index);
        }

        protected override void OnConnect()
        {
            Debug.Log($"Connect Client Index:{mIndex} Session Count:{mManager.GetSessionCount()}");
        }

        protected override void OnDisconnect()
        {
            Debug.Log($"Disconnect Client Index:{mIndex} Session Count:{mManager.GetSessionCount() - 1}");
        }

        protected override void OnPacket(IPacket packet)
        {
            throw new NotImplementedException();
        }

        protected override void OnSend()
        {
            throw new NotImplementedException();
        }
    }
}
