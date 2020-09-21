using Core.Network.Socket;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Core.Server.Network
{
    public class Listener : AsyncListenerEventDispatcher
    {
        #region Properties
        private Socket mSocket;
        private Int32 mPort;
        private Int32 mListening;
        #endregion

        #region Methods
        public Listener(int port)
        {
            mPort = port;
        }

        public void Start()
        {
            try
            {
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSocket.Bind(new IPEndPoint(IPAddress.Any, mPort));
                mSocket.Listen((int)SocketOptionName.MaxConnections);

                Interlocked.Increment(ref mListening);
                BeginAccept();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }

        public void Stop()
        {
            try
            {
                Interlocked.Decrement(ref mListening);

                if (mSocket != null && mSocket.IsBound)
                {
                    mSocket.Close(100);
                }
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }
        #endregion

        #region Private
        private void BeginAccept()
        {
            mSocket.BeginAccept(new AsyncCallback(AcceptResultCallback), mSocket);
        }

        private void AcceptResultCallback(IAsyncResult ar)
        {
            try
            {
                if (mListening <= 0)
                {
                    return;
                }

                Socket listener = (Socket)ar.AsyncState;
                Socket connection = listener.EndAccept(ar);

                NetworkSocket newConnection = new NetworkSocket(connection, 65535, 65535);
                newConnection.Receive();

                Accepted(new AsyncSocketAcceptEventArgs(newConnection));

                BeginAccept();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }
        #endregion
    }
}
