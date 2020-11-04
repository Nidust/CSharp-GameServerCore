using Core.Logger;
using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Job;
using Core.Server.Lock;
using Core.Server.Threaded;
using System;
using System.ComponentModel;
using System.Threading;

namespace Core.Server.Session
{
    public abstract class Session : Locker, ISession, IRunnable, IWorker
    {
        #region Properties
        protected ISessionManager mManager;
        protected NetworkSocket mSocket;

        private PacketSender mSender;
        private PacketReceiver mReceiver;

        private Int32 mIndex;
        private static Int32 Index = 0;
        #endregion

        #region Abstract Methods
        protected abstract void OnConnect();
        protected abstract void OnDisconnect();
        protected abstract void OnPacket(IPacket packet);
        protected abstract void OnSend(); // 무슨 패킷 보냈는지도 필요할까..?
        public abstract void OnUpdate();
        #endregion

        #region Methods
        private Session()
        {
            mManager = null;

            mSender = new PacketSender();
            mReceiver = new PacketReceiver();

            mIndex = Interlocked.Increment(ref Index);
            ThreadCoordinator.AddRunnable(this);
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
            mSender.Send(mSocket, packet);
        }

        public void PushJob(IJob job)
        {
            ThreadCoordinator.PushJob(this, job);
        }

        public void PushDbJob(IDbJob job)
        {
            ThreadCoordinator.PushDbJob(this, job);
        }

        public void PushTimerJob(TimerJob timerJob)
        {
            ThreadCoordinator.PushTimerJob(this, timerJob);
        }

        public int GetId()
        {
            return mIndex;
        }

        public void Dispose()
        {
            mSocket.Close();
            mSender.Dispose();
        }
        #endregion

        #region Network Events
        private void OnSendEvent(object sender, AsyncSocketSendEventArgs e)
        {
            Boolean sendComplete = mSender.Sending(mSocket, (UInt16)e.BytesWritten);

            if (sendComplete == false)
            {
                return;
            }

            OnSend();
        }

        private void OnReceiveEvent(object sender, AsyncSocketReceiveEventArgs e)
        {
            IPacket receivePacket = null;
            Boolean receiveComeplete = mReceiver.Receiving(e.ReceiveBuffer, e.ReceiveBytes, out receivePacket);

            if (receiveComeplete == false)
            {
                return;
            }

            OnPacket(receivePacket);
        }

        private void OnConnectEvent(object sender)
        {
            OnConnect();
        }

        private void OnDisconnectEvent(object sender)
        {
            ThreadCoordinator.RemoveRunnable(this);

            if (mManager != null)
                mManager.DestroySession(this);

            OnDisconnect();
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
        #endregion
    }
}