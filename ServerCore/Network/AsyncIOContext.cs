using System;
using System.Net.Sockets;

namespace ServerCore.Network
{
    public abstract class AsyncIOContext
    {
        public Socket Connection { get; }

        public AsyncIOContext(Socket connection)
        {
            Connection = connection;
        }
    }

    public class AsyncIOConnectContext : AsyncIOContext
    {
        public AsyncIOConnectContext(Socket connection)
            : base(connection)
        {
        }
    }

    public class AsyncIODisconnectContext : AsyncIOContext
    {
        public AsyncIODisconnectContext(Socket connection)
            : base(connection)
        {
        }
    }

    public class AsyncIOSendContext : AsyncIOContext
    {
        public Byte[] Buffer { get; }
        public Int32 BytesWritten { get; set; }

        public AsyncIOSendContext(Socket connection, Byte[] buffer)
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

        public AsyncIOReceiveContext(Socket connection, int bufferSize) 
            : base(connection)
        {
            Buffer = new Byte[bufferSize];
        }
    }
}