using System;
using System.Net;
using System.Net.Sockets;
using SystemSocket = System.Net.Sockets.Socket;

namespace Core.Network.Socket
{
    public class NetworkSocket : AsyncClientSocketEventDispatcher, INetworkSocket
    {
        #region Properties
        private SystemSocket mConnection;
        private IPAddress mAddress;
        private Int32 mPort;

        private Object mCalledClosedLock;
        private Boolean mCalledClosed;

        private NetworkConnectionType mConnectionType;

        public Int32 MaxReceiveBufferSize { get; }
        public Int32 MaxSendBufferSize { get; }
        #endregion

        #region Methods
        public NetworkSocket(NetworkConnectionType type = NetworkConnectionType.Normal)
        {
            mCalledClosedLock = new Object();
            mCalledClosed = false;

            mConnectionType = type;
        }

        public NetworkSocket(int maxReceiveBufferSize, int maxSendBufferSize, NetworkConnectionType type = NetworkConnectionType.Normal)
            : this(type)
        {
            MaxReceiveBufferSize = maxReceiveBufferSize;
            MaxSendBufferSize = maxSendBufferSize;
        }

        public NetworkSocket(SystemSocket connection, int maxReceiveBufferSize, int maxSendBufferSize, NetworkConnectionType type = NetworkConnectionType.Normal)
            : this(maxReceiveBufferSize, maxSendBufferSize, type)
        {
            mConnection = connection;
            mConnection.ReceiveBufferSize = MaxReceiveBufferSize;
            mConnection.SendBufferSize = MaxSendBufferSize;
        }

        public bool Connect(String hostAddress, Int32 port)
        {
            try
            {
                mAddress = Dns.GetHostAddresses(hostAddress)[0];
                mPort = port;
                EndPoint remoteEndPoint = new IPEndPoint(mAddress, mPort);

                mConnection = new SystemSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mConnection.ReceiveTimeout = 60 * 1000;
                mConnection.SendTimeout = 60 * 1000;
                mConnection.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectResultCallBack), new AsyncIOConnectContext(mConnection));

                return true;
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));

                TryReconnect();
                return false;
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

                if (IsCalledClosed(true))
                {
                    Disconnected();
                }
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
            finally
            {
                TryReconnect();
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

                TryReconnect();
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

                    while (bytesWritten != buffer.Length)
                    {
                        bytesWritten += mConnection.Send(buffer);
                    }

                    Sent(new AsyncSocketSendEventArgs(bytesWritten));

                    return true;
                }

                AsyncIOSendContext context = new AsyncIOSendContext(mConnection, buffer);
                mConnection.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendResultCallBack), context);

                return true;
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));

                TryReconnect();
                return false;
            }
        }

        public void Receive()
        {
            try
            {
                // TODO: ContextPool
                AsyncIOReceiveContext context = new AsyncIOReceiveContext(mConnection, MaxReceiveBufferSize);
                mConnection.BeginReceive(context.Buffer, 0, context.Buffer.Length, 0, new AsyncCallback(ReceiveResultCallback), context);
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));

                TryReconnect();
            }
        }
        #endregion

        #region Private
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

                TryReconnect();
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

                context.Connection.Dispose();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
            finally
            {
                TryReconnect();
            }
        }

        private void TryReconnect()
        {
            if (mConnectionType != NetworkConnectionType.TryReconnect)
            {
                return;
            }

            mConnection.Dispose();
            mConnection = null;

            Connect(mAddress.ToString(), mPort);
        }

        private void ReceiveResultCallback(IAsyncResult ar)
        {
            try
            {
                AsyncIOReceiveContext context = (AsyncIOReceiveContext)ar.AsyncState;
                Int32 bytesRead = context.Connection.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Received(new AsyncSocketReceiveEventArgs(context.Buffer, bytesRead));
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

                TryReconnect();
            }
        }

        private void SendResultCallBack(IAsyncResult ar)
        {
            try
            {
                AsyncIOSendContext context = (AsyncIOSendContext)ar.AsyncState;
                context.BytesWritten += context.Connection.EndSend(ar);

                if (context.SendComplete() == false)
                {
                    // 나머지 보낸다
                    mConnection.BeginSend(context.Buffer, context.BytesWritten, context.Buffer.Length, 0, new AsyncCallback(SendResultCallBack), context);
                    return;
                }

                Sent(new AsyncSocketSendEventArgs(context.BytesWritten));
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));

                TryReconnect();
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
            if (mConnection.Connected)
            {
                Close();
            }

            base.ErrorOccured(args);
        }
        #endregion
    }
}
