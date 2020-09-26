using System;

namespace Core.Server.Builder
{
    public interface IServerBuilder : IDisposable
    {
        void Build();
        void Run();
    }
}
