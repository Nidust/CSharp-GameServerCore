using Core.Logger;
using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Network;
using System;
using System.Collections.Generic;

namespace Core.Server.Session
{
    public abstract class SessionManager : ISessionManager
    {
        #region Properties
        private Listener mListener;

        private Int32 mMaxReceiveBufferSize;
        private Int32 mMaxSendBufferSize;

        private List<ISession> mSessions;
        #endregion

        #region Abstract Methods
        protected abstract Session CreateSession(ISessionManager manager, NetworkSocket connection);
        #endregion

        #region Methods
        public SessionManager(Int32 maxReceiveBufferSize, Int32 maxSendBufferSize)
        {
            mSessions = new List<ISession>();

            mMaxReceiveBufferSize = maxReceiveBufferSize;
            mMaxSendBufferSize = maxSendBufferSize;
        }

        public void StartListen(Int32 port)
        {
            mListener = new Listener(port, mMaxReceiveBufferSize, mMaxSendBufferSize);
            mListener.OnAccept += new AsyncSocketAcceptEventHandler(OnAcceptEvent);
            mListener.OnError += new AsyncSocketErrorEventHandler(OnErrorEvent);

            mListener.Start();
        }

        public void Broadcast(IPacket packet)
        {
            lock (mSessions)
            {
                foreach (var session in mSessions)
                {
                    session.Send(packet);
                }
            }
        }

        public void DestroySession(ISession session)
        {
            lock (mSessions)
            {
                session.Dispose();
                mSessions.Remove(session);
            }
        }

        public int GetSessionCount()
        {
            lock (mSessions)
            {
                return mSessions.Count;
            }
        }
        #endregion

        #region Network Events
        private void OnAcceptEvent(object sender, AsyncSocketAcceptEventArgs e)
        {
            lock (mSessions)
            {
                Session newSesion = CreateSession(this, e.Connection);
                e.Connection.Connected();

                mSessions.Add(newSesion);
            }
        }

        private void OnErrorEvent(object sender, AsyncSocketErrorEventArgs e)
        {
            Error.Log(e.Exception);
        }
        #endregion
    }
}
