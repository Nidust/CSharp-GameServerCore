using Core.Server.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server.Threaded
{
    public interface IWorkThread
    {
        void PushJob(IJob job);
        void PushDbJob(IDbJob job);
        void PushTimerJob(TimerJob timerJob);
    }
}
