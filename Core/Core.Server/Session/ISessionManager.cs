using Core.Network.Packet;
using System;

namespace Core.Server.Session
{
    public interface ISessionManager
    {
        void Broadcast(IPacket packet);
        void DestroySession(ISession session);

        Int32 GetSessionCount();
    }
}
