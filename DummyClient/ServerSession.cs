﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetID;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerID;
        public string name;

        public struct Skill_Info
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte>s , ref ushort count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count),level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);
                return success;
            }

            public void Read(ReadOnlySpan<byte>s,ref ushort count)
            {
                id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }

        public List<Skill_Info> skills = new List<Skill_Info>();
       

        public PlayerInfoReq()
        {
  
            this.packetID = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);


            count += sizeof(ushort); //size
            count += sizeof(ushort); //packet id

            this.playerID = BitConverter.ToInt64(s.Slice(count, s.Length - count)); //몇바이트인지 검사
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;

            //skill list
            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count+=sizeof(ushort);
            for(int i=0; i<skillLen; i++)
            {
                Skill_Info skill = new Skill_Info();
                skill.Read(s,ref count);
                skills.Add(skill);
            }

        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            bool success = true;
            ushort count = 0;

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetID);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
            count += sizeof(long);



            //string UTF 16
            //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            //success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count),nameLen);
            //count += sizeof(ushort);
            //Encoding.Unicode.GetBytes(this.name);
            //Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLen);
            //count += nameLen;

            //효율이 더 좋음
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count+sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;

            //skill list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            foreach(Skill_Info skill in skills)
            {
                success &= skill.Write(s, ref count);
            }


            //최종 크기
            this.size = count;//size는 전체적인 메모리의 크기
            success &= BitConverter.TryWriteBytes(s, this.size);

            if (success == false) return null;

            return SendBufferHelper.Close(count);//크기
        }
    }
    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;

        public override void Read(ArraySegment<byte> s)
        {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> Write()
        {
            throw new NotImplementedException();
        }
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

            PlayerInfoReq packet = new PlayerInfoReq() { playerID = 1001,name="ABCD" };
            packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 101, level = 1, duration = 3.2f });
            packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 201, level = 2, duration = 1.1f });
            packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 301, level = 3, duration = 5.3f });
            packet.skills.Add(new PlayerInfoReq.Skill_Info() { id = 401, level = 4, duration = 10.1f });
            
            //보낸다
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> openSegment = packet.Write();
           
                if(openSegment!=null)
                    Send(openSegment);

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
