using System;
using System.Net;

namespace StunTest
{
    public interface IMappedAddressAttribute
    {
        IPAddress Address { get; }
        UInt16 Port { get; }
    }

    // https://tools.ietf.org/html/rfc5389#section-15.1
    public enum MappedAddressFamily : byte
    {
        IPv4 = 0x01,
        IPv6 = 0x02,
    }
}