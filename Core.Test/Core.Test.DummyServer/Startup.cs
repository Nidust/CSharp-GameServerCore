using Core.Logger;
using Core.Server.Builder;

namespace Core.Test.DummyServer
{
    public class Startup : IStartup
    {
        public void PostBuild()
        {
        }

        public void PreBuild()
        {
        }

        public void Run()
        {
            Info.Log("--Server Ready");
        }
    }
}
