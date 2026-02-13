using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Scene
    {
        object m_lock = new object();
        private int m_iSceneID;
        public int SceneID { get { return m_iSceneID; } }
        public void SetSceneID(int _iSceneID) { m_iSceneID = _iSceneID; }

        private Dictionary<int, Player> m_hashPlayer = new Dictionary<int, Player>();
        private Queue<int> m_queuePlayerId = new Queue<int>();
        private JobQueue m_refJob = new JobQueue();

        public void Init()
        {
            for (int i = 0; i < 50; ++i)
                m_queuePlayerId.Enqueue(i);
        }
        public void EnterGame(Player _refPlayer)
        {
            JobTimer.m_Instance.Push(() =>
            {
               //lock (m_lock)
               //{
                if (m_queuePlayerId.Count == 0)
                    return;
                
                //플레이어 ID 발급
                int iID = m_queuePlayerId.Dequeue();
                if (_refPlayer == null)
                    return;
                
                m_hashPlayer.Add(iID, _refPlayer);
                _refPlayer.SetPlayerID(iID);

                //새로 들어온 플레이어에게 내 정보와 해당 씬에 있는 플레이어 목록을 전달
                {
                    S_EnterGame pkt = new S_EnterGame();
                    pkt.Player = _refPlayer.PlayerInfo;
                    _refPlayer.Session.Send(pkt);

                    S_Spawn otherPkt = new S_Spawn();
                    foreach (KeyValuePair<int, Player> _refOther in m_hashPlayer)
                    {
                        if (_refOther.Value == _refPlayer)
                            continue;
                        otherPkt.Players.Add(_refOther.Value.PlayerInfo);
                    }
                    _refPlayer.Session.Send(otherPkt);
                }

                //기존에 방에 있는 플레이어들에게도 새로 들어온 플레이어 전달
                {
                    S_Spawn pkt = new S_Spawn();
                    pkt.Players.Add(_refPlayer.PlayerInfo);
                    foreach (KeyValuePair<int, Player> _refOther in m_hashPlayer)
                    {
                        if (_refOther.Value == _refPlayer)
                            continue;
                        _refOther.Value.Session.Send(pkt);
                    }
                }
               //}

            });
        }

        public void LeaveGame(int _iPlayerID)
        {
            Player refPlayer = null;
            if(m_hashPlayer.TryGetValue(_iPlayerID, out refPlayer) == false)
                return;

            m_hashPlayer.Remove(_iPlayerID);
            refPlayer.SetRoom(null);

            //본인에게 전송
            {

            }           
            //타인에게 전송
            {
                S_Despawn pkt = new S_Despawn();
                pkt.PlayerIds.Add(_iPlayerID);
                foreach (KeyValuePair<int, Player> _refOther in m_hashPlayer)
                    _refOther.Value.Session.Send(pkt);
            }

        }
    }
}
