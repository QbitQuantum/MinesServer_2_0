using MinesServer.GameShit;
using MinesServer.GameShit.WorldSystem;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;

namespace MinesServer.Server
{
    public class MServer : TcpServer
    {
        private readonly ServerTime time = new ServerTime();

        public MServer(IPAddress address, int port) : base(address, port)
        {
            MConsole.InitCommands();
            OptionKeepAlive = true;
        }

        public override bool Start()
        {
            new World(Default.cfg.WorldName);
            time.Start();
            return base.Start();
        }

        public override bool Stop()
        {
            time.Dispose();
            return base.Stop();
        }

        protected override TcpSession CreateSession()
        {
            return new Session(this, time);
        }
        protected override void OnError(SocketError error)
        {
            Default.WriteError(error.ToString());
        }
    }
}
