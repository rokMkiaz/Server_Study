using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{

    class Porgram
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My Name Is {Thread.CurrentThread.ManagedThreadId}"; }); //전역변수인데, 쓰레드마다 고유의 공간사용
        //static string ThreadName; //같은 공간사용
       
        static void WhoAmI()
        {
            bool repeat = ThreadName.IsValueCreated;//만들어져있으면 true

            if(repeat)
            {
                Console.WriteLine(ThreadName.Value+"(repeat)");


            }
            else
            {
                Console.WriteLine(ThreadName.Value);
            }
            //ThreadName.Value = $"My Name Is {Thread.CurrentThread.ManagedThreadId}";
            //ThreadName = $"My Name Is {Thread.CurrentThread.ManagedThreadId}";

            //Thread.Sleep(1000);

            //Console.WriteLine(ThreadName);
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);//넣은만큼 Task로 실행


            ThreadName.Dispose();//날리는 것
        }



    }

}