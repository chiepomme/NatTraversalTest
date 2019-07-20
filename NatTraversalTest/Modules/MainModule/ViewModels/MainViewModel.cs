using Prism.Commands;
using Prism.Mvvm;
using StunTest;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace NatTraversalTest.MainModule.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable
    {
        readonly UdpClient udp;
        readonly DispatcherTimer timer;
        readonly StunClient stun;

        public IPAddress MyIPAddress { get; set; }
        public UInt16 MyPort { get; set; }
        public IPAddress RemoteIPAddress { get; set; }
        public UInt16 RemotePort { get; set; }

        public string LogMessage { get; set; }

        public DelegateCommand SendCommand { get; }
        public DelegateCommand GetMyIPAndPortCommand { get; }

        public MainViewModel()
        {
            SendCommand = new DelegateCommand(() => Send());
            GetMyIPAndPortCommand = new DelegateCommand(() => GetMyIPAndPort());

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                GenerateDesignFixture();
                return;
            }
            else
            {
                udp = new UdpClient();
                const int SIO_UDP_CONNRESET = -1744830452;
                udp.Client.IOControl(SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);

                stun = new StunClient(udp);

                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100),
                };

                timer.Tick += OnTick;
                timer.Start();
            }
        }

        void GenerateDesignFixture()
        {
            MyIPAddress = IPAddress.Any;
            MyPort = 3535;
            RemoteIPAddress = IPAddress.Broadcast;
            RemotePort = 7777;

            Log("これがろぐ");
            Log("これもろぐ");
            Log("あれもろぐ");
        }

        void Log(string message)
        {
            LogMessage = message + "\n" + LogMessage;
            LogMessage = LogMessage.Substring(0, Math.Min(LogMessage.Length, 10000));
        }

        void OnTick(object sender, EventArgs e)
        {
            if (udp.Available > 0)
            {
                var ep = new IPEndPoint(IPAddress.Any, 0);
                var bytes = udp.Receive(ref ep);

                try
                {
                    var text = Encoding.UTF8.GetString(bytes);
                    Log($"[受信({ep.Address.ToString()}:{ep.Port})]{text}");
                }
                catch
                {
                    Log($"[受信({ep.Address.ToString()}:{ep.Port})] テキストはパースできませんでした");
                }
            }
        }

        void Send()
        {
            Log("送信だ！");

            try
            {
                var text = Encoding.UTF8.GetBytes($"メッセージだよ！{DateTime.Now.ToShortTimeString()}");
                udp.Send(text, text.Length, new IPEndPoint(RemoteIPAddress, RemotePort));
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }

        void GetMyIPAndPort()
        {
            timer.Stop();
            try
            {
                var req = new StunMessage(MessageTypeClass.Request, MessageTypeMethod.Binding);
                stun.Send(req);

                var received = false;
                var timeout = DateTime.Now.AddSeconds(1);
                while (DateTime.Now < timeout)
                {
                    if (!stun.HasMessage) continue;

                    var res = stun.Receive();
                    var mappedAddress = res.Attributes.OfType<IMappedAddressAttribute>().First();
                    MyIPAddress = RemoteIPAddress = mappedAddress.Address;
                    MyPort = RemotePort = mappedAddress.Port;
                    received = true;
                    break;
                }

                if (!received)
                {
                    Log("STUN の応答が帰ってきませんでした");
                }
            }
            catch (Exception e)
            {
                Log($"[エラー] {e}");
            }
            timer.Start();
        }

        public void Dispose()
        {
            timer.Stop();
            udp.Dispose();
        }
    }
}
