using System;
using System.Linq;
using System.Net;

namespace StunTest
{
    // https://tools.ietf.org/html/rfc5389#section-15.2
    public class XorMappedAddressAttribute : StunAttribute, IMappedAddressAttribute
    {
        public override AttributeType Type => AttributeType.XorMappedAddress;

        public IPAddress Address { get; set; }
        public UInt16 Port { get; set; }

        public override void WriteTo(NetworkWriter writer)
        {
            throw new NotImplementedException();
        }

        public void ReadFrom(NetworkReader reader, byte[] transactionId)
        {
            var padding = reader.ReadByte();
            var family = (MappedAddressFamily)reader.ReadByte();
            var xPort = reader.ReadUInt16();
            Port = (UInt16)(xPort ^ 0x2112);

            if (family == MappedAddressFamily.IPv6)
            {
                var xAddress = reader.ReadBytes(16);
                var magicCookieAndTransactionId = StunMessage.MagicCookie.Concat(transactionId).ToArray();
                var addressBytes = xAddress.Select((v, i) => (byte)(v ^ magicCookieAndTransactionId[i])).ToArray();
                Address = new IPAddress(addressBytes);
            }
            else if (family == MappedAddressFamily.IPv4)
            {
                var xAddress = reader.ReadBytes(4);
                var addressBytes = xAddress.Select((v, i) => (byte)(v ^ StunMessage.MagicCookie[i])).ToArray();
                Address = new IPAddress(addressBytes);
            }
        }
    }
}
