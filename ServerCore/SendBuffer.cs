using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    //샌드버퍼를 내부에서 만들경우
    // 1의 이동정보->100명에게 공유
    // 100의 정보면 10000개를 만들어야함.
    //하지만 외부에 만들면 100개의 정보만 만들면됨.

    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new(() => { return null; });//자신의 지역에서만 사용하게하는 스레드

        public static int ChunkSize { get; set; } = 4096 * 100;//초기 버퍼 크기 설정

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            if (CurrentBuffer.Value.FreeSize < reserveSize)//C++에서는 메모리 풀사용하여 최적화하여 사용 가능
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }
    public class SendBuffer
    {
        readonly byte[] _buffer;
        int _usedSize = 0;

        public int FreeSize { get { return _buffer.Length-_usedSize; } }
        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);//작업할 영역 리턴
        }

        public ArraySegment<byte> Close(int usedSize) //실제로 사용한 크기
        {
            ArraySegment<byte> segment = new(_buffer, this._usedSize, usedSize);
            this._usedSize += usedSize;
            return segment;
        }


    }
}
