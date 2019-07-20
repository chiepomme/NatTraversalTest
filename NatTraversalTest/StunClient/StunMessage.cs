using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StunTest
{
    public class StunMessage
    {
        public static readonly byte[] MagicCookie = new byte[] { 0x21, 0x12, 0xA4, 0x42 };
        public byte[] TransactionId;

        static readonly Random random = new Random();

        public readonly StunMessageType MessageType = new StunMessageType();
        public readonly List<StunAttribute> Attributes = new List<StunAttribute>();

        public StunMessage() { }

        public StunMessage(MessageTypeClass messageClass, MessageTypeMethod messageMethod)
        {
            MessageType.Class = messageClass;
            MessageType.Method = messageMethod;

            GenerateTransactionId();
        }

        public void GenerateTransactionId()
        {
            TransactionId = new byte[12];
            random.NextBytes(TransactionId);
        }

        public void ReadFrom(NetworkReader reader)
        {
            MessageType.ReadFrom(reader);

            var length = reader.ReadUInt16();

            var magicCookie = reader.ReadBytes(4);
            if (!magicCookie.SequenceEqual(MagicCookie))
            {
                throw new InvalidDataException("Magic Cookie が一致しません。");
            }

            TransactionId = reader.ReadBytes(12);

            Attributes.Clear();

            while (!reader.EOF)
            {
                Attributes.Add(StunAttribute.CreateFrom(reader, TransactionId));
            }
        }

        public void WriteTo(NetworkWriter writer)
        {
            var attributeBytes = ConvertAttributesToBytes();

            MessageType.WriteTo(writer);
            writer.Write((UInt16)attributeBytes.Length); // length except the header
            writer.Write(MagicCookie);
            writer.Write(TransactionId);
            writer.Write(attributeBytes);
        }

        byte[] ConvertAttributesToBytes()
        {
            using (var attrStream = new MemoryStream())
            using (var attrWriter = new NetworkWriter(attrStream))
            {
                foreach (var attribute in Attributes)
                {
                    attribute.WriteTo(attrWriter);
                }

                return attrStream.ToArray();
            }
        }
    }
}