using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static Listener _listener= new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try//성공여부 판단
            {
                Session session = new Session();
                session.Init(clientSocket);
 
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");

                session.Send(sendBuff);

                Thread.Sleep(1000);

                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        static void Main(string[] args)
        {
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry iphost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iphost.AddressList[0]; //하나 혹은 여러개의 주소를 받을 수 있음 DNS서버가 해주는것
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); //포트 번호



            _listener.init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            //서버 오픈 대기
            while (true)
            {


            }
        

           
            
          
        }
    }
}
