using Core.Server.Builder;

namespace Core.Test.DummyServer
{
    public class Program
    {
        public static ClientSessionManager ClientSessionManager = new ClientSessionManager();

        static void Main(string[] args)
        {
            using (ServerHostBuilder builder = ServerHost.CreateDefaultBuilder())
            {
                builder
                    .ConfigureLogging((config) => 
                    {
                        config.SetFileName("DummyServer");
                        config.UseConsole(); 
                    })
                    .ConfigureThread((config) => 
                    {
                        config.SetFps(60);
                        config.SetWorkerThreads(10);
                        config.SetName("DummyWorkerThread"); 
                    })
                    .ConfigureListeners((config) =>
                    {
                        config.AddListener(5000, ClientSessionManager);
                    })
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
            }
        }
    }
}
