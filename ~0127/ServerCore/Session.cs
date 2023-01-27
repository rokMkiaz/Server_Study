using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        // [size(2)][packetId(2)][......][size(2)][packetId(2)][......]
        public sealed override int OnRecv(ArraySegment<byte> buffer) //sealed 다른 외부가 상속받으면 오류나게함
        {
            int processLen = 0;//몇바이트 까지 왔는지 검사
            //int packetCount = 0;

            while (true)
            {
                // 최소한 헤더는 파싱할 수 있는지 확인.
                if (buffer.Count < HeaderSize)
                    break;

                // 패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                // 여기까지 왔으면 패킷 조립 가능.
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                //++packetCount;

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            //if (packetCount > 1)
            //    Console.WriteLine($"패킷 모아보내기 : {packetCount}");

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(4096);

        object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        //새로운 클래스로 만들어도됨.
        public abstract void OnConnected(EndPoint endPoint);//클라이언트가 접속한 시점
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint); 
        public void Init(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            //recevArgs.UserToken 추가적으로 연결하고싶은게 있을 경우
            //_recevArgs.SetBuffer(new byte[1024],0,1024); //전부 안왔을 경우 오프셋 부터 시작=>RegisterRecv()로 이동

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(ArraySegment<byte> sendBuff) //다음에는 패킷
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }
        public void Disconnect()
        {
            if(Interlocked.Exchange(ref _disconnected, 1)==1)
            {
                return;
            }
            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();    
        }

        void RegisterSend( ) //Send를 할때는 다른 방식 필요
        {
            while (_sendQueue.Count>0) //상대방이 비정상적인 정보를 많이 보낼때 제한을 해야함.
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }

            _sendArgs.BufferList = _pendingList;

            bool pending =  _socket.SendAsync(_sendArgs);//만들어 질때 마다 호출하는건 비효율
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {

                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();//

                        OnSend(_sendArgs.BytesTransferred);

                       
                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterRecv()
        {
            _recvBuffer.Clean();
            ArraySegment<byte>segment =_recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array,segment.Offset,segment.Count);

            bool pending=_socket.ReceiveAsync(_recvArgs);

            if(pending == false)
            {
                OnRecvCompleted(null, _recvArgs);
            }
        }
        void OnRecvCompleted(object sender,SocketAsyncEventArgs args)
        {
        
            if(args.BytesTransferred>0 && args.SocketError==SocketError.Success) 
            {
                try
                {
                    //WritePos이동
                    if(_recvBuffer.OnWrite(args.BytesTransferred)==false)
                    {
                        Disconnect();
                        return;
                    }
                    //컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다

             
                    //string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    //Console.WriteLine($"[From Client]{recvData}");
                    //메시지 받았다는거 남겨줘야함.

                    int processLen = OnRecv(_recvBuffer.DataSegment);//ReadSegment<=OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    //ReadPos 이동
                    if(_recvBuffer.OnRead(processLen) ==false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();

                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
    }
}
