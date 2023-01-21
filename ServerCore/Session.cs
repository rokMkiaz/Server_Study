using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


namespace ServerCore
{
    internal class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Init(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recevArgs= new SocketAsyncEventArgs();
            recevArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            //recevArgs.UserToken 추가적으로 연결하고싶은게 있을 경우
            recevArgs.SetBuffer(new byte[1024],0,1024);

            RegisterRecv(recevArgs);
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }

        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending=_socket.ReceiveAsync(args);

            if(pending == false)
            {
                OnRecvCompleted(null, args);
            }
        }
        public void Disconnect()
        {
            if(Interlocked.Exchange(ref _disconnected, 1)==1);
            {
                return;
            }
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();    
        }
        void OnRecvCompleted(object sender,SocketAsyncEventArgs args)
        {
        
            if(args.BytesTransferred>0 && args.SocketError==SocketError.Success) 
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client]{recvData}");
                    RegisterRecv(args);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                //TODO :: Disconnnect
            }
        }
    }
}
