using System;
using System.Collections.Generic;

namespace Core.Server.Job
{
    public class JobExecuter
    {
        #region Properties
        private Stack<IJob> mJobs;

        private List<TimerJob> mTimerJobs;
        private List<TimerJob> mTimerJobsToRemove;
        #endregion

        #region Methods
        public JobExecuter()
        {
            mJobs = new Stack<IJob>();

            mTimerJobs = new List<TimerJob>();
            mTimerJobsToRemove = new List<TimerJob>();
        }

        public void Do()
        {
            lock (mJobs)
            {
                foreach (IJob job in mJobs)
                {
                    job.Do();
                }

                mJobs.Clear();
            }
        }

        public void DoTimer()
        {
            lock (mTimerJobs)
            {
                Int64 currentTick = DateTime.Now.Ticks;

                mTimerJobsToRemove.Clear();

                foreach (TimerJob timerJob in mTimerJobs)
                {
                    if (timerJob.Update(currentTick) == false)
                        continue;

                    mTimerJobsToRemove.Add(timerJob);
                }

                foreach (TimerJob timerJob in mTimerJobsToRemove)
                {
                    mTimerJobs.Remove(timerJob);
                }
            }
        }

        public void Push(IJob job)
        {
            lock (mJobs)
            {
                mJobs.Push(job);
            }
        }

        public void Push(TimerJob timerJob)
        {
            lock (mTimerJobs)
            {
                mTimerJobs.Add(timerJob);
            }
        }
        #endregion
    }
}
