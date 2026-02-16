using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PlayerManager
    {
        public static PlayerManager m_Instance { get; } = new PlayerManager();

        private object m_lock = new object();
        Dictionary<int, Player> m_hashPlayer = new Dictionary<int, Player>();
        private Queue<int> m_queuePlayerId = new Queue<int>();

        private int m_iSceneID = 0;


        public void Init()
        {
            for (int i = 0; i < 50; ++i)
                m_queuePlayerId.Enqueue(i);
        }

        public Player AddPlayer()
        {
            Player refPlayer = new Player();
            lock (m_lock)
            {
                if (m_queuePlayerId.Count == 0)
                    return null;

                //플레이어 ID 발급
                int iID = m_queuePlayerId.Dequeue();
                
                
                ObjectInfo refPlayerInfo = new ObjectInfo();
                refPlayerInfo.PosInfo = new PositionInfo();
                refPlayerInfo.PosInfo.PosX = 0;
                refPlayerInfo.PosInfo.PosY = 0;
                refPlayerInfo.Name = $"Player";

                refPlayer.SetPlayerID(iID);
                refPlayer.SetPlayerInfo(refPlayerInfo);
            }
            return refPlayer;
        }

        public bool Remove(int _iPlayerID)
        {
            lock (m_lock)
            {
                m_queuePlayerId.Enqueue(_iPlayerID);
                return m_hashPlayer.Remove(_iPlayerID);
            }
        }

        public Player FindPlayer(int _iPlayerID)
        {
            lock (m_lock)
            {
                if (m_hashPlayer.TryGetValue(_iPlayerID, out var refPlayer) == true)
                    return refPlayer;

                return null;
            }
        }

    }
}
