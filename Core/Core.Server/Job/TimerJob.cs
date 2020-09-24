using System;

namespace Core.Server.Job
{
    public abstract class TimerJob : IJob
    {
        #region Properties
        public TimeSpan ElapsedTime { get; protected set; }
        public Boolean Repeat { get; protected set; }

        private Int64 mLastTick;
        #endregion

        #region Abstract Methods
        public abstract void Do();
        #endregion

        #region Methods
        public TimerJob()
        {
            mLastTick = DateTime.Now.Ticks;
        }

        public Boolean Update(Int64 currentTick)
        {
            if (currentTick - mLastTick < ElapsedTime.Ticks)
            {
                return false;
            }

            Do();
            mLastTick = currentTick;

            return Repeat == false;
        }
        #endregion
    }
}
