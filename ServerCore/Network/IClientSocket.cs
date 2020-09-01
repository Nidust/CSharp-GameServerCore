using System.Net.Sockets;

namespace ServerCore.Network
{
    public interface IClientSocket
    {
        bool Send(byte[] buffer, bool blocking = false);
        void Close(SocketShutdown shutdownOption = SocketShutdown.Both);
        void BlockingClose(bool enableShutdown = false, SocketShutdown shutdownOption = SocketShutdown.Both);
    }
}
