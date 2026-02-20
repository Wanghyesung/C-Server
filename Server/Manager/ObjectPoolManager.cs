using Server.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Manager
{
    //메인 스레드에서만 접근하는 영역
    class ObjectPoolManager
    {
        private Dictionary<int, Queue<GameObject>> m_hashGameObject = new Dictionary<int, Queue<GameObject>>();

        public void Init()
        {

        }

        public GameObject GetObject(int _iID)
        {
            if (m_hashGameObject.TryGetValue(_iID, out Queue<GameObject> listObj) == true)
            {
                if(listObj.Count == 0)
                    return ObjectFactory.m_Instance.GetObject(_iID);

                return listObj.Dequeue();
            }
            return null;
        }

                
        public void AddObject(GameObject _refObject)
        {
            int iObjID = _refObject.ObjectID;
            if(m_hashGameObject.TryGetValue(iObjID, out Queue<GameObject> listObj) == true)
                listObj.Enqueue(_refObject);
        }

    }
}
