using Google.Protobuf.Protocol;
using Server;
using Server.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ObjectFactory
    {
        static public ObjectFactory m_Instance = new ObjectFactory();
        
        private Dictionary<int, Func<GameObject>> m_hashGameObject = new Dictionary<int, Func<GameObject>>();
        private List<CSVObjectData> m_listData = new List<CSVObjectData>();


        public void Init()
        {
            //csv 파싱 후 GameObjectInfo 생성, 그 후 ID 별로 클래스 생성
            m_listData = ObjectCsvLoader.LoadIdNameTable("C:\\Users\\wangh\\source\\wang\\Server\\Server\\bin\\Debug\\net8.0\\GameData\\ObjectData");
            for(int i = 0; i < m_listData.Count; ++i)
            {
                CSVObjectData refData = m_listData[i];

                ObjectInfo refObjInfo = new ObjectInfo();
                refObjInfo.Name = refData.name;
                refObjInfo.CreateId = refData.id;
            }

        }

        public GameObject GetObject(int _iObj)
        {
            if (m_hashGameObject.TryGetValue(_iObj, out var func) == true)
                return func.Invoke();

            return null;
        }

    }


    class Factory
    {
        public static GameObject CreateFireBallSkill()
        {
            PlayerAttack refPlayerAttack = new PlayerAttack();
            return refPlayerAttack;
        }
    }
}