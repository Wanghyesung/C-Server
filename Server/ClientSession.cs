using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using System.Threading.Tasks;

//나중에 프로젝트에는 내 클라세션을 pplayer에서 가지고 있게 설정
namespace Server
{
    
    public class ClientSession : PacketSession
    {
        public int m_iSessionID {  get; set; }
        public GameRoom Room { get; set; }
        public float m_PosX { get; set; }
        public float m_PosY { get; set; }
        public float m_PosZ { get; set; }
       
        public override void OnConnected(EndPoint _refEndPoint)
        {
            Porgram.Room.Push(() => { Porgram.Room.Enter(this); });
            
        }

        public override void OnDisConnected(EndPoint _refEndPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room!= null)
            {
                //다른 스레드에서 처리하기 전에 null로 가기 때문에 참조해서 가지고 있다가 해당 스택의 변수를 통해서 leave
                GameRoom refRoom = Room;
                refRoom.Push(() => { refRoom.Leave(this); });
                Room = null;
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> _arrBuffer)
        {
            PacketManager.Instance.OnRecvPacket(this, _arrBuffer,null);
        }

        public override void OnSend(int _iBytes)
        {
        }
    }
}
