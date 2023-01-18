using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Resolvers;
using static System.Formats.Asn1.AsnWriter;

namespace ServerCore
{
    class Porgram
    {
        //메모리 베리어
        //A 코드 재배치 억제
        //B 가시성

        //1)Full Memory Barrier(ASM MFENCE, C# Thread.MemoryBarrier) :Strore/Load 막음
        //2)Store Memory Barrier : Store만 막음
        //3)Load Memory Barrier : Load만 막음

        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;
        static void Thread_1()
        {
            y = 1;
            Thread.MemoryBarrier(); //서로 넘어갈 수 없게 경계선을 그으는것
            r1 = x;
        }
        static void Thread_2()
        {
            x = 1;
            Thread.MemoryBarrier();
            r2 = y;
        }

        static void Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번만에 빠져나옴!");

        }
    }

}