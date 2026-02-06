using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //만약 다음 맵으로 넘어가는 게임이라면 씬마다 잡큐를 넣으면 편하게 해결가능
    //만약 룸 개념이 아니라면 사물에 대해서 하나씩 잡큐를 가지고있으면 편하다

    //잡큐를 사용한 이유는 만약 lock을 걸고 바로 해당 패킷에 대한 행동을 취한 경우
    //다른 스레드들은 대기를 할거고 스레드풀에서 해당 스레드들은 돌아오지 않고 새로운 스레드를
    //계속해서 만들어서 받기 때문에 더욱 대기 상활이 길어진다 또한 그 상태에서 샌드버퍼를 사용할 때 
    //버퍼를 쪼개서 만들기 때문에 게속 메모리가 늘어난다(작업이 밀리기 때문)
    // broadcast가 n^2  이걸 게임 로직 중간중간에서 계속 호출 클라 100명 10000
    // lock 대기, 메모리 증가, 프레임 밀림..

    public class GameRoom : IJobQeueu
    {
        private List<ClientSession> m_listSession = new List<ClientSession>();
        private JobQueue m_queueJob = new JobQueue();
        private List<ArraySegment<byte>> m_listPending = new List<ArraySegment<byte>>();
        public void Push(Action _refJob)
        {
            m_queueJob.Push(_refJob);
        }

        //jobqueue안에서 하나의 스레드만 작업하기 때문에 lock X
        public void Flush()
        {
            foreach (ClientSession session in m_listSession)
                session.Send(m_listPending);

            Console.WriteLine($"Flush Count {m_listPending.Count}");
            m_listPending.Clear();
        }

        public void Broadcast(ArraySegment<byte> _arrSeg)
        {
            //해당 패킷을 바로 broadcast하지 않고 패킷을 모아두기
            m_listPending.Add(_arrSeg);   
        }

        public void Enter(ClientSession _refSession)
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

        public void Leave(ClientSession _refSession)
        {
          
                m_listSession.Remove(_refSession);


                S_BroadcastLeveGame leave = new S_BroadcastLeveGame();
                leave.playerID = _refSession.m_iSessionID;
                Broadcast(leave.Write());
            
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
