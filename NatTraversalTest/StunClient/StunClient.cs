using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace StunTest
{
    public class StunClient
    {
        readonly UdpClient udp;

        public bool HasMessage => udp.Available > 0;

        public IPEndPoint LocalEndPoint => (IPEndPoint)udp.Client.LocalEndPoint;
        public IPEndPoint StunEndPoint { get; private set; }

        public StunClient(UdpClient udp)
        {
            var ipHostEntry = Dns.GetHostEntry("stun.l.google.com");
            var address = ipHostEntry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork); // Force v4
            StunEndPoint = new IPEndPoint(address, 19302);

            this.udp = udp;
        }

        public void Send(StunMessage message)
        {
            using (var stream = new MemoryStream())
            using (var writer = new NetworkWriter(stream))
            {
                message.WriteTo(writer);
                var bytes = stream.ToArray();
                udp.Send(bytes, bytes.Length, StunEndPoint);
            }
        }

        public StunMessage Receive()
        {
            if (!HasMessage) throw new Exception("メッセージがありません");

            var endpoint = new IPEndPoint(IPAddress.Any, 0);
            var receivedBytes = udp.Receive(ref endpoint);

            using (var stream = new MemoryStream(receivedBytes))
            using (var reader = new NetworkReader(stream))
            {
                var message = new StunMessage();
                message.ReadFrom(reader);
                return message;
            }
        }
    }
}