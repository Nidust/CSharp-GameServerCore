using System;

namespace Core.Network.Socket
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
        public ClientSocket Connection { get; }

        public AsyncSocketAcceptEventArgs(ClientSocket connection)
        {
            Connection = connection;
        }
    }

    public class AsyncSocketSendEventArgs : AsyncSocketEventArgs
    {
        public Int32 BytesWritten { get; }

        public AsyncSocketSendEventArgs(Int32 sendBytes)
        {
            BytesWritten = sendBytes;
        }
    }

    public class AsyncSocketReceiveEventArgs : AsyncSocketEventArgs
    {
        public Byte[] ReceiveBuffer { get; }
        public Int32 ReceiveBytes { get; }

        public AsyncSocketReceiveEventArgs(Byte[] buffer, Int32 receiveBytes)
        {
            ReceiveBuffer = buffer;
            ReceiveBytes = receiveBytes;
        }
    }
}
