using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Porgram
    {
        class SessionManager //데드락 테스트
        {
            static object _lock = new object();

            public static void TestSession()
            {
                lock( _lock )
                {

                }
            }
            public static void  Test()
            {
                lock (_lock)
                {
                    UserManager.Test();
                }
            }

        }
        class UserManager//데드락 테스트
        {
            static object _lock = new object();
            public static void Test()
            {

                lock(_lock)
                {
                    SessionManager.TestSession();
                }
            }

            public static void TestUser()
            {
                lock (_lock)
                {
                    
                }
            }
        }

        static int num = 0;
        static object obj = new object();  

        static void Thread1()
        {
            //int prev=num;
            for (int i = 0; i < 1000000; i++)
            {
                //상호 배제 Mutual Exclusive
                //Monitor.Enter(obj);

                //lock(obj) //상호배제  ,try{} finaliy
                //{
                //
                //    num++;
                //}

                //    Interlocked.Increment(ref num);//원자적(쪼개질 수 없는 상태)으로 만들어줌-하지만 성능에서 손해를 많이봄.
                //int afterValue = Interlocked.Increment(ref num);//다음 값 추출방법

                //num++;
                //Monitor.Exit(obj);

                SessionManager.Test();
            }
            //int next = num; //주소값으로 접근하기에 다음 값을 알 수 없다.
        }

        static void Thread2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                //Monitor.Enter(obj);
                //num--;
                //Interlocked.Decrement(ref num);
                //Monitor.Exit(obj);

                //lock(obj)
                //{
                //    num--;
                //}

                UserManager.Test();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread1);
            Task t2 = new Task(Thread2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(num);

        }
       
            
       
    }

}