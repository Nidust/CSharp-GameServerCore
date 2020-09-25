using AccountServer.Session.GameServerSession;
using Core.Logger;
using System;
using System.Threading;

namespace AccountServer
{
    class Program
    {
        private static ManualResetEvent Terminated = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Logger.Initialize(Environment.CurrentDirectory, "AccountServer");

            GameServerSessionManager sessionManager = new GameServerSessionManager();
            sessionManager.StartListen(5001);

            Info.Log("--Server Ready");

            Terminated.WaitOne();
            Logger.Uninitialize();
        }
    }
}
