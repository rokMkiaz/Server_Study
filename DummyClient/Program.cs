using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{

    class Program 
    {
        static void Main(string[] args)
        {
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry iphost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iphost.AddressList[0]; //하나 혹은 여러개의 주소를 받을 수 있음 DNS서버가 해주는것
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); //포트 번호=서버와 맞춰줘야함

            Connector connector = new Connector();
            connector.Connect(endPoint,
              () => { return SessionManager.Instance.Generate(); },
              500);
            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    //연결 시도->Connector
                    //socket.Connect(endPoint);
                    //Console.WriteLine($"Connected To{socket.RemoteEndPoint.ToString()}");


                    ////보낸다-> Session으로 이동
                    //for(int i = 0;i < 5; i++)
                    //{
                    //    byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i}");
                    //    int sendBytes = socket.Send(sendBuff);
                    //}


                    ////받는다 -> Session으로 이동
                    //byte[] recBuff = new byte[1024];
                    //int recBytes = socket.Receive(recBuff);
                    //string recData = Encoding.UTF8.GetString(recBuff, 0, recBytes);
                    //Console.WriteLine($"[From Server]{recData}");

                    //나간다
                    //socket.Shutdown(SocketShutdown.Both);
                    //socket.Close();
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(1000);
            }
            //단말기 설정
      


           
        }
    }

}


