using MinesServer.GameShit.ClanSystem;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.Sys_Miss;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer.GameShit
{
    public static class MConsole
    {
        public delegate void Command(Player p, string args);
        private static Dictionary<string, Command> commands = [];

        public static void ShowConsole(Player p)
        {
            p.win = BuildWindowConsole(p);
            p.SendWindow();
        }

        public static void InitCommands()
        {
            commands.Add("tp", (p, args) =>
            {
                var arr = args.Split(" ");
                if (arr.Length > 1 && int.TryParse(arr[1], out var x) && int.TryParse(arr[2], out var y))
                {
                    p.x = x;
                    p.y = y;
                    p.teleport_tp(x, y);
                }
            });

            commands.Add("consolefedresp", (p, arg) =>
            {
                p.resp = null;
            });

            commands.Add("consoleclans", (p, arg) =>
            {
                Clan.OpenListAsync(p);
            });

            commands.Add("setitem", (p, arg) =>
            {
                if (arg.Split(" ").Length > 1 && int.TryParse(arg.Split(" ")[1], out var i) && int.TryParse(arg.Split(" ")[2], out var c))
                {
                    p.inventory[i] = c;
                    AddConsoleLine(p, "ok");
                    p.SendInventory();
                }
            });

            commands.Add("addgeo", (p, arg) =>
            {
                if (arg.Split(" ").Length > 1 && byte.TryParse(arg.Split(" ")[1], out var i))
                {
                    p.geo.Push(i);
                    p.SendGeo();
                }
            });

            commands.Add("addmoney", (p, arg) =>
            {
                if (arg.Split(" ").Length > 1 && int.TryParse(arg.Split(" ")[1], out var i))
                {
                    p.money += i;
                    AddConsoleLine(p, "ok");
                    p.SendMoney();
                }
            });

            commands.Add("myid", (p, arg) =>
            {
                AddConsoleLine(p, p.id.ToString());
            });

            commands.Add("getallmap", (p, arg) =>
            {
                World.SendWorldInfo(p);
            });

            commands.Add("setnick", (p, arg) =>
            {
                if (arg.Split(' ').Length > 0)
                {
                    p.name = arg.Split(' ')[1];
                    using var db = new DataBase();
                    db.players.Update(p);
                    db.SaveChanges();
                    p.SendBotUpdateInfo();
                }
            });

            commands.Add("addmis", (p, arg) =>
            {
                Task.Run(() =>
                new MissonDestroy("123").SendMisson(p));
            });

            commands.Add("fordevs", (p, arg) =>
            {
                ShowOldConsole(p);
            });
        }

        private static void AddConsoleLine(Player p, string text)
        {
            p.console.Enqueue(new Line { text = ">" + text });
            if (p.console.Count > 16)
            {
                p.console.Dequeue();
                var l = p.console.First();
                l.text = "@@" + l.text;
            }
        }

        private static Window? BuildWindowConsole(Player p)
        {
            void Handler(ActionArgs args)
            {
                if (string.IsNullOrEmpty(args.Action)) return;

                var command = args.Action;
                if (commands.TryGetValue(command, out Command? value))
                {
                    value(p, command);
                    p.SendWindow();
                    return;
                }
            }

            static CanvasElement Btn(string label, string action, int originX, int? originY = null, Action<ActionArgs>? handler = null) =>
                CanvasElement.Button(new MButton(label, action, handler), originDX: originX, originDY: originY);

            var page = new Page
            {
                Buttons = new[]
                {
                    new MButton("", "")
                },
                Text = "Приём трейдов от других игроков: <color=lime>ВКЛЮЧЕН</color>\nHarakiri: <color=red>ВЫКЛЮЧЕНО</color>",
                Style = new Style()
                {
                    Space = 0,
                    Canvas = new GridStyle()
                    {
                        Height = 275
                    }
                },
                Canvas = new[]
                {
                    // Строка 1
                    Btn("  ПЕРЕВОД <color=lime>$</color>/<color=yellow>C</color><color=white>/предметов</color>  ", "consolesend", -150, 100, Handler),
                    Btn("    <color=lime>Включить</color> harakiri    ", "consoleharakiri", 300, handler: Handler),

                    // Строка 2
                    Btn("      Сбросить респ      ", "consolefedresp", -300, -35, Handler),
                    Btn("          <color=white>Кланы</color>          ", "consoleclans", 300, handler: Handler),

                    // Строка 3
                    Btn("      Изменить <color=white>ник</color>       ", "consolenewnick", -300, -35, Handler),
                    Btn("  Изменить <color=yellow>цвет</color> в чате   ", "consolecolor", 300, handler: Handler),

                    // Строка 4
                    Btn(" <color=yellow>Купить FreeUp/Прокачку x3</color>", "consolegetupgrades", -300, -35, Handler),
                    Btn("          Мой ID         ", "consolemyid", 300, handler: Handler),

                    // Строка 5
                    Btn("      <color=white>Последний бокс</color>     ", "info_box", -300, -35, Handler),
                    Btn("         <color=white>Трейды</color>          ", "consoletrades", 300, handler: Handler),

                    // Строка 6
                    Btn(" <color=red>Включить</color> приём <color=white>трейдов</color>  ", "consoletradesonoff", -300, -35, Handler),
                    Btn("   Цены на <color=yellow>кристаллы</color>     ", "consoleprices", 300, handler: Handler),

                    // Строка 7
                    Btn("   <color=white>История переводов</color>     ", "mytranslationsbutton", -300, -35, Handler),
                    Btn("   <color=yellow>Для разработчиков</color>     ", "fordevs", 300, handler: Handler),

                    // Строка 8
                    Btn("   <color=cyan>Отладочная позиция</color>    ", "test", -300, -35, Handler),
                    Btn("   <color=red>Сброс авторизации</color>     ", "re_auth", 300, handler: Handler),
                }
            };

            return new Window
            {
                Title = "КОНСОЛЬ",
                ShowTabs = true,
                Tabs = new[]
                {
                    new Tab
                    {
                        Label = "ОБЩИЕ",
                        Action = "",
                        InitialPage = page
                    }
                }
            };
        }

        private static void ShowOldConsole(Player p)
        {
            void Handler(ActionArgs args)
            {
                if (string.IsNullOrEmpty(args.Input)) return;
                var msg = args.Input;
                AddConsoleLine(p, msg);
                if (msg.Contains(' '))
                {
                    string command = msg.Split(' ')[0];
                    if (commands.TryGetValue(command, out Command? value))
                    {
                        value(p, msg);
                    }
                }
                ShowOldConsole(p);
            }

            p.win = new Window()
            {
                Title = "Консоль",
                Tabs = 
                [
                    new Tab()
                    {
                        Label = "",
                        Action = "console",
                        InitialPage = new Page()
                        {
                            Input = new InputConfig()
                            {
                                Placeholder = "cmd",
                                IsConsole = true
                            },
                            ClanList = [],
                            Text = string.Join("", p.console.Select(x => x.text + '\n').ToArray()),
                            Buttons = [new MButton("Выполнить команду", $"{ActionMacros.Input}", Handler)]
                        }
                    }
                ],
                ShowTabs = false
            };

        }
    }

    public class Line
    {
        public string text { get; set; }
    }
}
