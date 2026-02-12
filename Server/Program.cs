using ServerCore;
using System;
using System.Net;
using System.Text;
using Google.Protobuf.Protocol;

//content
namespace Server
{
    //game content
   
    class Porgram
    {
        static private Listener m_Listener = new Listener();
        //public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            //Room.Push(() => Room.Flush());
            //현재 예약된 일감을 실행하고 다음 일정을 예약
            JobTimer.m_Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            m_Listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); },5);

            JobTimer.m_Instance.Push(FlushRoom);

            while(true)
            {
                JobTimer.m_Instance.Flush();
            }
        }
    }
}