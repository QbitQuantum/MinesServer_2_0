using MinesServer.GameShit.ClanSystem;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.Network;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Settings
    {
        public string serialized { get; set; } = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>
        {
            ["cc"] = "10",
            ["snd"] = "0",
            ["mus"] = "0",
            ["isca"] = "0",
            ["tsca"] = "0",
            ["mous"] = "1",
            ["pot"] = "0",
            ["frc"] = "1",
            ["ctrl"] = "1",
            ["mof"] = "1"
        });

        public Settings() { }

        public string this[string key]
        {
            get
            {
                sett ??= Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized) ?? [];
                return sett[key];
            }
            set
            {
                using var db = new DataBase();
                sett ??= Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized) ?? [];
                sett[key] = value;
                db.SaveChanges();
            }
        }
        [Key] public int id { get; set; }
        
        [NotMapped] private Dictionary<string, string>? sett = null;
        public void SendSettings(Player p)
        {
            sett = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized) ?? [];
            p.connection?.SendU(new SettingsPacket(sett));
        }
        public void Save(Player p, Dictionary<string, string> list)
        {
            foreach (var i in list)
            {
                this[i.Key] = i.Value;
            }
            SendSettings(p);
            SendSettingsGUI(p);
        }
        public void SendSettingsGUI(Player p)
        {
            MButton[] btns = [new MButton("Сохранить", $"save:{ActionMacros.RichList}", (args) => { Save(p, args.RichList); })];
            if (p.cid == 0)
            {
                btns = btns.Append(new MButton("Создать клан", $"clancreate", (args) => { Clan.OpenCreateWindow(p); })).ToArray();
            }
            p.win = new Window()
            {
                ShowTabs = true,
                Title = "НА СТРОЙКЕ",
                Tabs = [new Tab()
                {
                    Label = "Настройки",
                    Action = "settings",
                    InitialPage = new Page()
                    {
                        RichList = new RichListConfig()
                        {
                            Entries = [RichListEntry.DropDown("Масштаб интерфейса", "isca", ["мелко", "КРУПНО"], int.Parse(this["isca"])),
                                RichListEntry.DropDown("Масштаб территории", "tsca", ["мелко", "КРУПНО"], int.Parse(this["tsca"])),
                                RichListEntry.Bool("Включить управление мышкой", "mous", this["mous"].ToBool()),
                                RichListEntry.Bool("Упрощенный режим графики", "pot", this["pot"].ToBool()),
                                RichListEntry.Bool("ринудительно обновлять породы (увеличит потр. CPU)", "frc", this["frc"].ToBool()),
                                RichListEntry.Bool("CTRL переключает скорость робота (вместо удерживания)", "ctrl", this["ctrl"].ToBool()),
                                RichListEntry.Bool("Отключить ближайшие звуки", "mof", this["mof"].ToBool())
                            ]
                        },
                        Buttons = btns
                    }
                }]
            };
            p.SendWindow();
        }
    }
}
