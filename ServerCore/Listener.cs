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
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;


            //식별자 번호 부여
            _listenSocket.Bind(endPoint);//Bind

            //시작
            _listenSocket.Listen(backlog); //최대 대기수

            for (int i = 0; i < register; ++i)
            {
                SocketAsyncEventArgs args = new();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }

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
                Session session = _sessionFactory.Invoke();//밖에서 만들어주는게 좋음.
                session.Init(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                //_onAcceptHandler.Invoke(args.AcceptSocket);
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
