using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ServerCore
{

    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        object _lock = new object();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recevArgs = new SocketAsyncEventArgs();

        //새로운 클래스로 만들어도됨.
        public abstract void OnConnected(EndPoint endPoint);//클라이언트가 접속한 시점
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint); 
        public void Init(Socket socket)
        {
            _socket = socket;

            _recevArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            //recevArgs.UserToken 추가적으로 연결하고싶은게 있을 경우
            _recevArgs.SetBuffer(new byte[1024],0,1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff) //다음에는 패킷
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
                byte[] buff = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buff,0,buff.Length));//struct는 스택영역
           
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
            bool pending=_socket.ReceiveAsync(_recevArgs);

            if(pending == false)
            {
                OnRecvCompleted(null, _recevArgs);
            }
        }
        void OnRecvCompleted(object sender,SocketAsyncEventArgs args)
        {
        
            if(args.BytesTransferred>0 && args.SocketError==SocketError.Success) 
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    //string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    //Console.WriteLine($"[From Client]{recvData}");
                    //메시지 받았다는거 남겨줘야함.

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
