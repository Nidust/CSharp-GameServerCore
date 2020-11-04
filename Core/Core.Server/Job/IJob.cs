using System;

namespace Core.Server.Job
{
    public interface IJob
    {
        void Do();

        int GetId();
    }
}
