﻿using Core.Network.Packet;
using Core.Network.Socket;
using Core.Server.Network;
using System;
using System.Collections.Generic;

namespace Core.Server.Session
{
    public abstract class SessionManager : ISessionManager
    {
        #region Properties
        private Object mLock;
        private Listener mListener;

        private List<ISession> mSessions;
        #endregion

        #region Abstract Methods
        protected abstract Session CreateSession(ISessionManager manager, ClientSocket connection);
        #endregion

        #region Methods
        public SessionManager()
        {
            mLock = new Object();
            mSessions = new List<ISession>();
        }

        public void StartListen(Int32 port)
        {
            mListener = new Listener(port);
            mListener.OnAccept += new AsyncSocketAcceptEventHandler(OnAcceptEvent);
            mListener.OnError += new AsyncSocketErrorEventHandler(OnErrorEvent);

            mListener.Start();
        }

        public void Broadcast(IPacket packet)
        {
            lock (mLock)
            {
                foreach (var session in mSessions)
                {
                    session.Send(packet);
                }
            }
        }

        public void DestroySession(ISession session)
        {
            lock (mLock)
            {
                mSessions.Remove(session);
            }
        }
        #endregion

        #region Network Events
        private void OnAcceptEvent(object sender, AsyncSocketAcceptEventArgs e)
        {
            lock (mLock)
            {
                Session newSesion = CreateSession(this, e.Connection);
                e.Connection.Connected();

                mSessions.Add(newSesion);
            }
        }

        private void OnErrorEvent(object sender, AsyncSocketErrorEventArgs e)
        {
            Console.Error.WriteLine($"Accpet Error: {e}");
        }
        #endregion
    }
}