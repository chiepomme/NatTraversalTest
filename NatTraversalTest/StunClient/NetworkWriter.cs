using System;
using System.IO;

namespace StunTest
{
    public class NetworkWriter : IDisposable
    {
        readonly Stream stream;
        readonly byte[] innerBuffer = new byte[64];

        public NetworkWriter(Stream stream)
        {
            this.stream = stream;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public void Write(byte value)
        {
            innerBuffer[0] = value;
            Write(innerBuffer, 0, 1);
        }

        public void Write(UInt16 value)
        {
            innerBuffer[0] = (byte)((value & 0xFF00) >> 8 * 1);
            innerBuffer[1] = (byte)((value & 0x00FF) >> 8 * 0);
            Write(innerBuffer, 0, 2);
        }

        public void Write(Int16 value)
        {
            innerBuffer[0] = (byte)((value & 0xFF00) >> 8 * 1);
            innerBuffer[1] = (byte)((value & 0x00FF) >> 8 * 0);
            Write(innerBuffer, 0, 2);
        }

        public void Write(UInt32 value)
        {
            innerBuffer[0] = (byte)((value & 0xFF00_0000) >> 8 * 3);
            innerBuffer[1] = (byte)((value & 0x00FF_0000) >> 8 * 2);
            innerBuffer[2] = (byte)((value & 0x0000_FF00) >> 8 * 1);
            innerBuffer[3] = (byte)((value & 0x0000_00FF) >> 8 * 0);
            Write(innerBuffer, 0, 4);
        }

        public void Write(Int32 value)
        {
            innerBuffer[0] = (byte)((value & 0xFF000000) >> 8 * 3);
            innerBuffer[1] = (byte)((value & 0x00FF0000) >> 8 * 2);
            innerBuffer[2] = (byte)((value & 0x0000FF00) >> 8 * 1);
            innerBuffer[3] = (byte)((value & 0x000000FF) >> 8 * 0);
            Write(innerBuffer, 0, 4);
        }

        public void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public void Dispose()
        {
        }
    }
}