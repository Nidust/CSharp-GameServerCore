namespace ServerCore.Session
{
    public interface ISessionManager
    {
        void CreateSession(ISession session);
        void DestroySession(ISession session);
    }
}