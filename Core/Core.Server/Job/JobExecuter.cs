using System;
using System.Collections.Generic;

namespace Core.Server.Job
{
    public class JobExecuter
    {
        #region Properties
        private Stack<IJob> mJobs;
        private Stack<IDbJob> mDbJobs;
        
        private List<TimerJob> mTimerJobs;
        private List<TimerJob> mTimerJobsToRemove;
        #endregion

        #region Methods
        public JobExecuter()
        {
            mJobs = new Stack<IJob>();
            mDbJobs = new Stack<IDbJob>();

            mTimerJobs = new List<TimerJob>();
            mTimerJobsToRemove = new List<TimerJob>();
        }

        public void DoJob()
        {
            lock (mJobs)
            {
                if (mJobs.Count == 0)
                    return;

                foreach (IJob job in mJobs)
                {
                    job.Do();
                }

                mJobs.Clear();
            }
        }

        public void DoDbJob()
        {
            lock (mDbJobs)
            {
                if (mDbJobs.Count == 0)
                    return;

                foreach (IDbJob job in mDbJobs)
                {
                    job.Do();
                }

                mDbJobs.Clear();
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

        public void PushJob(IJob job)
        {
            lock (mJobs)
            {
                mJobs.Push(job);
            }
        }

        public void PushDbJob(IDbJob job)
        {
            lock (mDbJobs)
            {
                mDbJobs.Push(job);
            }
        }

        public void PushTimerJob(TimerJob timerJob)
        {
            lock (mTimerJobs)
            {
                mTimerJobs.Add(timerJob);
            }
        }
        #endregion
    }
}
