﻿using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using MinesServer.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
namespace MinesServer.GameShit.Buildings
{
    public class Resp : Pack, IDamagable
    {
        #region fields
        public override PackType type => PackType.Resp;
        public float maxcharge { get; set; }
        public int cost { get; set; }
        public override int cid { get; set; }
        public long moneyinside { get; set; }
        public int hp { get; set; }
        public int maxhp { get; set; }
        public DateTime brokentimer { get; set; }
        #endregion
        private Resp(){ }
        public Resp(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            cost = 10;
            charge = 100;
            maxcharge = 1000;
            hp = 1000;
            maxhp = 1000;
            using var db = new DataBase();
            db.resps.Add(this);
            db.SaveChanges();
        }
        public void OnRespawn(Player p)
        {
            using var db = new DataBase();
            db.Attach(this);
            if (ownerid > 0)
            {
                if (p.money > cost)
                {
                    p.money -= cost;
                    moneyinside += cost;
                }
                else
                {
                    p.resp = null;
                    p.resp.OnRespawn(p);
                }
                if (charge > 0) charge--;
                else
                {
                    p.resp = null;
                    p.resp.OnRespawn(p);
                }
                p.SendMoney();
                World.W.GetChunk(x, y).ResendPack(this);
            }
            db.SaveChanges();
        }
        [NotMapped]
        public override int off
        {
            get => (charge > 0 ? 1 : 0);
        }
        public (int x, int y) GetRandompoint()
        {
            var r = new Random();
            return (r.Next(x + 2, x + 5), r.Next(y - 1, y + 3));
        }
        public void Fill(Player p, long num)
        {
            using var db = new DataBase();
            if (p.crys[CrystalType.Blue] < num)
            {
                num = p.crys[CrystalType.Blue];
            }
            db.Attach(this);
            if (p.crys.RemoveCrys((int)CrystalType.Blue, num))
            {
                charge += (int)num;
            }
            p.win?.CurrentTab.Replace(AdmnPage(p));
            p.SendWindow();
            db.SaveChanges();
        }
        #region affectworld
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x + 1, y, 37, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x, y + 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x + 1, y + 2, 106, true);
            World.SetCell(x - 1, y + 2, 106, true);
            World.SetCell(x, y + 2, 37, true);
            for (int xx = x + 2; xx < x + 6; xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35, true);
                }
            }
            base.Build();
        }
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x, y + 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x + 1, y + 2, 32, false);
            World.SetCell(x - 1, y + 2, 32, false);
            World.SetCell(x, y + 2, 32, false);
            for (int xx = x + 2; xx < x + 6; xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35, false);
                }
            }
        }
        public void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            foreach (var i in db.players.Include(p => p.resp))
            {
                var player = DataBase.GetPlayer(i.id);
                player.resp = null;
            }
            db.resps.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[1]++;
            }
        }
        #endregion
        public void AdminSaveChanges(Player p, Dictionary<string, string> d)
        {
            if (bool.TryParse(d["clan"], out var clan))
            {
                if (DataBase.GetPlayer(ownerid) != null)
                {
                    cid = clan ? DataBase.GetPlayer(ownerid).cid : 0;
                }
            }
            if (int.TryParse(d["cost"], out var costs) && costs >= 0 && costs <= 5000)
            {
                cost = costs;
            }
            if (int.TryParse(d["clanzone"], out var clanz))
            {
                clanzone = clanz;
            }
            p.win?.CurrentTab.Replace(AdmnPage(p));
        }
        public int clanzone { get; set; }
        private IPage AdmnPage(Player p)
        {
            MButton[] fillbuttons = [p.crys[CrystalType.Blue] >= 100 ? new MButton("+100", "fill:100", (args) => Fill(p, 100)) : new MButton("+100", "fill:100"),
                p.crys[CrystalType.Blue] >= 1000 ? new MButton("+1000", "fill:1000", (args) => Fill(p, 1000)) : new MButton("+1000", "fill:1000"),
                p.crys[CrystalType.Blue] >= 0 ? new MButton("max", "fill:max", (args) => Fill(p, (long)(maxcharge - charge))) : new MButton("max", "fill:max")
               ];
            return new Page()
            {
                Text = " ",
                RichList = new RichListConfig()
                {
                    Entries = [RichListEntry.Fill("заряд", (int)charge, (int)maxcharge, CrystalType.Blue, fillbuttons[0], fillbuttons[1], fillbuttons[2]),
                        RichListEntry.Text("hp"),
                        RichListEntry.UInt32("cost", "cost", (uint)cost),
                        RichListEntry.Button($"прибыль {moneyinside}$", moneyinside == 0 ? default : new MButton("Получить", "getprofit", (args) => { 
                            using var db = new DataBase(); p.money += moneyinside; moneyinside = 0; p.SendMoney(); db.SaveChanges(); p.win?.CurrentTab.Replace(AdmnPage(p)); p.SendWindow(); 
                        })),
                        RichListEntry.Bool("Клановый респ", "clan", cid > 0),
                        RichListEntry.UInt32("clanzone", "clanzone", (uint)clanzone)
                            ]
                },
                Buttons = [new MButton("СОХРАНИТЬ", $"save:{ActionMacros.RichList}", (args) => { AdminSaveChanges(p, args.RichList); })]
            };
        }
        public override Window? GUIWin(Player p)
        {
            Action adminaction = (p.id == ownerid) ? () =>
            {
                if (p.id == ownerid)
                {
                    p.win?.CurrentTab.Open(AdmnPage(p));
                }
            } : null;
            Page page = (p.resp.x != x && p.resp.y != y) ? new Page()
            {
                OnAdmin = adminaction,
                Text = $"@@Респ - это место, где будет появляться ваш робот\nпосле уничтожения (HP = 0)\n\nЦена восстановления: <color=green>${cost}</color>\n\n<color=#f88>Привязать робота к респу?</color>",
                Buttons = [new MButton("ПРИВЯЗАТЬ", "bind", (args) =>
                {
                    p.SetResp(this);
                    p.win = GUIWin(p)!;
                })]
            } : new Page()
            {
                OnAdmin = adminaction,
                Text = $"@@Респ - это место, где будет появляться ваш робот\nпосле уничтожения (HP = 0)\n\nЦена восстановления: <color=green>${cost}</color>\n\n<color=#8f8>Вы привязаны к этому респу.</color>",
                Buttons = []
            };

            return new Window()
            {
                Title = "РЕСП",
                Tabs = [
                    new Tab()
                    {
                        Label = "РЕСП",
                        Action = "resp",
                        InitialPage = page
                    }
                ]
            };
        }
    }
}
