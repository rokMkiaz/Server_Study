using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Lock
    {

        ManualResetEvent _available = new ManualResetEvent(true);//아무나 들어오는상태:true, 누구도 못들어오는 :false

        public void Acquire()
        {
            _available.WaitOne();//입장시도
        }

        public void Release()
        {
            _available.Set();//이벤트 상태를  true로 바꿔준다
        }//커널자체가 너무 오래걸려서 시간이 오래걸리는 것이다.
    }
    class Porgram
    {
        static int _num = 0;

        static Lock _lock = new Lock();
        static Mutex __lock = new Mutex();

        static void Thread_1()
        {
            for (int i = 0; i < 1000000; i++)
            {
                //_lock.Acquire();
                __lock.WaitOne();
                
                _num++;
                //_lock.Release();
                __lock.ReleaseMutex();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                //_lock.Acquire();
                __lock.WaitOne();
                _num--;
                //_lock.Release();
                __lock.ReleaseMutex();
            }
        }

        static void Main(string[] args)
        {

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();

            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }



    }

}