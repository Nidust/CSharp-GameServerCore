using AccountServer.Session.GameServerSession;
using System;
using System.Threading;

namespace AccountServer
{
    class Program
    {
        private static ManualResetEvent Terminated = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            GameServerSessionManager sessionManager = new GameServerSessionManager();
            sessionManager.StartListen(5001);

            Console.WriteLine("--Server Ready");

            Terminated.WaitOne();
        }
    }
}
