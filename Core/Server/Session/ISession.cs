using Core.Network.Packet;
using System;

namespace Core.Server.Session
{
    public interface ISession : IDisposable
    {
        void Send(IPacket packet);
    }
}
