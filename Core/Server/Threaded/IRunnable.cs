using System;

namespace Core.Server.Threaded
{
    public interface IRunnable : IDisposable
    {
        void OnUpdate();
        Int32 GetId();
        void SetworkerThreadId(Int32 workerThreadId);
        Int32 GetWorkerThreadId();
    }
}
