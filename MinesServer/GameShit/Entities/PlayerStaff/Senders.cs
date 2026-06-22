using MinesServer.Enums;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.BotInfo;
using MinesServer.Network.Chat;
using MinesServer.Network.GUI;
using MinesServer.Network.Movement;
using MinesServer.Network.Programmator;
using MinesServer.Server;
using MinesServer.Server.Network;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public static class Senders
    {
        public static void SendGeo(this Player p)
        {
            var SkillGeology = p.skillslist.GetSkill(SkillType.Geology);
            if (SkillGeology != default)
            {
                var currentIndex = p.geo.Count;   // Текущая позиция (верхушка стека)
                var currentValue = currentIndex > 0 ? World.GetProp(p.geo.Peek()).name : ""; // Текущий элемент
                var maxCount = SkillGeology.Effect;       // Максимальное количество
                p.connection?.SendU(new GeoPacket($"{currentIndex}/{maxCount} {currentValue}"));
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
        public static void ProgStatus(this Player p) => p.connection?.SendU(new ProgrammatorPacket(p.programsData.ProgRunning));
        public static void SendAutoDigg(this Player p) => p.connection?.SendU(new AutoDiggPacket(p.autoDig));
        public static void SendSpeed(this Player p) => p.connection?.SendU(new SpeedPacket((int)(p.pause * 5 * 1.4 / 1000 * 1.7), (int) (p.pause * 0.80 * 5 * 1.4 / 1000 * 1.7), 100000));
        public static void SendCrys(this Player p) => p.connection?.SendU(p.crys.BPacket);
        public static void SendHealth(this Player p) => p.connection?.SendU(new LivePacket(p.Health, p.MaxHealth));
        public static void SendBeep(this Player p) => p.connection?.SendU(new BibikaPacket());
        public static void SendBotInfo(this Player p) => p.connection?.SendU(new BotInfoPacket(p.name, p.x, p.y, p.id));
        public static void SendLvl(this Player p) => p.connection?.SendU(new LevelPacket(p.skillslist.lvlsummary()));
        public static void SendOnline(this Player p) => p.connection?.SendU(new OnlinePacket(DataBase.activeplayers.Count, 0));
        public static void SendInventory(this Player p) => p.connection?.SendU(p.inventory.InvToSend());
        public static void SendConfig(this Player p) => p.connection?.SendU(new ConfigPacket("oldprogramformat+"));
        public static void SendSettings(this Player p) => p.settings.SendSettings(p);
        public static void UpdateProg(this Player p, Program? prog)
        {
            if (prog == null) return;
            p.connection?.SendU(new UpdateProgrammatorPacket(prog.id, prog.name, prog.data));
        }

        public static void OpenProg(this Player p, Program? prog)
        {
            if (prog == null) return;
            p.connection?.SendU(new OpenProgrammatorPacket(prog.id, prog.name, prog.data));
        }
        public static void SendChat(this Player p)
        {
            if (p.connection == null) return;

            using var db = new DataBase();
            p.currentchat ??= db.chats.FirstOrDefault(i => i.tag == "FED");

            if (p.currentchat == null) return;

            p.connection.SendU(new CurrentChatPacket(p.currentchat.tag, p.currentchat.Name));

            var msg = p.currentchat.GetMessages();

            if (msg.Length > 0)
            {
                p.connection.SendU(new ChatMessagesPacket(p.currentchat.tag, p.currentchat.GetMessages()));
            }
        }
    }
}
