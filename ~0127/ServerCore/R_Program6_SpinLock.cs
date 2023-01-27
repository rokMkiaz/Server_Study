using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SpinLock
    {
        volatile int _locked = 0;

        public void Acquire()
        {
            while (true)
            {

                //int original = Interlocked.Exchange(ref _locked, 1);
                //if (original == 0)break; //0이면 없는 상태, 1이면 있음
                //잠김이 풀리기를 기다린다

                //CAS Compare-And-Swap
                int expected = 0;//C++에서는 이렇게 표시되어있음.
                int desired = 1;
                int original=Interlocked.CompareExchange(ref _locked, desired, expected);//좀 더 일반적인 상황
                if (original == 0)
                    break; 
            }
            //Thread.Sleep(1);//무조건 휴식 => 무조건 1ms정도 쉬고 시작
            //Thread.Sleep(0); //조건부 양보=> 자신보다 우선순위가 낮은 곳에는 양보 불가=>우선순위가 같거나 높은 쓰래드 가없으면 자신에게 돌아옴
            Thread.Yield(); // 관대한 양보=> 지금 실행 가능한 쓰레드가 있으면 실행=>실행 가능한게 없다면 실행            
        }

        public void Release()
        {
            _locked= 0; 
        }
    }
    class Porgram
    {
        static int _num = 0;

        static SpinLock _lock = new SpinLock(); 

        static void Thread_1()
        {
            for(int i=0; i<1000000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num) ;
        }



    }

}