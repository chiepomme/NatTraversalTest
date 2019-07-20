using System;

namespace StunTest
{
    // https://tools.ietf.org/html/rfc5389#section-18.2
    public enum AttributeType : UInt16
    {
        MappedAddress = 0x0001,
        ChangeRequest = 0x0003, // Obsolete?
        Username = 0x0006,
        MessageIntegrity = 0x0008,
        ErrorCode = 0x0009,
        UnknownAttributes = 0x000A,
        Realm = 0x0014,
        Nonce = 0x0015,
        XorMappedAddress = 0x0020,
    }

    // https://tools.ietf.org/html/rfc5389#section-15
    public abstract class StunAttribute
    {
        public abstract AttributeType Type { get; }

        public abstract void WriteTo(NetworkWriter writer);

        public static StunAttribute CreateFrom(NetworkReader reader, byte[] transactionId)
        {
            var type = (AttributeType)reader.ReadUInt16();
            var attrLength = reader.ReadUInt16();

            switch (type)
            {
                case AttributeType.XorMappedAddress:
                {
                    var attr = new XorMappedAddressAttribute();
                    attr.ReadFrom(reader, transactionId);
                    return attr;
                }
                case AttributeType.MappedAddress:
                {
                    var attr = new MappedAddressAttribute();
                    attr.ReadFrom(reader);
                    return attr;
                }
                case AttributeType.ChangeRequest:
                {
                    var attr = new ChangeRequestAttribute();
                    attr.ReadFrom(reader);
                    return attr;
                }

                default:
                    throw new NotImplementedException(type + "には対応していません");
            }
        }
    }
}