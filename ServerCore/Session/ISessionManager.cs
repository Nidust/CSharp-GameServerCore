using ServerCore.Packet;

namespace ServerCore.Session
{
    public interface ISessionManager
    {
        void Broadcast(IPacket packet);
        void DestroySession(ISession session);
    }
}
