using Core.Network.Packet;

namespace Core.Server.Session
{
    public interface ISessionManager
    {
        void Broadcast(IPacket packet);
        void DestroySession(ISession session);
    }
}
