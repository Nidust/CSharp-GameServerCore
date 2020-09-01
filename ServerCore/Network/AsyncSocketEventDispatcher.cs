using System;

namespace ServerCore.Network
{
    public delegate void AsyncSocketErrorEventHandler(object sender, AsyncSocketErrorEventArgs e);
    public delegate void AsyncSocketAcceptEventHandler(object sender, AsyncSocketAcceptEventArgs e);
    public delegate void AsyncSocketConnectEventHandler(object sender);
    public delegate void AsyncSocketDisconnectEventHandler(object sender);
    public delegate void AsyncSocketSendEventHandler(object sender, AsyncSocketSendEventArgs e);
    public delegate void AsyncSocketReceiveEventHandler(object sender, AsyncSocketReceiveEventArgs e);

    public abstract class AsyncSocketEventDispatcher
    {
        public event AsyncSocketErrorEventHandler OnError;

        protected virtual void ErrorOccured(AsyncSocketErrorEventArgs args)
        {
            OnError?.Invoke(this, args);
        }
    }

    public class AsyncListenerEventDispatcher : AsyncSocketEventDispatcher
    {
        public event AsyncSocketAcceptEventHandler OnAccept;

        protected void Accepted(AsyncSocketAcceptEventArgs args)
        {
            OnAccept?.Invoke(this, args);
        }
    }

    public class AsyncClientSocketEventDispatcher : AsyncSocketEventDispatcher
    {
        public event AsyncSocketConnectEventHandler OnConnect;
        public event AsyncSocketDisconnectEventHandler OnDisconnect;
        public event AsyncSocketSendEventHandler OnSend;
        public event AsyncSocketReceiveEventHandler OnReceive;

        protected void Connected()
        {
            OnConnect?.Invoke(this);
        }

        protected void Disconnected()
        {
            OnDisconnect?.Invoke(this);
        }

        protected void Sent(AsyncSocketSendEventArgs args)
        {
            OnSend?.Invoke(this, args);
        }

        protected void Received(AsyncSocketReceiveEventArgs args)
        {
            OnReceive?.Invoke(this, args);
        }
    }
}
