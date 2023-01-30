using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    

    class ClientSession : PacketSession
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
            ushort count = 0;
         
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset+count);
            count += sizeof(ushort);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);
            //switch((PacketID)id)
            //{
            //    case PacketID.PlayerInfoReq:
            //        {
            //            PlayerInfoReq p = new PlayerInfoReq();
            //            p.Read(buffer);
            //            Console.WriteLine($"Player InfoReq : {p.playerID} , {p.name}");
                   
            //            foreach(PlayerInfoReq.Skill_Info skill in p.skills)
            //            {
            //                Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
            //            }
            //        }
            //        break;
            //}
            
            Console.WriteLine($"RecvPacketID : {id}, Size : {size}");

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
 
}
