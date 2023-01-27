using System;
using System.Net.Http.Headers;
using System.Threading;


namespace ServerCore
{
    class R_Program
    {
        static void MainThread(object state)
        {
            for(int i=0; i<5; i++)
            Console.WriteLine("HelloThread");
        }


        static void Main(string[] args)
        {


            ThreadPool.SetMinThreads(1, 1);//workerT=쓰래드 갯수, IO관련된 이밴트 대기하는 갯수
            ThreadPool.SetMaxThreads(5,5);//5개의 풀로 구성

            for(int i=0; i<5;i++) //5 사용시 먹통상황 
            {
                Task t=new Task(() => { while (true) { } },TaskCreationOptions.LongRunning); //처리 단위를 설정해서 사용하겠다. TaskCreationOptions.LongRunning=긹게처리할 별도옵션설정
                t.Start();
            }

            //for (int i = 0; i < 4; i++) //5 사용시 먹통상황 
            //{
            //     ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });  //C#람다식
            //    
            //}

            ThreadPool.QueueUserWorkItem(MainThread);//짧은 시스템에 적당

            //쓰지말 것
            //for (int i = 0; i < 1000; i++)
            //{
            //    Thread t = new Thread(MainThread); //new 는 부담이 큼
            //    //t.Name = "TestThread";
            //    t.IsBackground = true; //프론트 , 백 작동 여부 선택
            //    t.Start();
            //}
           //Console.WriteLine("Wait for Thread!");

           //t.Join();
           //Console.WriteLine("Hello World");
            while (true)
            { 
                 
            }
            
            
        }
    }

}