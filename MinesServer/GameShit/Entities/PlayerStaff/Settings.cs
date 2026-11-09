using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Rich;
using MinesServer.GameShit.Sys_Clan;
using MinesServer.Network;
using MinesServer.Server;
using MinesServer.Utils;
using Newtonsoft.Json;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Settings
    {
        public Settings() { }

        [Key]
        public int id { get; set; }
        public string settings { get; set; } = "[]";

        public string this[string key]
        {
            get
            {
                _purchasedSettings = JsonConvert.
                    DeserializeObject<Dictionary<string, string>>(settings) ?? [];
                return _purchasedSettings[key];
            }
            set
            {
                using var db = new DataBase();
                _purchasedSettings[key] = value;
                db.SaveChanges();
            }
        }

        [NotMapped] private Dictionary<string, string> _purchasedSettings = new()
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
        };

        public void Init()
        {
            using var db = new DataBase();
            settings = JsonConvert.SerializeObject(_purchasedSettings);
            db.SaveChanges();
        }        

        public void Save(Player p, Dictionary<string, string> _settings)
        {
            settings = JsonConvert.SerializeObject(_settings);
            var db = new DataBase();
            db.Save(this, t => t.settings);

            p.SendSettings();
            SendSettingsGUI(p);
        }

        public SettingsPacket BuildSettingPacket()
        {
            _purchasedSettings = JsonConvert.
                DeserializeObject<Dictionary<string, string>>(settings) ?? [];
            return new SettingsPacket(_purchasedSettings);
        }

        public void SendSettingsGUI(Player p)
        {
            MButton[] btns = 
            [
                new MButton("Сохранить", $"save:{ActionMacros.RichList}", (args) => 
                {
                    if (args.RichList == null) return;
                    Save(p, args.RichList); 
                })
            ];

            if (p.cid == 0)
            {
                btns = btns.Append(new MButton("Создать клан", $"clancreate", (args) => { Clan.OpenCreateWindowAsync(p); })).ToArray();
            }

            p.win = new Window()
            {
                ShowTabs = true,
                Title = "Настройки клиента",
                Tabs = [new Tab()
                {
                    Label = "Настройки",
                    Action = "settings",
                    InitialPage = new Page()
                    {
                        RichList = new RichListConfig()
                        {
                            Entries = 
                            [
                                RichListEntry.DropDown("Масштаб интерфейса", "isca", ["мелко", "КРУПНО"], int.Parse(this["isca"])),
                                RichListEntry.DropDown("Масштаб территории", "tsca", ["мелко", "КРУПНО"], int.Parse(this["tsca"])),
                                RichListEntry.Bool("Включить управление мышкой", "mous", this["mous"].ToBool()),
                                RichListEntry.Bool("Упрощенный режим графики", "pot", this["pot"].ToBool()),
                                RichListEntry.Bool("Принудительно обновлять породы (увеличит потр. CPU)", "frc", this["frc"].ToBool()),
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
