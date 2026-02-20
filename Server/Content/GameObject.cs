using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameObject
    {
        protected Component[] m_arrComponent = new Component[(int)ComponentType.End];

        protected ObjectInfo m_refObjectInfo = null;
        public ObjectInfo ObjectInfo { get { return m_refObjectInfo; } }

        protected int m_iObjetID = 0;
        public int ObjectID { get {  return m_iObjetID; }  }

        protected ObjectType m_eObjectType = ObjectType.Default;
        public ObjectType ObjectType { get { return m_eObjectType; } }


        protected SceneType m_eSceneType = SceneType.Town;
        public SceneType SceneType { get { return m_eSceneType; } }

        virtual public int GetCreateID() { return -1; }
       

        virtual public void Init(ObjectInfo _refObjectInfo)
        {
           
        }

        virtual public void Update() { }
        virtual public void LateUpdate() { }

        public void SetObjectID(int _iObjectID) { m_iObjetID = _iObjectID; }


        void AddComponent<T>() where T : Component, new()
        {
            T com = new T();
            com.Init(this);
            m_arrComponent[(int)com.ComponentType] = com;
        }

        public T GetComponent<T>(ComponentType componentType) where T : Component
        {
            Component com = m_arrComponent[(int)componentType];
            if (com == null)
                return null;

            return com as T;
        }


    }
}
