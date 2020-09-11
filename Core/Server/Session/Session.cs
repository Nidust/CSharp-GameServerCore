﻿using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Lock;
using System;

namespace Core.Server.Session
{
    public abstract class Session : Locker, ISession
    {
        #region Properties
        private ISessionManager mManager;
        protected ClientSocket mSocket;

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
            WriteLock();
            {
                mSender.Send(mSocket, packet);
            }
            WriteUnlock();
        }

        public void Disconnect()
        {
            Dispose();
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
            IPacket receivePacket;

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
            Console.Error.WriteLine(e.Exception);
            Disconnect();
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