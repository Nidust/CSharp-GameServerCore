using Core.Network.Socket;
using Core.Server.Session;

namespace Core.Test.DummyServer
{
    public class ClientSessionManager : SessionManager
    {
        public ClientSessionManager() 
            : base(65535, 65535)
        {
        }

        protected override Session CreateSession(ISessionManager manager, NetworkSocket connection)
        {
            return new ClientSession(this, connection);
        }
    }
}
