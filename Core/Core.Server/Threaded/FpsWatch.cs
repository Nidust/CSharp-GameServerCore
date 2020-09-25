using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core.Server.Threaded
{
    internal class FpsWatch
    {
        #region Properties
        public Int32 FramePerMilliseconds { get; private set; }

        private const Int32 MaximumSamplingTickCount = 100;

        private Stopwatch mWatch;        
        private Queue<Int64> mSampleTicks;

        private Int64 mSampleTotal;
        private Int64 mPreviousTicks;
        private Int64 mCurrentTicks;

        #endregion

        #region Methods
        public FpsWatch()
        {
            mWatch = new Stopwatch();
            mSampleTicks = new Queue<Int64>(MaximumSamplingTickCount);

            mPreviousTicks = mWatch.ElapsedTicks;
        }

        public void Begin()
        {
            mCurrentTicks = mWatch.ElapsedTicks;
            
            if (mSampleTicks.Count >= MaximumSamplingTickCount)
                mSampleTotal -= mSampleTicks.Dequeue();

            Int64 deltaTicks = mCurrentTicks - mPreviousTicks;

            mSampleTicks.Enqueue(deltaTicks);
            mSampleTotal += deltaTicks;
        }

        public void End()
        {
            Int64 average = mSampleTotal / mSampleTicks.Count;

            FramePerMilliseconds = (Int32)((Stopwatch.Frequency / average) / 1000);
        }
        #endregion
    }
}
