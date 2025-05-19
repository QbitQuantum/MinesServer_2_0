using MinesServer.GameShit.Generator;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.BotInfo;
using MinesServer.Network.GUI;
using MinesServer.Network.Movement;
using MinesServer.Network.Programmator;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public static class pSenders
    {
        public static void SendGeo(this Player p)
        {
            if (p.geo.Count > 0)
            {
                p.connection?.SendU(new GeoPacket(World.GetProp(p.geo.Peek()).name));
                return;
            }
            p.connection?.SendU(new GeoPacket(""));
        }
        public static void SendWindow(this Player p)
        {
            if (p.win is not null)
            {
                p.connection?.SendU(new GUIPacket(p.win.ToString()));
                return;
            }
            p.connection?.SendU(new GuPacket());
        }
        public static void SendMoney(this Player p)
        {
            p.money = p.money < 0 ? 0 : p.money > long.MaxValue ? long.MaxValue : p.money;
            p.creds = p.creds < 0 ? 0 : p.creds > long.MaxValue ? long.MaxValue : p.creds;
            p.connection?.SendU(new MoneyPacket(p.money, p.creds));
        }
        public static void SendClan(this Player p)
        {
            if (p.cid == 0) p.connection?.SendU(new ClanHidePacket());
            else p.connection?.SendU(new ClanShowPacket(p.cid));
        }
        public static void UpdateProg(this Player p, Program prog) => p.connection?.SendU(new UpdateProgrammatorPacket(prog.id, prog.name, prog.data));
        public static void OpenProg(this Player p,Program prog) => p.connection?.SendU(new OpenProgrammatorPacket(prog.id, prog.name, prog.data));
        public static void ProgStatus(this Player p) => p.connection?.SendU(new ProgrammatorPacket(p.programsData.ProgRunning));
        public static void SendAutoDigg(this Player p) => p.connection?.SendU(new AutoDiggPacket(p.autoDig));
        public static void SendSpeed(this Player p) => p.connection?.SendU(new SpeedPacket((int)(p.pause * 5 * 1.4 / 1000 * 1.7), (int) (p.pause * 0.80 * 5 * 1.4 / 1000 * 1.7), 100000));
        public static void SendCrys(this Player p) => p.connection?.SendU(p.crys.BPacket);
        public static void SendHealth(this Player p) => p.connection?.SendU(new LivePacket(p.Health, p.MaxHealth));
        public static void Beep(this Player p) => p.connection?.SendU(new BibikaPacket());
        public static void SendBotInfo(this Player p) => p.connection?.SendU(new BotInfoPacket(p.name, p.x, p.y, p.id));
        public static void SendLvl(this Player p) => p.connection?.SendU(new LevelPacket(p.skillslist.lvlsummary()));
        public static void SendOnline(this Player p) => p.connection?.SendU(new OnlinePacket(MServer.Instance!.online, 0));
        public static void SendInventory(this Player p) => p.connection?.SendU(p.inventory.InvToSend());
    }
}
