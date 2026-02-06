using ServerCore;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class Porgram
    {
 
        static void Main(string[] args)
        {
           
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector refConnector = new Connector();

            refConnector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); },10);

            Thread.Sleep(1000);

            while (true)
            {
                try
                {

                }
                catch(Exception e) 
                {
                    Console.WriteLine(e.ToString());    
                }

                Thread.Sleep(250);
            }
        }
    }
}