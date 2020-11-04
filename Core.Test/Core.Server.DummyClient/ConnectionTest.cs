using Core.Server.DummyClient.Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Server.DummyClient
{
    public class ConnectionTesBot : Bot.Bot
    {
        protected override void OnConnect()
        {
        }

        protected override void OnDisconnect()
        {
        }
    }

    [TestClass]
    public class ConnectionTest
    {
        string ip = "127.0.0.1";
        int port = 5000;
        int testcase = 100;

        [TestMethod]
        public void TestConnection()
        {
            for (int index = 0; index < 10; index++)
            {
                Task.Factory.StartNew(ConnectClients);
            }

            Thread.Sleep(10000);
        }

        private void ConnectClients()
        {
            Thread.Sleep(1000);

            BotManager manager = new BotManager();

            for (int index = 0; index < testcase; index++)
            {
                ConnectionTesBot bot = manager.CreateBot<ConnectionTesBot>();
                bot.Connect(ip, port);
            }

            manager.DisconnectAll();
        }
    }
}
