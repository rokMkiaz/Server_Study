using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Packet
    {
        public ushort size;
        public ushort packetID;
    }


    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint} ");

            //Packet packet = new Packet() { size = 100, packetID = 10 };
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");

            //TODO:: 나중에 자동화
            //ArraySegment<byte> openSegment=SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetID);
            //Array.Copy(buffer,0,openSegment.Array,openSegment.Offset,buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset, buffer2.Length);
            //ArraySegment<byte> sendBuff= SendBufferHelper.Close(buffer.Length + buffer2.Length);//크기

            //Send(sendBuff);

            Thread.Sleep(1000);

            Disconnect();
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset+2);
            Console.WriteLine($"RecvPacketID : {id}, Size{size}");
            
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected :{endPoint}");
        }



        //public override int OnRecv(ArraySegment<byte> buffer)
        //{
        //    string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        //    Console.WriteLine($"[From Client]{recvData}");

        //    return buffer.Count;
        //}

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes:{numOfBytes}");

        }
    }

    class Program
    {
        static Listener _listener = new Listener();

        //static void OnAcceptHandler(Socket clientSocket)//OnConnected로 이동됨.
        //{
        //    try//성공여부 판단
        //    {
        //        GameSession session = new GameSession();
        //        session.Init(clientSocket);

        //        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");

        //        session.Send(sendBuff);

        //        Thread.Sleep(1000);

        //        session.Disconnect();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
        static void Main(string[] args)
        {
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry iphost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iphost.AddressList[0]; //하나 혹은 여러개의 주소를 받을 수 있음 DNS서버가 해주는것
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); //포트 번호



            _listener.init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            //서버 오픈 대기
            while (true)
            {


            }


        }
    }
}