using ServerCore.Packet;

namespace ServerCore.Session
{
    public interface ISession
    {
        void Send(IPacket packet);
    }
}
