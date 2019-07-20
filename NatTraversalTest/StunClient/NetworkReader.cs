using System;
using System.IO;

namespace StunTest
{
    public class NetworkReader : IDisposable
    {
        readonly Stream stream;
        readonly byte[] innerBuffer = new byte[64];

        public bool EOF => stream.Position == stream.Length;

        public NetworkReader(Stream stream)
        {
            this.stream = stream;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public byte ReadByte()
        {
            Read(innerBuffer, 0, 1);
            return innerBuffer[0];
        }

        public byte[] ReadBytes(int count)
        {
            var buffer = new byte[count];
            Read(buffer, 0, count);
            return buffer;
        }

        public UInt16 ReadUInt16()
        {
            Read(innerBuffer, 0, 2);
            return (UInt16)((innerBuffer[0] << 8 * 1) +
                            (innerBuffer[1] << 8 * 0));
        }

        public Int16 ReadInt16()
        {
            Read(innerBuffer, 0, 2);
            return (Int16)((innerBuffer[0] << 8 * 1) +
                           (innerBuffer[1] << 8 * 0));
        }

        public UInt32 ReadUInt32()
        {
            Read(innerBuffer, 0, 4);
            return (UInt32)((innerBuffer[0] << 8 * 3) +
                            (innerBuffer[1] << 8 * 2) +
                            (innerBuffer[2] << 8 * 1) +
                            (innerBuffer[3] << 8 * 0));
        }

        public Int32 ReadInt32()
        {
            Read(innerBuffer, 0, 4);
            return (Int32)((innerBuffer[0] << 8 * 3) +
                           (innerBuffer[1] << 8 * 2) +
                           (innerBuffer[2] << 8 * 1) +
                           (innerBuffer[3] << 8 * 0));
        }

        public void Dispose()
        {
        }
    }
}