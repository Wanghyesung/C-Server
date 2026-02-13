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
        public Scene Add()
        {
            Scene refScene = new Scene();

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

        public Scene Find(int _iSceneID)
        {
            lock(m_lock)
            {
                if(m_hashScene.TryGetValue(_iSceneID, out var _refScene) == true)
                    return _refScene;

                return null;
            }
        }
    }
}
