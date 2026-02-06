using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager m_Instance = new SessionManager();
        public static SessionManager Instance { get { return m_Instance; } }

        private List<ServerSession> m_listSession = new List<ServerSession>();
        private object m_lock = new object();

        public void SendForEach()
        {
            lock (m_lock)
            {
                foreach(ServerSession session in m_listSession)
                {
                   
                }
            }
        }

        public ServerSession Generate()
        {
            lock (m_lock)
            {
                ServerSession session = new ServerSession();
                m_listSession.Add(session);
                return session;
            }
        }



    }
}
