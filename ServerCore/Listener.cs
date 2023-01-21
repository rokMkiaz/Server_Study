using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler;

        public void init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;


            //식별자 번호 부여
            _listenSocket.Bind(endPoint);//Bind

            //시작
            _listenSocket.Listen(10); //최대 대기수


            SocketAsyncEventArgs args= new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);//Call Back 함수
            RegisterAccept(args); //최초 호출
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args); //비동기 함수

            if(pending == false)  //true면 통보되게
            {
                OnAcceptCompleted(null,args); //멀티스레드에서 해당 작업이 작동한다, 그러므로 위험하다
            }
        }
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success) 
            {
                //TODO::유저가 왔을 때 처리
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args);//다시 호출
        }

        //public Socket Accept() //Blocking
        //{
        //
        //    //접속
        //    return _listenSocket.Accept();
        //}

    }
}
