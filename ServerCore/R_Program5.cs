using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Porgram
    {
        static int num = 0;

        static void Thread1()
        {
            //int prev=num;
            for (int i = 0; i < 1000000; i++) 
                Interlocked.Increment(ref num);//원자적(쪼개질 수 없는 상태)으로 만들어줌-하지만 성능에서 손해를 많이봄.
            //int afterValue = Interlocked.Increment(ref num);//다음 값 추출방법
            //    num++;

            //int next = num; //주소값으로 접근하기에 다음 값을 알 수 없다.
        }

        static void Thread2()
        {
            for (int i = 0; i < 1000000; i++)
               // num--;
            Interlocked.Decrement(ref num);
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