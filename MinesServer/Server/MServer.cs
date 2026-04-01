using MinesServer.GameShit;
using MinesServer.GameShit.WorldSystem;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;
namespace MinesServer.Server
{
    public class MServer : TcpServer
    {
        public System.Timers.Timer timer;
        public ServerTime time { get; private set; }
        public static MServer? Instance;
        public static bool started = false;
        CancellationTokenSource s = new();

        public MServer(IPAddress address, int port) : base(address, port)
        {
            Instance = this;
            MConsole.InitCommands();
            GameShit.SysCraft.RDes.Init();
            OptionKeepAlive = true;
        }
        public override bool Start()
        {
            new World(Default.cfg.WorldName);
            time = new ServerTime();
            return base.Start();
        }
        public override bool Stop()
        {
            s.Cancel();
            return base.Stop();
        }
        protected override TcpSession CreateSession()
        {
            var s = new Session(this);
            return s;
        }
        protected override void OnError(SocketError error)
        {
            Default.WriteError(error.ToString());
        }
    }
}
