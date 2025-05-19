using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network;
using MinesServer.Network.Auth;
using MinesServer.Network.BotInfo;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


namespace MinesServer.Server
{
    public class Auth(Session initiator)
    {
        public Window authwin;
        public bool complited = false;
        public string nick = "";
        public string passwd = "";
        public void CallAction(string text)
        {
            if (text.StartsWith("exit"))
            {
                temp = null;
                nick = "";
                passwd = "";
                authwin = def;
                initiator.SendWin(authwin.ToString());
                return;
            }
            authwin?.ProcessButton(text);
            initiator.SendWin(authwin.ToString());
        }
        public void NickNotA(Session initiator)
        {
            authwin.CurrentTab.Replace(new Page
            {
                Text = "Пароль\nВведён не верный пароль. Попробуйте ещё раз.",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = " "
                },
                Buttons = [new("OK", "%I%", (args) => TryToAuthByPlayer(args.Input))]
            });
        }
        private Window def => new Window()
        {
            Title = "ВХОД",
            Tabs = [new Tab()
                    {
                        Label = "Ник",
                        Action = "auth",
                        InitialPage = new Page()
                        {
                            Text = "Авторизация",
                            Buttons = [
                                new MButton("Новый акк", "newakk", (args) => CreateNew()),
                                new MButton("ok", $"nick:{ActionMacros.Input}", (args) => TryToFindByNick(args.Input!))
                            ],
                            Input = new InputConfig()
                            {
                                IsConsole = true,
                                Placeholder = " "
                            }
                        }
                    }],
            ShowTabs = false
        };
        public void TryToAuth(AUPacket p, string sid)
        {
            Console.WriteLine("auth?");
            int res;
            Player player = null;
            if (p.user_id.HasValue)
            {
                player = DataBase.GetPlayer(p.user_id.Value)!;
            }
            initiator.SendU(new WorldInfoPacket(World.W.name, World.CellsWidth, World.CellsHeight, 0, "COCK", "http://pi.door/", "ok"));
            if (player == null)
            {
                initiator.SendU(new BotInfoPacket("pidor", 0, 0, -1));
                initiator.SendU(new HBPacket([new HBMapPacket(0, 0, 32, 32, World.W.GetChunk(0,0).cells)]));
                authwin = def;
                initiator.SendWin(authwin.ToString());
                return;
            }
            else if (player != null && CalculateMD5Hash(player.hash + sid) == p.token)
            {
                player.connection = null;
                player.connection = initiator;
                initiator.player = player;
                initiator.SendU(new GuPacket());
                player.Init();
                return;
            }
            if (player == null)
            {
                return;
            }
            initiator.auth = null;
        }
        public static bool NickNotAvl(string nick)
        {
            using var db = new DataBase();
            try
            {
                Console.WriteLine(db.players.Count(p => p.name == nick));

                return db.players.Count(p => p.name == nick) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void CreateNew()
        {
            temp = new Player();
            authwin.CurrentTab.Open(new Page
            {
                Title = "НОВЫЙ ИГРОК",
                Text = "Ник",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = " "
                },
                Buttons = [new("OK", $"newnick:{ActionMacros.Input}", (args) => { using var db = new DataBase(); if (db.players.FirstOrDefault(i => i.name == args.Input) == null) { SetPasswdForNew(args.Input!); } else { initiator.SendU(new OKPacket("auth", "Ник занят")); CreateNew(); } })]
            });
            initiator.SendWin(authwin.ToString());
        }
        public void SetPasswdForNew(string nick)
        {
            this.nick = nick;
            authwin.CurrentTab.Open(new Page
            {
                Title = "НОВЫЙ ИГРОК",
                Text = "Пароль",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = " "
                },
                Buttons = [new("OK", $"passwd:{ActionMacros.Input}", (args) => EndCreateAndInit(args.Input!))]
            });
            initiator.SendWin(authwin.ToString());
        }
        public void EndCreateAndInit(string passwd)
        {
            using var db = new DataBase();
            temp.CreatePlayer();
            db.players.Add(temp);
            temp.passwd = passwd;
            temp.name = nick;
            db.skills.Attach(temp.skillslist);
            db.SaveChanges();
            initiator.SendU(new AHPacket(temp.id, temp.hash));
            initiator.player = DataBase.GetPlayer(temp.name);
            initiator.player.Death();
            db.SaveChanges();
            initiator.player = DataBase.GetPlayer(initiator.player.id);
            initiator.player.connection = initiator;
            initiator.player.Init();
        }
        public void TryToFindByNick(string name)
        {
            using var db = new DataBase();
            Player player = DataBase.GetPlayer(name);
            if (player != default(Player))
            {
                temp = player;
                nick = name;
                authwin.CurrentTab.Open(new Page
                {
                    Text = "Пароль",
                    Input = new InputConfig
                    {
                        IsConsole = true,
                        Placeholder = " "
                    },
                    Buttons = [new("OK", $"passwd:{ActionMacros.Input}", (args) => TryToAuthByPlayer(args.Input!))]
                });
                initiator.SendWin(authwin.ToString());
                return;

            }
            initiator.SendU(new OKPacket("auth", "Игрок не найден"));
            initiator.SendWorldInfo();
            initiator.SendWin(authwin.ToString());
        }
        public void TryToAuthByPlayer(string passwd)
        {
            if (temp.passwd == passwd)
            {
                complited = true;
                initiator.player = DataBase.GetPlayer(temp.id);
                initiator.player.connection = initiator;
                initiator.SendU(new AHPacket(temp.id, temp.hash));
                initiator.player.Init();
                return;
            }
            /*authwin.CurrentTab.Replace(new Page
            {
                Text = "Пароль\nВведён не верный пароль. Попробуйте ещё раз.",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = " "
                },
                Buttons = [new("OK", "%I%", (args) => TryToAuthByPlayer(args.Input, initiator))]
            });*/
            initiator.SendU(new OKPacket("auth", "Не верный пароль"));
            initiator.SendWin(authwin.ToString());

        }
        public Player temp = null;
        public static string GenerateSessionId()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnoprtsuxyz0123456789";
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string CalculateMD5Hash(string input)
        {
            HashAlgorithm hashAlgorithm = MD5.Create();
            var bytes = Encoding.ASCII.GetBytes(input);
            var array = hashAlgorithm.ComputeHash(bytes);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}


