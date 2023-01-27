using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetID;


    }

    class PlayerInfoReq : Packet
    {
        public long playerID;
    }
    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq=1,
        PlayerInfoOk=2,
    }

    class ServerSession : Session
    {


        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected :{endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetID = (ushort)PacketID.PlayerInfoReq, playerID = 1001 };
            //보낸다
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                bool success = true;
                ushort count = 0;

                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset+count, openSegment.Count-count), packet.packetID);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset + count, openSegment.Count - count), packet.playerID);
                count += 8;

                packet.size = count;//size는 전체적인 메모리의 크기
                success &=BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count), packet.size);
                
                //교체 이전 버전
                //byte[] size = BitConverter.GetBytes(packet.size);
                //byte[] packetId = BitConverter.GetBytes(packet.packetID);
                //byte[] playerId = BitConverter.GetBytes(packet.playerID);

                //Array.Copy(size, 0, openSegment.Array, openSegment.Offset+count, 2);
                //count += (ushort)size.Length;
 
                //Array.Copy(packetId, 0, openSegment.Array, openSegment.Offset+count, 2);
                //count += (ushort)packetId.Length;
        
                //Array.Copy(playerId, 0, openSegment.Array, openSegment.Offset + count,8);
                //count +=(ushort)playerId.Length;
          
           
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);//크기

                


                //byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i}");
                if(success)
                    Send(sendBuff);

            }
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
