using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class SessionManager
    {
        static SessionManager m_Instance = new SessionManager();
        public static SessionManager Instance { get { return m_Instance; } }

        private int m_iSessionID = 0;
        private Dictionary<int, ClientSession> m_hashSession = new Dictionary<int, ClientSession>();
        private object m_lock = new object();   

        public ClientSession Generate()
        {
            lock (m_lock)
            {
                int iSessionID = ++m_iSessionID;
                ClientSession refSession = new ClientSession();
                refSession.m_iSessionID = iSessionID;
                m_hashSession.Add(iSessionID, refSession);

                Console.WriteLine($"Connected : {m_iSessionID}");
                return refSession;
            }
        }

        public ClientSession Find(int _iID)
        {
            lock (m_lock)
            {
                ClientSession refSession = null;
                m_hashSession.TryGetValue(_iID, out refSession);
                return refSession;

            }
        }


        public void Remove(ClientSession _refSession)
        {
            lock (m_lock)
            {
                m_hashSession.Remove(m_iSessionID);
            }
        }

    }
}
