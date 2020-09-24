using System;

namespace Core.Network.Socket
{
    public delegate void AsyncSocketErrorEventHandler(Object sender, AsyncSocketErrorEventArgs e);
    public delegate void AsyncSocketAcceptEventHandler(Object sender, AsyncSocketAcceptEventArgs e);
    public delegate void AsyncSocketConnectEventHandler(Object sender);
    public delegate void AsyncSocketDisconnectEventHandler(Object sender);
    public delegate void AsyncSocketSendEventHandler(Object sender, AsyncSocketSendEventArgs e);
    public delegate void AsyncSocketReceiveEventHandler(Object sender, AsyncSocketReceiveEventArgs e);

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

        public void Connected()
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
