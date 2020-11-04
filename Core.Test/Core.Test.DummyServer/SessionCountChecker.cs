using Core.Logger;
using Core.Server.Job;
using Core.Server.Session;
using System;

namespace Core.Test.DummyServer
{
    public class SessionCountChecker : TimerJob
    {
        private ISessionManager mManager;

        public SessionCountChecker(ISessionManager mananger)
        {
            ElapsedTime = new TimeSpan(0, 0, 5);
            Repeat = true;

            mManager = mananger;
        }

        public override void Do()
        {
            Debug.Log($"Client Session Count:{mManager.GetSessionCount()}");
        }

        public override int GetId()
        {
            return 0;
        }
    }
}
