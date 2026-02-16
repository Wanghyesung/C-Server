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

        private object m_lock = new object();
        Dictionary<int, Scene> m_hashScene = new Dictionary<int, Scene>();

        private int m_iSceneID = 0;
        public Scene Add(string _strMapDataPath)
        {
            Scene refScene = new Scene();
            refScene.Init(_strMapDataPath);

            lock (m_lock)
            {
                refScene.SetSceneID(m_iSceneID);
                m_hashScene.Add(m_iSceneID, refScene);
                ++m_iSceneID;
            }
            return refScene;
        }

        public bool Remove(int _iSceneID)
        {
            lock (m_lock)
            {
                return m_hashScene.Remove(m_iSceneID);
            }
        }

        public Scene Find(SceneType _eSceneID)
        {
            lock(m_lock)
            {
                if(m_hashScene.TryGetValue((int)_eSceneID, out var refScene) == true)
                    return refScene;

                return null;
            }
        }
        
        public GameObject FindObject(SceneType _eSceneID, int _iObjectID)
        {
            lock (m_lock)
            {
                if (m_hashScene.TryGetValue((int)_eSceneID, out var refScene) == true)
                {
                    int iPlayerID = refScene.SceneID;
                }

                return null;
            }
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
