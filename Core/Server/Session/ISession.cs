using Core.Network.Packet;
using Core.Server.Lock;
using System;

namespace Core.Server.Session
{
    public interface ISession : IDisposable
    {
        void Send(IPacket packet);
    }
}
