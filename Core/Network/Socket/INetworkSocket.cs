using System.Net.Sockets;

namespace Core.Network.Socket
{
    public interface INetworkSocket
    {
        bool Send(byte[] buffer, bool blocking = false);
        void Close(SocketShutdown shutdownOption = SocketShutdown.Both);
        void BlockingClose(bool enableShutdown = false, SocketShutdown shutdownOption = SocketShutdown.Both);
    }
}
