using System;
using NetworkSocket = System.Net.Sockets.Socket;

namespace Core.Network.Socket
{
    public abstract class AsyncIOContext
    {
        public NetworkSocket Connection { get; }

        public AsyncIOContext(NetworkSocket connection)
        {
            Connection = connection;
        }
    }

    public class AsyncIOConnectContext : AsyncIOContext
    {
        public AsyncIOConnectContext(NetworkSocket connection)
            : base(connection)
        {
        }
    }

    public class AsyncIODisconnectContext : AsyncIOContext
    {
        public AsyncIODisconnectContext(NetworkSocket connection)
            : base(connection)
        {
        }
    }

    public class AsyncIOSendContext : AsyncIOContext
    {
        public Byte[] Buffer { get; }
        public Int32 BytesWritten { get; set; }

        public AsyncIOSendContext(NetworkSocket connection, Byte[] buffer)
            : base(connection)
        {
            Buffer = buffer;
        }

        public bool SendComplete()
        {
            return BytesWritten == Buffer.Length;
        }
    }

    public class AsyncIOReceiveContext : AsyncIOContext
    {
        public Byte[] Buffer { get; set; }

        public AsyncIOReceiveContext(NetworkSocket connection, int bufferSize) 
            : base(connection)
        {
            Buffer = new Byte[bufferSize];
        }
    }
}