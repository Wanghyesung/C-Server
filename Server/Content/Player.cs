using Google.Protobuf.Protocol;
using Server.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Player : GameObject
    {
        public void SetPlayerInfo(ObjectInfo _refPlayerInfo) 
        { 
            m_refObjectInfo = _refPlayerInfo; 
            m_refPosition = m_refObjectInfo.PosInfo;
            m_refPosition.MoveDir = new MoveDir();
        }

        private int m_iPlayerID;
        private Scene m_refScene = null;
        private ClientSession m_refSession = null;
        public ClientSession Session {  get { return m_refSession; } }
        public int PlayerID { get { return m_iPlayerID; } }
        public void SetPlayerID(int _iID) { m_iPlayerID = _iID; }
        public int GetPlayerID() { return m_iPlayerID; }
        public void SetRoom(Scene _refScene) { m_refScene = _refScene; }
        public void SetSession(ClientSession _refSession) { m_refSession = _refSession; }


        override public void UpdateAI()
        {

        }

    }
}
