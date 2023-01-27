using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ServerCore
{
    //재귀적 락을 허용할지(Yes 일경우 WriteLock->Write Lock OK, WriteLock->ReadLock OK, ReadLock->WriteLock No
    //스핀락 정책(5000번-> Yield)


    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        //[Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
        int _flag=EMPTY_FLAG;
        int _writeCount = 0;
        public void WriteLock()
        {
            //동일 쓰레드가  WriteLock를 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId==lockThreadId)
            {
                _writeCount++;
                return;
            }
            int desired=(Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while(true)
            {
                for(int i =0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
       
                }
            }
        }
        public void WriteUnLock()
        {
            int lockCount = --_writeCount;
            if(lockCount ==0)Interlocked.Exchange(ref _flag,EMPTY_FLAG);
        }
        public void ReadLock()
        {
            //동일 쓰레드가  WriteLock를 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            //아무도 WriteLock를 획득하지 않으면, ReadCount 를 1 늘린다.
            while (true)
            {
                for(int i=0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;//실패조건 : 원하는 값이 되면 통과, 여러스레드가 동시에 할 경우 첫번째만 늘릴 수 있음.
 
                }
                Thread.Yield();
            }
        }
        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);
        }


    }

}
