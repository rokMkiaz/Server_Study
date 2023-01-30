using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace DummyClient
{
   
    class ServerSession : Session
    {


        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected :{endPoint}");

            //PlayerInfoReq packet = new PlayerInfoReq() { playerID = 1001,name="ABCD" };
            //packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 101, level = 1, duration = 3.2f });
            //packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 201, level = 2, duration = 1.1f });
            //packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 301, level = 3, duration = 5.3f });
            //packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 401, level = 4, duration = 10.1f });
            
            //보낸다
            //for (int i = 0; i < 5; i++)
            //{
            //    ArraySegment<byte> openSegment = packet.Write();
           
            //    if(openSegment!=null)
            //        Send(openSegment);

            //}
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected :{endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server]{recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes:{numOfBytes}");

        }
    }
 
}
