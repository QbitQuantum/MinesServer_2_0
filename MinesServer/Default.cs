using System.Net;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer
{
    public static partial class Default
    {
        private static readonly int port = 8090;
        private static readonly IPAddress iPAddress = IPAddress.Any;

        private static readonly Dictionary<string, Action> commands = [];
        private static MServer server { get; set; }

        public static void Main(string[] args)
        {
            server = new MServer(iPAddress, port);
            server.Start();
            InitCommand();
            CommandProcessing();
        }

        private static void InitCommand()
        {
            commands.Add("save", () =>
            {
                using var db = new DataBase();
                db.SaveChanges();
                World.CommitWorld();
            });
            commands.Add("restart", () => {
                server.Stop();
                Console.WriteLine("kinda restart");
                server.Start();
            });
            commands.Add("players", () =>
            {
                Console.WriteLine($"online {DataBase.activeplayers.Count}");
                for (int i = 0; i < DataBase.activeplayers.Count; i++)
                {
                    var player = DataBase.activeplayers.ElementAt(i);
                    Console.WriteLine($"id: {player.id}\n name :[{player.name}] online:{player.online}");
                }
            });
            commands.Add("fullnew", () =>
            {
                server.Stop();
                World.W.DeleteWorld();
                var db = new DataBase();
                db.Delete();
                server.Start();
            });
            //buggy
            commands.Add("newworld", () =>
            {
                server.Stop();
                DataBase.ClearAll();
                World.W.DeleteWorld();
                server.Start();
            });
        }

        private static void CommandProcessing()
        {
            for (; ; )
            {
                var l = Console.ReadLine();
                if (l != null && commands.Keys.Contains(l))
                    commands[l]();
            }
        }
    }
}