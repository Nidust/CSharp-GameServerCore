using Core.Network.Packet;

namespace Core.Server.Session
{
    public interface ISession
    {
        void Send(IPacket packet);
    }
}
