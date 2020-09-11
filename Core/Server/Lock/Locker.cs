using System;
using System.Threading;

namespace Core.Server.Lock
{
    public abstract class Locker
    {
        #region Properties
        private ReaderWriterLock mLock;
        private static readonly TimeSpan LockTimeout = new TimeSpan(0, 10, 0);
        #endregion

        #region Method
        public Locker()
        {
            mLock = new ReaderWriterLock();
        }

        public void ReadLock()
        {
            try
            {
                mLock.AcquireReaderLock(LockTimeout.Milliseconds);
            }
            catch (ApplicationException)
            {
                Environment.FailFast("ReadLock Timeout");
            }
        }

        public void ReadUnlock()
        {
            mLock.ReleaseReaderLock();
        }

        public void WriteLock()
        {
            try
            {
                if (mLock.IsReaderLockHeld)
                    throw new Exception("WriterLock Fail.. ReaderLockHeld");

                mLock.AcquireWriterLock(LockTimeout);
            }
            catch (ApplicationException)
            {
                Environment.FailFast("Writelock Timeout");
            }
            catch (Exception e)
            {
                Environment.FailFast(e.Message);
            }
        }

        public void WriteUnlock()
        {
            mLock.ReleaseWriterLock();
        }
        #endregion
    }
}
