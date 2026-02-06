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

        public void Broadcast(ArraySegment<byte> _arrSeg)
        {
            lock( m_lock)
            {
                foreach (ClientSession session in m_listSession)
                    session.Send(_arrSeg);
            }
        }

        public void Enter(ClientSession _refSession)
        {
            lock (m_lock)
            {
                _refSession.Room = this;
                m_listSession.Add(_refSession);

                //신입생한테 모든 플레이어 목록 전송
                S_PlayerList players = new S_PlayerList();
                foreach (ClientSession session in m_listSession)
                {
                    players.players.Add(new S_PlayerList.Player()
                    {
                        isSelf = (_refSession == session),
                        playerID = session.m_iSessionID,
                        posX = session.m_PosX,
                        posY = session.m_PosY,
                        posZ = session.m_PosZ,
                    });
                }
                _refSession.Send(players.Write());

                //신입생 입장을 모두에게 알린다
                S_BoradcastEnterGame enter = new S_BoradcastEnterGame();
                enter.playerID = _refSession.m_iSessionID;
                enter.posX = 0;
                enter.posY = 0;
                enter.posZ = 0;
                Broadcast(enter.Write());
            }
        }

        public void Leave(ClientSession _refSession)
        {
            lock (m_lock)
            {
                m_listSession.Remove(_refSession);


                S_BroadcastLeveGame leave = new S_BroadcastLeveGame();
                leave.playerID = _refSession.m_iSessionID;
                Broadcast(leave.Write());
            } 
        }

        public void Move(ClientSession _refSession, C_Move _pkt)
        {
            _refSession.m_PosX = _pkt.posX;
            _refSession.m_PosY = _pkt.posY;
            _refSession.m_PosZ = _pkt.posZ;

            S_BroadcastMove move = new S_BroadcastMove();
            move.playerID = _refSession.m_iSessionID;
            move.posX = _pkt.posX;
            move.posY = _pkt.posY;
            move.posZ = _pkt.posZ;
            Broadcast(move.Write());
        }
    }
}
