using GameServer.Session.AccountServer;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private static ManualResetEvent Terminated = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            AccountServerSession accountSession = new AccountServerSession();
            accountSession.Connect("127.0.0.1", 5001);

            Terminated.WaitOne();
        }
    }
}
