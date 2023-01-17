using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Resolvers;

namespace ServerCore
{
    class Porgram
    {
     
    
        static void Main(string[] args)
        {

            int[,] arr = new int[10000, 10000];

            {//캐시에서 옆주소를 미리 가지고 있다
                long now = DateTime.Now.Ticks;
                for(int y = 0; y < 10000; y++)
                    for(int x= 0; x<10000;x++)
                        arr[y,x] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y,x)순서 걸린 시간 {end-now}");

            }

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[x, y] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y,x)순서 걸린 시간 {end - now}");

            }
        }
    }

}