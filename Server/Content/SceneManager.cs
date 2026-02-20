using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SceneManager
    {
        public static SceneManager m_Instance { get;} = new SceneManager();

        //private object m_lock = new object(); 단일 스레드에서 처리
        Dictionary<int, Scene> m_hashScene = new Dictionary<int, Scene>();

        private int m_iSceneID = 0;
        public Scene Add(string _strMapDataPath)
        {
            Scene refScene = new Scene();
            refScene.Init(_strMapDataPath);

         
            refScene.SetSceneID(m_iSceneID);
            m_hashScene.Add(m_iSceneID, refScene);
            ++m_iSceneID;
            
            return refScene;
        }

        public bool Remove(int _iSceneID)
        {
            return m_hashScene.Remove(m_iSceneID);
        }

        public Scene Find(SceneType _eSceneID)
        {
              if(m_hashScene.TryGetValue((int)_eSceneID, out var refScene) == true)
                  return refScene;

              return null;
        }
        
        public GameObject FindObject(SceneType _eSceneID, int _iObjectID)
        {
             //if (m_hashScene.TryGetValue((int)_eSceneID, out var refScene) == true)
             //{
             //}
             return null;
        }

        //단일 스레드
        public void BroadCast(SceneType _eSceneID, IMessage _IMessage)
        {
            if (m_hashScene.TryGetValue((int)_eSceneID, out var refScene) == true)
                refScene.BroadCast(_IMessage);
        }


        //public Scene Find(int _iSceneID, )
        //{
        //    lock (m_lock)
        //    {
        //        if (m_hashScene.TryGetValue(_iSceneID, out var _refScene) == true)
        //            return _refScene;
        //
        //        return null;
        //    }
        //}


    }
}
