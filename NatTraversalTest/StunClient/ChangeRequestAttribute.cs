using System;

namespace StunTest
{
    // https://tools.ietf.org/html/rfc3489#section-11.2.4
    public class ChangeRequestAttribute : StunAttribute
    {
        public override AttributeType Type => AttributeType.ChangeRequest;

        public bool ChangeIP { get; set; }
        const UInt32 ChangeIPMask = 0b_00000000_00000000_00000000_00000100;
        public bool ChangePort { get; set; }
        const UInt32 ChangePortMask = 0b_00000000_00000000_00000000_00000010;

        public ChangeRequestAttribute() { }
        public ChangeRequestAttribute(bool changeIP, bool changePort)
        {
            ChangeIP = changeIP;
            ChangePort = changePort;
        }

        public override void WriteTo(NetworkWriter writer)
        {
            var v = 0u;
            if (ChangeIP) v |= ChangeIPMask;
            if (ChangePort) v |= ChangePortMask;
            writer.Write(v);
        }

        public void ReadFrom(NetworkReader reader)
        {
            var v = reader.ReadUInt32();
            ChangeIP = (v & ChangeIPMask) == ChangeIPMask;
            ChangePort = (v & ChangePortMask) == ChangePortMask;
        }
    }
}