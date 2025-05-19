using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MinesServer
{

    public static class Default
    {
        static public bool HasProperty(this Type type, string name)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.Name == name);
        }
        public static bool ToBool(this string s) => s != "0";
        public static int port = 8090;
        private static Dictionary<string, Action> commands = new Dictionary<string, Action>();
        public static void Main(string[] args)
        {
            CellsSerializer.Load();
            new ImgSpace();
            var configPath = "config.json";
            if (File.Exists(configPath))
                cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
            else
            {
                cfg = new Config();
                cfg.WorldName = "ff";
                File.WriteAllText(configPath, JsonConvert.SerializeObject(cfg, Formatting.Indented));
            }
            server = new MServer(System.Net.IPAddress.Any, port);
            server.Start();
            Loop();

        }
        private static void Loop()
        {
            commands.Add("save", () =>
            {
                using var db = new DataBase();
                db.SaveChanges();
                World.CommitWorld();
            });
            commands.Add("restart", () => { server.Stop(); Console.WriteLine("kinda restart"); server.Start(); });
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
                MServer.Instance?.time.Dispose();
                World.W.DeleteWorld();
                var db = new DataBase();
                db.Delete();
                server.Start();
            });
            //buggy
            commands.Add("newworld", () =>
            {
                server.Stop();
                MServer.Instance?.time.Dispose();
                using var db = new DataBase();
                DataBase.activeplayers = new List<Player>();
                foreach (var i in db.players) i.resp = null;
                db.boxes.RemoveAll();
                db.teleports.RemoveAll();
                db.resps.RemoveAll();
                db.ups.RemoveAll();
                db.storages.RemoveAll();
                db.vulkans.RemoveAll();
                db.markets.RemoveAll();
                db.guns.RemoveAll();
                db.gates.RemoveAll();
                db.gates.RemoveAll();
                db.SaveChanges();
                World.W.DeleteWorld();
                server.Start();
            });
            for (; ; )
            {
                var l = Console.ReadLine();
                if (l != null && commands.Keys.Contains(l))
                    commands[l]();
            }
        }
        public static void RemoveAll<T>(this Microsoft.EntityFrameworkCore.DbSet<T> s) where T : class => s.RemoveRange(s);
        public static Bitmap ConvertMapPart(int fromx,int fromy,int tox,int toy)
        {
            var bitmap = new Bitmap(tox - fromx, toy - fromy);
            for (int x = 0; fromx + x < tox; x++)
            {
                for (int y = 0; fromy + y < toy; y++)
                {
                    bitmap.SetPixel(x, y, World.GetProp(fromx + x, fromy + y).isEmpty ? Color.Green : Color.CornflowerBlue);
                }
            }
            return bitmap;
        }
        public static Config cfg;
        public static Regex def = new Regex("^[а-яА-ЯёЁa-zA-Z 0-9]+$");
        public static void WriteError(string ex)
        {
            var trace = new System.Diagnostics.StackTrace();
            var method = trace.GetFrame(1).GetMethod().Name;
            Console.WriteLine($"{method} caused error {ex}");
        }
        public static int size = 1;
        public static MServer server { get; set; }
    }
}