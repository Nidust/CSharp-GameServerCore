using Core.Logger;
using Core.Server.Threaded;
using GameServer.Session.AccountServer;
using System;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private static ManualResetEvent Terminated = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Logger.Initialize(Environment.CurrentDirectory, "GameServer");

            ThreadCoordinator.MaxFramePerSecond = 60;
            ThreadCoordinator.Initialize("GameServer WorkerThread", 4);

            AccountServerSession accountSession = new AccountServerSession();
            accountSession.Connect("127.0.0.1", 5001);

            Info.Log("--Server Ready");

            Terminated.WaitOne();
            Logger.Uninitialize();
        }
    }
}
