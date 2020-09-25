using System;

namespace Core.Server.Threaded
{
    public interface IRunnable : IDisposable
    {
        void OnUpdate();

        Int32 GetId();
    }
}
