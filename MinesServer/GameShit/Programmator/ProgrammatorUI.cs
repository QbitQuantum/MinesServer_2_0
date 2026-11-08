using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Inputs;
using MinesServer.GameShit.GUI.List;
using MinesServer.GameShit.GUI.Styles;
using MinesServer.Server;
using MinesServer.Utils;

namespace MinesServer.GameShit.Programmator
{
    public static class ProgrammatorUI
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

        public static ListEntry[]? LoadProgs(Player p)
        {
            using var db = new DataBase();
            var progs = db.progs.Where(i => i.owner == p).ToList();
            if (progs.Count == 0)
                return null;
            return progs.Select(i => new ListEntry(i.name, new MButton("open", $"openprog:{i.id}", (a) => OpenProg(p, i)))).ToArray();
        }

        public static void OpenProg(Player p, Program prog)
        {
            p.win = null;
            p.OpenProg(prog);
        }
        
        public static void Rename(Player p,int id)
        {
            void rename(ActionArgs args)
            {
                if (string.IsNullOrEmpty(args.Input))
                    return;

                if (InputValidation.AllowedCharsOnly.IsMatch(args.Input))
                {
                    using var db = new DataBase();
                    var prog = db.progs.FirstOrDefault(p => p.id == id);
                    prog.name = args.Input;
                    db.SaveChanges();
                    p.UpdateProg(prog);
                }
                p.win = null;
            }

            p.win = new Window()
            {
                Title = "RenameProg",
                Tabs = [new Tab()
                {
                    Action = "prog",
                    InitialPage = new Page()
                    {
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

        public static void OpenCreateProg(Player p)
        {
            void Handler(ActionArgs args)
            {
                if (string.IsNullOrEmpty(args.Input)) 
                    return;

                NewProg(p, args.Input);
            }

            p.win?.CurrentTab.Open(new Page()
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
                Buttons = [new MButton("Создать", $"create2{ActionMacros.Input}", Handler)]
            });
            p.SendWindow();
        }

        public static void OpenGui(Player p)
        {
            var ProgrammList = LoadProgs(p);

            p.win = new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "prog",
                    Label = "",
                    Title = "ПРОГРАММАТОР",
                    InitialPage = new Page()
                    {
                        List = ProgrammList,
                        Buttons = [
                            new MButton("Создать программу", "createrog", (e) => OpenCreateProg(p))]
                    }

                }]
            };
            p.SendWindow();
        }

        public static void DeleteProgram(Player p, int ProgramId)
        {
            DataBase.DeleteProg(ProgramId);
            p.OpenProg(null);
            OpenGui(p);
        }
    }
}
