using System;
using System.Net.Sockets;

namespace ServerCore.Network
{
    public abstract class AsyncSocketEventArgs : EventArgs
    {
    }

    public class AsyncSocketErrorEventArgs : AsyncSocketEventArgs
    {
        public Exception Exception { get; }

        public AsyncSocketErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }

    public class AsyncSocketAcceptEventArgs : AsyncSocketEventArgs
    {
        public Socket Connection { get; }

        public AsyncSocketAcceptEventArgs(Socket connection)
        {
            Connection = connection;
        }
    }

    public class AsyncSocketSendEventArgs : AsyncSocketEventArgs
    {
        public Int32 BytesWritten { get; }
        public Boolean SendComplete { get; }

        public AsyncSocketSendEventArgs(Int32 sendBytes, Boolean sendComplete)
        {
            BytesWritten = sendBytes;
            SendComplete = sendComplete;
        }
    }

    public class AsyncSocketReceiveEventArgs : AsyncSocketEventArgs
    {
        public Byte[] ReceiveBuffer { get; }

        public AsyncSocketReceiveEventArgs(Byte[] buffer)
        {
            ReceiveBuffer = buffer;
        }
    }
}
