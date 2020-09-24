using Core.Server.Job;
using System;

namespace Core.Server.Threaded
{
    public interface IRunnable : IDisposable
    {
        void SetWorker(WorkerThread worker);

        void OnUpdate();

        void PushJob(IJob job);
        void PushDbJob(IDbJob job);
        void PushTimerJob(TimerJob timerJob);
    }
}
