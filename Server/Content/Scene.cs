using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Content;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        //나중에 object로 변경 or 몬스터, 사물 따로 hash로 가지기
        private List<Dictionary<int, GameObject>> m_listObject = new List<Dictionary<int, GameObject>>();
        private List<int> m_listObjectID = new List<int>();

        //Map Data 읽어오기
        private Map m_refMap = new Map();
        public void Init(string _strMapDataPath)
        {
            string[] lines = File.ReadAllLines(_strMapDataPath);
            m_refMap.Init(_strMapDataPath);

            for(int i = 0; i<(int)ObjectType.Monsterattack + 1; ++i)
            {
                m_listObject.Add(new Dictionary<int, GameObject>());
                m_listObjectID.Add(0);
            }
        }

        public void Update()
        {
            foreach (Dictionary<int, GameObject> hashObj in m_listObject)
            {
                foreach (GameObject refObj in hashObj.Values)
                    refObj.Update();
            }
        }

        public void LateUpdate()
        {
            foreach (Dictionary<int, GameObject> hashObj in m_listObject)
            {
                foreach (GameObject refObj in hashObj.Values)
                    refObj.LateUpdate();
            }
        }

        public void BroadCast(IMessage _IMessage)
        {
            Dictionary<int, GameObject> m_hashPlayer = m_listObject[(int)ObjectType.Player];
            foreach(Player refPlayer in m_hashPlayer.Values)
                refPlayer.Session.Send(_IMessage);
        }

        public void BroadCast(IMessage _IMessage, GameObject _refException)
        {
            Dictionary<int, GameObject> m_hashPlayer = m_listObject[(int)ObjectType.Player];
            foreach (Player refPlayer in m_hashPlayer.Values)
            {
                if(_refException != refPlayer)
                    refPlayer.Session.Send(_IMessage);
            }
        }

        public void EnterGame(Player refPlayer)
        {
            JobTimer.m_Instance.Push((Action)(() =>
            {
                //lock (m_lock)
                //{

                if (refPlayer == null)
                    return;

                Dictionary<int, GameObject> refHashObject = m_listObject[(int)ObjectType.Player];
                //현재 씬에 있는 오브젝트 ID
                int ObjID = m_listObjectID[(int)ObjectType.Player];
                refHashObject.Add(ObjID, refPlayer);
                refPlayer.SetObjectID(ObjID);
                ++m_listObjectID[(int)ObjectType.Player];

                //새로 들어온 플레이어에게 내 정보와 해당 씬에 있는 플레이어 목록을 전달
                {
                    S_EnterGame pkt = new S_EnterGame();
                    pkt.PlayerId = refPlayer.PlayerID;
                    refPlayer.Session.Send(pkt);


                    S_Spawn otherPkt = new S_Spawn();
                    foreach (GameObject _refOther in refHashObject.Values)
                    {
                        if (_refOther == refPlayer)
                            continue;

                        otherPkt.Objects.Add(_refOther.ObjectInfo);
                    }
                    refPlayer.Session.Send(otherPkt);
                }

                //기존에 방에 있는 플레이어들에게도 새로 들어온 플레이어 전달
                {
                    S_Spawn pkt = new S_Spawn();
                    pkt.Objects.Add(refPlayer.ObjectInfo);
                    foreach (GameObject refOther in refHashObject.Values)
                    {
                        if (refOther == refPlayer)
                            continue;

                        Player refOtherPlayer = refOther as Player;
                        refOtherPlayer.Session.Send(pkt);
                    }
                }
               //}

            }));
        }

        public void LeaveGame(int _iPlayerID)
        {
            GameObject refObject = null;
            Dictionary<int, GameObject> refHashObject = m_listObject[(int)ObjectType.Player];

            if (refHashObject.TryGetValue(_iPlayerID, out refObject) == false)
                return;

            Player refPlayer = refObject as Player;
            refHashObject.Remove(_iPlayerID);
            refPlayer.SetRoom(null);

            //본인에게 전송
            {

            }           
            //타인에게 전송
            {
                S_Despawn pkt = new S_Despawn();
                pkt.PlayerIds.Add(_iPlayerID);
                foreach (KeyValuePair<int, GameObject> refOther in refHashObject)
                {
                    Player refOtherPlayer = refOther.Value as Player;

                    refOtherPlayer.Session.Send(pkt);

                }
            }

        }


        public bool MapCheck(float _X, float _Z)
        {
            return m_refMap.CanGo(_X, _Z);
        }
        public Vector2 ClamToLastVaild(float _x, float _z, float _gx, float _gy, MoveDir _refMove)
        {
            return m_refMap.ClamToLastVaild(_x, _z, _gx, _gy, _refMove);
        }


        public GameObject FindObject(ObjectType _eObjectType, int _iObjectID)
        {
            Dictionary<int, GameObject> m_hashobject = m_listObject[(int)_eObjectType];
            if (m_hashobject.TryGetValue(_iObjectID, out GameObject refObject) == true)
                return refObject;

            return null;
        }


        public void AddGameObject(ObjectType _eObjectType, GameObject _refObj)
        {
            //m_listObject[(int)_eObjectType].Add()
            Dictionary<int, GameObject> refHashObject = m_listObject[(int)_eObjectType];
            int ObjID = m_listObjectID[(int)_eObjectType];
            refHashObject.Add(ObjID, _refObj);
            _refObj.SetObjectID(ObjID);
            ++m_listObjectID[(int)_eObjectType];
        }

        //15/1 틱만
        public void SendPosition()
        {
            foreach(Dictionary<int, GameObject> hashObj in m_listObject)
            {
                foreach(GameObject refObj in hashObj.Values)
                {
                    Transform refTr = refObj.GetComponent<Transform>(ComponentType.Transform);
                    if (refTr != null)
                        refTr.SendPosition();
                }
            }
        }
    }
}
