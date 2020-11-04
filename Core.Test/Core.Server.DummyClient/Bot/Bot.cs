using Core.Network.Packet;
using Core.Network.Socket;
using System;

namespace Core.Server.DummyClient.Bot
{
    public class Bot
    {
        #region Properties
        private NetworkSocket mSocket;

        private PacketReceiver mReceiver;
        private PacketSender mSender;

        public BotManager Manager { get; set; }
        #endregion

        #region Virtual Methods
        protected virtual void OnConnect() { }
        protected virtual void OnDisconnect() { }
        protected virtual void OnPacket(IPacket packet) { }
        protected virtual void OnSend() { } // 무슨 패킷 보냈는지도 필요할까..?
        #endregion

        #region Methods
        public Bot()
        {
            mSocket = new NetworkSocket(65535, 65535);
            mSocket.OnConnect += new AsyncSocketConnectEventHandler(OnConnectEvent);
            mSocket.OnDisconnect += new AsyncSocketDisconnectEventHandler(OnDisconnectEvent);
            mSocket.OnReceive += new AsyncSocketReceiveEventHandler(OnReceiveEvent);
            mSocket.OnSend += new AsyncSocketSendEventHandler(OnSendEvent);

            mReceiver = new PacketReceiver();
            mSender = new PacketSender();
        }

        public void Connect(String ip, int port)
        {
            mSocket.Connect(ip, port);
        }

        public void Disconnect()
        {
            mSocket.Close();
        }

        public void Send(IPacket packet)
        {
            lock (mSender)
            {
                mSender.Send(mSocket, packet);
            }
        }
        #endregion

        #region Network Events
        public void OnConnectEvent(Object sender)
        {
            OnConnect();
        }

        public void OnDisconnectEvent(Object sender)
        {
            OnDisconnect();
        }

        public void OnReceiveEvent(Object sender, AsyncSocketReceiveEventArgs e)
        {
            IPacket receivePacket = null;
            Boolean receiveComeplete = mReceiver.Receiving(e.ReceiveBuffer, e.ReceiveBytes, out receivePacket);

            if (receiveComeplete == false)
            {
                return;
            }

            OnPacket(receivePacket);
        }

        public void OnSendEvent(Object sender, AsyncSocketSendEventArgs e)
        {
            Boolean sendComplete = mSender.Sending(mSocket, (UInt16)e.BytesWritten);

            if (sendComplete == false)
            {
                return;
            }

            OnSend();
        }
        #endregion
    }
}
