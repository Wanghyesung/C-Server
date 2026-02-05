using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using System.Threading.Tasks;

namespace Server
{
    
    public class ClientSession : PacketSession
    {
        public int m_iSessionID {  get; set; }
        public GameRoom Room { get; set; }
        
        public override void OnConnected(EndPoint _refEndPoint)
        {
            Porgram.Room.Enter(this);
        }

        public override void OnDisConnected(EndPoint _refEndPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room!= null)
            {
                Room.Leave(this);
                Room = null;
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> _arrBuffer)
        {
            PacketManager.Instance.OnRecvPakcet(this, _arrBuffer);
        }

        public override void OnSend(int _iBytes)
        {
        }
    }
}
