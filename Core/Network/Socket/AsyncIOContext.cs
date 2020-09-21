using System;
using NetworkSocket = System.Net.Sockets.Socket;

namespace Core.Network.Socket
{
    public abstract class AsyncIOContext
    {
        public System.Net.Sockets.Socket Connection { get; }

        public AsyncIOContext(System.Net.Sockets.Socket connection)
        {
            Connection = connection;
        }
    }

    public class AsyncIOConnectContext : AsyncIOContext
    {
        public AsyncIOConnectContext(System.Net.Sockets.Socket connection)
            : base(connection)
        {
        }
    }

    public class AsyncIODisconnectContext : AsyncIOContext
    {
        public AsyncIODisconnectContext(System.Net.Sockets.Socket connection)
            : base(connection)
        {
        }
    }

    public class AsyncIOSendContext : AsyncIOContext
    {
        public Byte[] Buffer { get; }
        public Int32 BytesWritten { get; set; }

        public AsyncIOSendContext(System.Net.Sockets.Socket connection, Byte[] buffer)
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

        public AsyncIOReceiveContext(System.Net.Sockets.Socket connection, int bufferSize) 
            : base(connection)
        {
            Buffer = new Byte[bufferSize];
        }
    }
}