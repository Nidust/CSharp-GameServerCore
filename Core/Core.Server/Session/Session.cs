using Core.Logger;
using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Lock;
using System;
using System.ComponentModel;

namespace Core.Server.Session
{
    public abstract class Session : Locker, ISession
    {
        #region Properties
        protected ISessionManager mManager;
        protected NetworkSocket mSocket;

        private PacketSender mSender;
        private PacketReceiver mReceiver;
        #endregion

        #region Abstract Methods
        protected abstract void OnConnect();
        protected abstract void OnDisconnect();
        protected abstract void OnPacket(IPacket packet);
        protected abstract void OnSend(); // 무슨 패킷 보냈는지도 필요할까..?
        #endregion

        #region Methods
        private Session()
        {
            mManager = null;

            mSender = new PacketSender();
            mReceiver = new PacketReceiver();
        }

        public Session(Int32 receiveBufferSize, Int32 sendBufferSize, NetworkConnectionType connectionType)
            : this()
        {
            mSocket = new NetworkSocket(receiveBufferSize, sendBufferSize, connectionType);
            mSocket.OnConnect += new AsyncSocketConnectEventHandler(OnConnectEvent);
            mSocket.OnDisconnect += new AsyncSocketDisconnectEventHandler(OnDisconnectEvent);
            mSocket.OnError += new AsyncSocketErrorEventHandler(OnError);
            mSocket.OnSend += new AsyncSocketSendEventHandler(OnSendEvent);
            mSocket.OnReceive += new AsyncSocketReceiveEventHandler(OnReceiveEvent);
        }

        public Session(ISessionManager manager, NetworkSocket socket)
            : this()
        {
            mManager = manager;

            mSocket = socket;
            mSocket.OnConnect += new AsyncSocketConnectEventHandler(OnConnectEvent);
            mSocket.OnDisconnect += new AsyncSocketDisconnectEventHandler(OnDisconnectEvent);
            mSocket.OnError += new AsyncSocketErrorEventHandler(OnError);
            mSocket.OnSend += new AsyncSocketSendEventHandler(OnSendEvent);
            mSocket.OnReceive += new AsyncSocketReceiveEventHandler(OnReceiveEvent);
        }

        public void Connect(String ip, Int32 port)
        {
            mSocket.Connect(ip, port);
        }

        public virtual void Send(IPacket packet)
        {
            WriteLock();
            {
                mSender.Send(mSocket, packet);
            }
            WriteUnlock();
        }
        #endregion

        #region Network Events
        private void OnSendEvent(object sender, AsyncSocketSendEventArgs e)
        {
            WriteLock();
            {
                Boolean sendComplete = mSender.Sending(mSocket, (UInt16)e.BytesWritten);

                if (sendComplete == false)
                {
                    return;
                }
            }
            WriteUnlock();

            OnSend();
        }

        private void OnReceiveEvent(object sender, AsyncSocketReceiveEventArgs e)
        {
            IPacket receivePacket = null;

            WriteLock();
            {
                Boolean receiveComeplete = mReceiver.Receiving(e.ReceiveBuffer, e.ReceiveBytes, out receivePacket);

                if (receiveComeplete == false)
                {
                    return;
                }
            }
            WriteUnlock();

            OnPacket(receivePacket);
        }

        private void OnConnectEvent(object sender)
        {
            OnConnect();
        }

        private void OnDisconnectEvent(object sender)
        {
            OnDisconnect();

            if (mManager != null)
                mManager.DestroySession(this);
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            Win32Exception winException = e.Exception as Win32Exception;
            if (winException != null)
            {
                switch (winException.ErrorCode)
                {
                    case 10057:
                    case 10061:
                    case 10054:
                        return;
                }
            }

            Error.Log(e.Exception);
        }

        public void Dispose()
        {
            WriteLock();
            {
                mSocket.Close();
                mSender.Dispose();
            }
            WriteUnlock();
        }
        #endregion
    }
}