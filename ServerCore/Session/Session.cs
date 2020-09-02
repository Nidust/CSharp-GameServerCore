using ServerCore.Network;
using ServerCore.Packet;
using System;

namespace ServerCore.Session
{
    public abstract class Session : ISession
    {
        #region Properties
        private ISessionManager mManager;
        private ClientSocket mSocket;

        private PacketSender mSender;
        private PacketReceiver mReceiver;

        // TODO: ReaderWriterLock 구현
        private Object mSessionLock;
        #endregion

        #region Abstract Methods
        public abstract void OnConnect();
        public abstract void OnDisconnect();
        public abstract void OnPacket(IPacket packet);
        public abstract void OnSend(); // 무슨 패킷 보냈는지도 필요할까..?
        #endregion

        #region Methods
        private Session()
        {
            mSessionLock = new Object();

            mSender = new PacketSender();
            mReceiver = new PacketReceiver();
        }

        public Session(ISessionManager manager, ClientSocket socket)
            : this()
        {
            mManager = manager;
            
            mSocket = socket;
            mSocket.OnDisconnect += new AsyncSocketDisconnectEventHandler(OnDisconnectEvent);
            mSocket.OnError += new AsyncSocketErrorEventHandler(OnError);
            mSocket.OnSend += new AsyncSocketSendEventHandler(OnSendEvent);
            mSocket.OnReceive += new AsyncSocketReceiveEventHandler(OnReceiveEvent);
        }

        public virtual void Send(IPacket packet)
        {
            lock (mSessionLock)
            {
                mSender.Send(mSocket, packet);
            }
        }

        public void Disconnect()
        {
            lock (mSessionLock)
            {
                mSocket.Close();
            }
        }
        #endregion

        #region Network Events
        private void OnSendEvent(object sender, AsyncSocketSendEventArgs e)
        {
            lock (mSessionLock)
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

            lock (mSessionLock)
            {
                Boolean receiveComeplete = mReceiver.Receiving(e.ReceiveBuffer, out receivePacket);

                if (receiveComeplete == false)
                {
                    return;
                }
            }

            OnPacket(receivePacket);
        }

        private void OnDisconnectEvent(object sender)
        {
            mManager.DestroySession(this);
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            Console.Error.WriteLine(e.Exception);
            Disconnect();
        }
        #endregion
    }
}
