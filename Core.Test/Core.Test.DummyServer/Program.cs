using Core.Server.Builder;

namespace Core.Test.DummyServer
{
    class Program
    {
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
                        config.AddListener(5000, new ClientSessionManager());
                    })
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
            }
        }
    }
}
