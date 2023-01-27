using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        
        ArraySegment<byte> _buffer;
        int _readPos;//처리를 하는 위치
        int _writePos;//쓸 수 있는 위치

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } }//데이터 크기
        public int FreeSize { get { return _buffer.Count - _writePos; } }//버퍼에 남은 공간

        public ArraySegment<byte> DataSegment //어디 부터 데이터를 읽어야 하는지에 대한 유효범위
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean() // 중간중간 정리를 해주어 readPos,writePos를 처음으로 당겨주는 함수
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 위치만 리셋.
                _readPos = 0;
                _writePos = 0;
            }
            else
            {
                // 남은 찌끄레기가 있으면 시작 위치로 복사.
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)//데이터를 가공해서 처리했을 때 성공했을 경우
        {
            if (numOfBytes > DataSize)
                return false;
            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)//클라에서 데이터를 쏴줬을 때 Recv만큼 WritePos를 이동
        {
            if (numOfBytes > FreeSize)
                return false;
            _writePos += numOfBytes;
            return true;
        }
    }
}
