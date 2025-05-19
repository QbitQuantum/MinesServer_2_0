using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.Programmator.SevenZip.LZMA;
using MinesServer.Network.Programmator;
using MinesServer.Server;
using System.Text;

namespace MinesServer.GameShit.Programmator
{
    public static class StaticGUI
    {

        public static void NewProg(Player p, string name)
        {
            using var db = new DataBase();
            db.players.Attach(p);
            var prog = new Program(p, name, "");
            p.programs.Add(prog);
            db.SaveChanges();
            p.OpenProg(prog);
            p.win = null;
        }
        public static ListEntry[] LoadProgs(Player p)
        {
            using var db = new DataBase();
            var progs = db.progs.Where(i => i.owner == p).ToList();
            if (progs.Count == 0)
                return [];
            return progs.Select(i => new ListEntry(i.name, new MButton("open", $"openprog:{i.id}", (a) => OpenProg(p, i)))).ToArray();
        }
        public static void OpenProg(Player p, Program prog)
        {
            p.win = null;
            p.OpenProg(prog);
        }
        
        public static void Rename(Player p,int id)
        {
            var rename =  (ActionArgs args) =>
            {
                if (Default.def.IsMatch(args.Input))
                {
                    using var db = new DataBase();
                    var prog = db.progs.FirstOrDefault(p => p.id == id);
                    prog.name = args.Input;
                    db.SaveChanges();
                    p.connection?.SendU(new UpdateProgrammatorPacket(prog.id, prog.name, prog.data));
                }
                p.win = null;
            };
            p.win = new Window()
            {
                Title = "RenameProg",
                Tabs = [new Tab()
                {
                    Action = "pren1",
                    InitialPage = new Page(){
                        Text = "Rename\n",
                Input = new InputConfig()
                {
                    Placeholder = "Название программы..."
                },
                Style = new Style()
                {
                    FixScrollTag = "prg"
                },
                Buttons = [new MButton("Ok", $"rename{ActionMacros.Input}", rename)]
                    },
                    Label = "pren2"
                }]
            };
            p.SendWindow();
        }
        public static void StartedProg(Player p, (int id, string source) data)
        {
            using var db = new DataBase();
            var programm = db.progs.FirstOrDefault(i => i.id == data.id);
            if (programm != default)
            {
                programm.data = data.source;
                db.SaveChanges();
                p.RunProgramm(programm);
                p.UpdateProg(programm);
            }
        }
        public static void OpenCreateProg(Player p)
        {
            p.win.CurrentTab.Open(new Page()
            {
                Text = "Введите название вашей программы\n",
                Input = new InputConfig()
                {
                    Placeholder = "Название программы..."
                },
                Style = new Style()
                {
                    FixScrollTag = "prg"
                },
                Buttons = [new MButton("Создать", $"create2{ActionMacros.Input}", (args) => NewProg(p, args.Input))]
            });
            p.SendWindow();
        }
        public static void DeleteProg(Player p,int id)
        {
            using var db = new DataBase();
            db.progs.Remove(db.progs.FirstOrDefault(i => i.id == id)!);
            db.SaveChanges();
        }
        public static void OpenGui(Player p)
        {
            var l = LoadProgs(p);
            if (l.Length > 0)
            {
                p.win = new Window()
                {
                    Tabs = [new Tab()
                {
                    Action = "prog",
                    Label = "",
                    Title = "ПРОГРАММАТОР",
                    InitialPage = new Page()
                    {
                        List = l,
                        Buttons = [new MButton("Создать","createrog",(e) => OpenCreateProg(p))]
                    }

                }]
                };
                p.SendWindow();
                return;
            }
            p.win = new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "prog",
                    Label = "",
                    Title = "ПРОГРАММАТОР",
                    InitialPage = new Page()
                    {
                        Buttons = [new MButton("СОЗДАТЬ ПРОГРАММУ", "createprog", (args) => OpenCreateProg(p))]
                    }

                }]
            };
            p.SendWindow();
        }
    }
}
