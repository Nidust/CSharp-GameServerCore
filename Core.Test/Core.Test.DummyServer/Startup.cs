using Core.Server.Builder;
using System;

namespace Core.Test.DummyServer
{
    public class Startup : IStartup
    {
        public void PreBuild()
        {
        }

        public void PostBuild()
        {
        }

        public void Run()
        {
            Logger.Logger.WriteLine(ConsoleColor.Magenta, "[Server Ready]");
        }
    }
}
