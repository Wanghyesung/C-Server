using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Transform : Component
    {
        private S_Move m_pkt = new S_Move();
        private PositionInfo m_refPosition = null;
        private MoveDir m_refMoveDir = null;
        private Vector3 m_vPosition;
        private Vector3 m_vPrevPosition;

        public override void Init(GameObject _refOwner)
        {
            base.Init(_refOwner);
            m_refPosition = new PositionInfo();
            m_refMoveDir = new MoveDir();
            m_pkt.PosInfo = m_refPosition;
            m_refPosition.MoveDir = m_refMoveDir;

            m_vPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public void SetPosition(float x, float y, float z) 
        {
            m_vPrevPosition = m_vPosition;

            m_vPosition.X = x;
            m_vPosition.Y = y;
            m_vPosition.Z = z;
        }
            
        public void SetPosition(in Vector3 _vPos) 
        {
            m_vPrevPosition = m_vPosition;

            m_vPosition.X = _vPos.X;
            m_vPosition.Y = _vPos.Y;
            m_vPosition.Z = _vPos.Z;
        }

        public void SetMoveDir(float x, float y, float z)
        {
            m_refMoveDir.DirX = x;
            m_refMoveDir.DirY = y;
            m_refMoveDir.DirZ = z;
        }
        public void SetMoveDir(in Vector3 _vPos)
        {
            m_refMoveDir.DirX = _vPos.X;
            m_refMoveDir.DirY = _vPos.Y;
            m_refMoveDir.DirZ = _vPos.Z;
        }

        public Vector3 GetPosition() { return m_vPosition; }

        public override void Update()
        {
            //맵 체크
            Scene refScene = SceneManager.m_Instance.Find(m_refOwner.SceneType);
            if (refScene.MapCheck(m_refPosition.PosX, m_refPosition.PosZ) == false)
                refScene.ClamToLastVaild(m_vPrevPosition.X, m_vPrevPosition.Z, m_vPosition.X, m_vPosition.Z, m_refPosition.MoveDir);
        }

        public override void LateUpdate()
        {

        }


        public void SendPosition()
        {
            m_refPosition.PosX = m_vPosition.X;
            m_refPosition.PosY = m_vPosition.Y;
            m_refPosition.PosZ = m_vPosition.Z;

            SceneManager.m_Instance.BroadCast(m_refOwner.SceneType, m_pkt);
        }

        
    }
}
