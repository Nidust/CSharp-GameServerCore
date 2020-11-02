using Core.Server.DummyClient.Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

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
        [TestMethod]
        public void TestConnection()
        {
            string ip = "127.0.0.1";
            int port = 5000;
            int testcase = 100;

            BotManager manager = new BotManager();

            for (int index = 0; index < testcase; index++)
            {
                ConnectionTesBot bot = manager.CreateBot<ConnectionTesBot>();
                bot.Connect(ip, port);
            }

            Thread.Sleep(1000);

            manager.DisconnectAll();
        }
    }
}
