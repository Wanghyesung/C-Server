using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameRoom
    {
        private List<ClientSession> m_listSession = new List<ClientSession>();
        private object m_lock = new object();

        public void Broadcast(ClientSession _refSession, string strChat)
        {
            S_Chat packet = new S_Chat();
            packet.playerID = _refSession.m_iSessionID;
            packet.chat = $"{strChat} i am {packet.playerID}";
            ArraySegment<byte> segment = packet.Write();

            lock( m_lock)
            {
                foreach (ClientSession session in m_listSession)
                    session.Send(segment);
            }
        }

        public void Enter(ClientSession _refSession)
        {
            lock (m_lock)
            {
                _refSession.Room = this;
                m_listSession.Add(_refSession);
            }
        }

        public void Leave(ClientSession _refSession)
        {
            lock (m_lock)
            {
                m_listSession.Remove(_refSession);
            }
            
        }
    }
}
