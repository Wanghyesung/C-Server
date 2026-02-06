using ServerCore;
using System;
using System.Net;
using System.Text;

//content
namespace Server
{
    //game content
   
    class Porgram
    {
        static private Listener m_Listener = new Listener();
        public static GameRoom Room = new GameRoom();
        static void Main(string[] args)
        {
           
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            m_Listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); },10);

            while(true)
            {
                //내가 모은 패킷을 처리
                Room.Push( ()=> Room.Flush());
                Thread.Sleep(250);
            }
        }
    }
}