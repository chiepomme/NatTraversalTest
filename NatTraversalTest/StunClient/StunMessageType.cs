using System;
using System.IO;

namespace StunTest
{
    // https://tools.ietf.org/html/rfc5389#section-6
    // mask: 0b0000_0001_0001_0000
    public enum MessageTypeClass : UInt16
    {
        Request = 0b0000_0000_0000_0000,
        Indication = 0b0000_0000_0001_0000,
        SuccessResp = 0b0000_0001_0000_0000,
        ErrResp = 0b0000_0001_0001_0000,
    }

    // https://tools.ietf.org/html/rfc5389#section-18.1
    // mask: 0b0011_1110_1110_1111
    public enum MessageTypeMethod : UInt16
    {
        Binding = 0b0000_0000_0000_0001,
    }

    public class StunMessageType
    {
        const UInt16 ZeroMask = 0b1100_0000_0000_0000;
        const UInt16 ClassMask = 0b0000_0001_0001_0000;
        const UInt16 MethodMask = 0b0011_1110_1110_1111;

        public MessageTypeClass Class { get; set; }
        public MessageTypeMethod Method { get; set; }

        public void ReadFrom(NetworkReader reader)
        {
            var value = reader.ReadUInt16();
            var zero = value & 0b1100_0000_0000_0000;

            Class = (MessageTypeClass)(value & 0b0000_0001_0001_0000);
            Method = (MessageTypeMethod)(value & 0b0011_1110_1110_1111);

            if (zero != 0)
            {
                throw new InvalidDataException("Stun のメッセージではありません。");
            }
        }

        public void WriteTo(NetworkWriter writer)
        {
            var messageType = (UInt16)((UInt16)Class | (UInt16)Method);
            writer.Write(messageType);
        }
    }
}