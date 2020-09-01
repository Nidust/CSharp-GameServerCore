using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore.Network
{
    public sealed class Listener : AsyncListenerEventDispatcher
    {
        private Socket mSocket;
        private Int32 mPort;
        private Int32 mListening;

        private Listener(int port)
        {
            mPort = port;
        }

        public static Listener Create(int port)
        {
            return new Listener(port);
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

                Accepted(new AsyncSocketAcceptEventArgs(connection));

                BeginAccept();
            }
            catch (Exception e)
            {
                ErrorOccured(new AsyncSocketErrorEventArgs(e));
            }
        }
    }
}
