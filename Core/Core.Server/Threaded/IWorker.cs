using Core.Server.Job;

namespace Core.Server.Threaded
{
    public interface IWorker
    {
        void PushJob(IJob job);
        void PushDbJob(IDbJob job);
        void PushTimerJob(TimerJob timerJob);
    }
}
