using System;
using System.Net;

namespace StunTest
{
    // https://tools.ietf.org/html/rfc5389#section-15.1
    public class MappedAddressAttribute : StunAttribute, IMappedAddressAttribute
    {
        public override AttributeType Type => AttributeType.MappedAddress;

        public IPAddress Address { get; set; }
        public UInt16 Port { get; set; }

        public override void WriteTo(NetworkWriter writer)
        {
            throw new NotImplementedException();
        }

        public void ReadFrom(NetworkReader reader)
        {
            var padding = reader.ReadByte();
            var family = (MappedAddressFamily)reader.ReadByte();
            Port = reader.ReadUInt16();

            if (family == MappedAddressFamily.IPv6)
            {
                Address = new IPAddress(reader.ReadBytes(16));
            }
            else if (family == MappedAddressFamily.IPv4)
            {
                Address = new IPAddress(reader.ReadBytes(4));
            }
        }
    }
}