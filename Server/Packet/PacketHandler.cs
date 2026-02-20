using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession _refSession, IMessage _iPacket)
    {
        ClientSession refSession = _refSession as ClientSession;
        C_Move pkt = _iPacket as C_Move;

        
        JobTimer.m_Instance.Push(() =>
        {
            //해당 클라소켓과 연결된 플레이어
            Player refPlayer = refSession.m_refPlayer;
            Scene refScene = SceneManager.m_Instance.Find(refPlayer.SceneType);
           
            S_Move sPkt = new S_Move();
            //S_Other_Move sOtherPkt = new S_Other_Move();
            //sOtherPkt.Success = true;

            //[16비트 플레이어 ID][16비트 월드 오브젝트 ID] 
            sPkt.ObjectId = (refPlayer.PlayerID << 16) | refPlayer.ObjectID;

            Vector3 vPosition = new Vector3(pkt.PosInfo.PosX, pkt.PosInfo.PosY, pkt.PosInfo.PosZ);
            MoveDir refPlayerDir = pkt.PosInfo.MoveDir;
            //if (refScene.MapCheck(vPosition.X, vPosition.Z) == false)
            //{
            //    sOtherPkt.Success = false;
            //    Vector2 vClampPos = refScene.ClamToLastVaild(refPosition.PosX, refPosition.PosZ, pkt.PosInfo.PosX, pkt.PosInfo.PosZ, refPlayer.Position.MoveDir);
            //    vPosition.X = vClampPos.X;
            //    vPosition.Z = vClampPos.Y;
            //}
            Transform refTr = refPlayer.GetComponent<Transform>(ComponentType.Transform);
            refTr.SetPosition(vPosition);
            refTr.SetMoveDir(refPlayerDir.DirX, refPlayerDir.DirY, refPlayerDir.DirZ);
         
            //sOtherPkt.PosInfo = sPkt.PosInfo;

            //refScene.BroadCast(sPkt, refPlayer);
            //refSession.Send(sOtherPkt);
        });
    }
}

