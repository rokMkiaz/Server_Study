using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Porgram
    {
        static bool _stop = false; //volatile ==휘발성 데이터 선언이므로 최적화 금지, 웬만하면 쓰지마라

        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작!");

            while(_stop==false)
            {

            }
            Console.WriteLine("쓰레드 종료");

            //기계어에서 최적화중에 문제가 발생할 수 있다.
            //if(_stop ==false)
            //{
            //    while(true)
            //    {
            //
            //    }
            //}

        }
        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");
            t.Wait();


            Console.WriteLine("종료 성공");

        }
    }

}