using Core.Network.Packet;
using Core.Network.Socket;
using System;

namespace Core.Server.Session
{
    public abstract class Session : ISession
    {
        #region Properties
        private ISessionManager mManager;
        protected ClientSocket mSocket;

        private PacketSender mSender;
        private PacketReceiver mReceiver;

        // TODO: ReaderWriterLock 구현
        protected Object mLock;
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
            mLock = new Object();

            mSender = new PacketSender();
            mReceiver = new PacketReceiver();
        }

        public Session(Int32 receiveBufferSize, Int32 sendBufferSize)
            : this()
        {
            mSocket = new ClientSocket(receiveBufferSize, sendBufferSize);
            mSocket.OnConnect += new AsyncSocketConnectEventHandler(OnConnectEvent);
            mSocket.OnDisconnect += new AsyncSocketDisconnectEventHandler(OnDisconnectEvent);
            mSocket.OnError += new AsyncSocketErrorEventHandler(OnError);
            mSocket.OnSend += new AsyncSocketSendEventHandler(OnSendEvent);
            mSocket.OnReceive += new AsyncSocketReceiveEventHandler(OnReceiveEvent);
        }

        public Session(ISessionManager manager, ClientSocket socket)
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

        public virtual void Send(IPacket packet)
        {
            lock (mLock)
            {
                mSender.Send(mSocket, packet);
            }
        }

        public void Disconnect()
        {
            lock (mLock)
            {
                mSocket.Close();
            }
        }
        #endregion

        #region Network Events
        private void OnSendEvent(object sender, AsyncSocketSendEventArgs e)
        {
            lock (mLock)
            {
                Boolean sendComplete = mSender.Sending(mSocket, (UInt16)e.BytesWritten);

                if (sendComplete == false)
                {
                    return;
                }
            }

            OnSend();
        }

        private void OnReceiveEvent(object sender, AsyncSocketReceiveEventArgs e)
        {
            IPacket receivePacket;

            lock (mLock)
            {
                Boolean receiveComeplete = mReceiver.Receiving(e.ReceiveBuffer, e.ReceiveBytes, out receivePacket);

                if (receiveComeplete == false)
                {
                    return;
                }
            }

            OnPacket(receivePacket);
        }

        private void OnConnectEvent(object sender)
        {
            OnConnect();
        }

        private void OnDisconnectEvent(object sender)
        {
            if (mManager != null)
                mManager.DestroySession(this);

            OnDisconnect();
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            Console.Error.WriteLine(e.Exception);
            Disconnect();
        }
        #endregion
    }
}
