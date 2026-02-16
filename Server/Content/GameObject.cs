using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Content
{
    public class GameObject
    {
        protected ObjectInfo m_refObjectInfo = null;
        public ObjectInfo ObjectInfo { get { return m_refObjectInfo; } }

        protected PositionInfo m_refPosition;
        public PositionInfo Position { get { return m_refPosition; } }

        protected int m_iObjetID = 0;
        public int ObjectID { get {  return m_iObjetID; }  }

        protected ObjectType m_eObjectType = ObjectType.Default;
        public ObjectType ObjectType { get { return m_eObjectType; } }


        protected SceneType m_eSceneType = SceneType.Town;
        public SceneType SceneType { get { return m_eSceneType; } }

        public void SetPosition(float _x, float _y, float _z)
        {
            m_refPosition.PosX = _x;
            m_refPosition.PosY = _y;
            m_refPosition.PosZ = _z;
        }
        virtual public void UpdateMove()
        {

        }

        virtual public void UpdateAI()
        {

        }

        public void SetObjectID(int _iObjectID) { m_iObjetID = _iObjectID; }


    }
}
