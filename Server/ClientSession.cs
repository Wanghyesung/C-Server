using Google.Protobuf;
using Google.Protobuf.Protocol;
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
        public Player m_refPlayer { get; set; }
        public int m_iSessionID {  get; set; }
        //public GameRoom Room { get; set; }
       
        public void Send(IMessage _Ipacket)
        {
            string strMsgName = _Ipacket.Descriptor.Name.Replace("_", string.Empty);
            MsgId eID = (MsgId)Enum.Parse(typeof(MsgId), strMsgName);


            ushort sSize = (ushort)_Ipacket.CalculateSize();
            byte[] arrSendBuffer = new byte[sSize + 4]; //패킷 사이즈, 패킷 아이디
            Array.Copy(BitConverter.GetBytes((sSize + 4)), 0, arrSendBuffer, 0, sizeof(ushort));

            ushort protocolId = (ushort)eID;
            Array.Copy(BitConverter.GetBytes(protocolId), 0, arrSendBuffer, 2, sizeof(ushort));
            Array.Copy(_Ipacket.ToByteArray(), 0, arrSendBuffer, 4, sSize);

            Send(new ArraySegment<byte>(arrSendBuffer));
        }
        public override void OnConnected(EndPoint _refEndPoint)
        {
            //Porgram.Room.Push(() => { Porgram.Room.Enter(this); });
            Console.WriteLine($"Connected");


            m_refPlayer = PlayerManager.m_Instance.AddPlayer();
            if (m_refPlayer == null)
                return;
            

            m_refPlayer.SetSession(this);
            SceneManager.m_Instance.Find(0).EnterGame(m_refPlayer);
            
        }

        public override void OnDisConnected(EndPoint _refEndPoint)
        {
            Console.WriteLine($"DisConnected");
        }

        public override void OnRecvPacket(ArraySegment<byte> _arrBuffer)
        {
            PacketManager.Instance.OnRecvPacket(this, _arrBuffer);
        }

        public override void OnSend(int _iBytes)
        {
        }
    }
}
