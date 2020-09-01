using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore.Network
{
    public class ClientSocket : AsyncClientSocketEventDispatcher, IClientSocket
    {
        private Socket mConnection;
        
        private Int32 mMaxReceiveBufferSize;
        private Int32 mMaxSendBufferSize;

        private Object mCalledClosedLock;
        private Boolean mCalledClosed;

        public ClientSocket()
        {
            mCalledClosedLock = new object();
            mCalledClosed = false;
        }

        public ClientSocket(int maxReceiveBufferSize, int maxSendBufferSize)
            : this()
        {
            mMaxReceiveBufferSize = maxReceiveBufferSize;
            mMaxSendBufferSize = maxSendBufferSize;
        }

        public ClientSocket(Socket connection, int maxReceiveBufferSize, int maxSendBufferSize)
            : this(maxReceiveBufferSize, maxSendBufferSize)
        {
            mConnection = connection;
            mConnection.ReceiveBufferSize = mMaxReceiveBufferSize;
            mConnection.SendBufferSize = mMaxSendBufferSize;
        }

        public bool Connect(String hostAddress, Int32 port)
        {
            try
            {
                IPAddress[] ips = Dns.GetHostAddresses(hostAddress);
                IPEndPoint remoteEndPoint = new IPEndPoint(ips[0], port);

                AsyncIOConnectContext context = new AsyncIOConnectContext(mConnection);
                mConnection.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectResultCallBack), context);

                return true;
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
                return false;
            }
        }

        private void ConnectResultCallBack(IAsyncResult ar)
        {
            try
            {
                AsyncIOConnectContext context = (AsyncIOConnectContext)ar.AsyncState;
                context.Connection.EndConnect(ar);

                Receive();

                IsCalledClosed(false);

                Connected();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        public void BlockingClose(bool enableShutdown = false, SocketShutdown shutdownOption = SocketShutdown.Both)
        {
            try
            {
                if (enableShutdown)
                {
                    mConnection.Shutdown(shutdownOption);
                }

                mConnection.Close();
                mConnection.Dispose();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        public void Close(SocketShutdown shutdownOption = SocketShutdown.Both)
        {
            try
            {
                AsyncIODisconnectContext context = new AsyncIODisconnectContext(mConnection);

                mConnection.Shutdown(shutdownOption);
                mConnection.BeginDisconnect(false, new AsyncCallback(DisconnectResultCallBack), context);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        private void DisconnectResultCallBack(IAsyncResult ar)
        {
            try
            {
                AsyncIODisconnectContext context = (AsyncIODisconnectContext)ar.AsyncState;
                context.Connection.EndDisconnect(ar);

                if (IsCalledClosed(true) == false)
                {
                    Disconnected();
                }

                mConnection.Dispose();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        // TODO: ReceivedComplete
        // 한번에 못받는 경우가 생길 수 있다.
        // 패킷 프로토콜 정해서 버퍼 크기 넘어가더라도 받아올 수 있도록 수정하자
        private void Receive()
        {
            try
            {
                // TODO: ContextPool
                AsyncIOReceiveContext context = new AsyncIOReceiveContext(mConnection, mMaxReceiveBufferSize);
                mConnection.BeginReceive(context.Buffer, 0, context.Buffer.Length, 0, new AsyncCallback(ReceiveResultCallback), context);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        private void ReceiveResultCallback(IAsyncResult ar)
        {
            try
            {
                AsyncIOReceiveContext context = (AsyncIOReceiveContext)ar.AsyncState;
                Int32 bytesRead = context.Connection.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Received(new AsyncSocketReceiveEventArgs(context.Buffer));
                }
                else if (bytesRead == 0)
                {
                    if (IsCalledClosed(true) == false)
                    {
                        Disconnected();
                    }

                    mConnection.Dispose();
                    return;
                }

                Receive();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        public bool Send(Byte[] buffer, bool blocking = false)
        {
            // 빈 버퍼는 보내지 않는다
            if (buffer.Length <= 0)
            {
                return false;
            }

            try
            {
                // 한번에 못보내는 경우는 총 두 가지이다.
                // 1. 네트워크 상태 (극히 드뭄)
                // 2. buffer가 한번에 보낼 수 있는 size를 넘어갔을 때

                if (blocking)
                {
                    Int32 bytesWritten = 0;
                    Boolean sendComplete = false;

                    while (sendComplete == false)
                    {
                        bytesWritten += mConnection.Send(buffer);
                        sendComplete = bytesWritten == buffer.Length;

                        Sent(new AsyncSocketSendEventArgs(bytesWritten, sendComplete));
                    }

                    return true;
                }

                AsyncIOSendContext context = new AsyncIOSendContext(mConnection, buffer);
                mConnection.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendResultCallBack), context);

                return true;
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
                return false;
            }
        }

        private void SendResultCallBack(IAsyncResult ar)
        {
            try
            {
                AsyncIOSendContext context = (AsyncIOSendContext)ar;
                context.BytesWritten += context.Connection.EndSend(ar);

                Sent(new AsyncSocketSendEventArgs(context.BytesWritten, context.SendComplete()));

                if (context.SendComplete() == false)
                {
                    // 나머지 보낸다
                    mConnection.BeginSend(context.Buffer, context.BytesWritten, context.Buffer.Length, 0, new AsyncCallback(SendResultCallBack), context);
                    return;
                }
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        private bool IsCalledClosed(bool called)
        {
            lock (mCalledClosedLock)
            {
                var isCalledClosed = mCalledClosed;

                mCalledClosed = called;

                return isCalledClosed;
            }
        }

        protected override void ErrorOccured(AsyncSocketErrorEventArgs args)
        {
            base.ErrorOccured(args);

            mConnection.Dispose();
        }
    }
}
