using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.BotInfo;
using MinesServer.Network.Chat;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.Movement;
using MinesServer.Network.Programmator;
using MinesServer.Network.Tutorial;
using MinesServer.Network.World;
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
        public static void SendCrys(this Player p) => p.connection?.SendU(p.crys.BPacket);
        public static void SendHealth(this Player p) => p.connection?.SendU(new LivePacket(p.Health, p.MaxHealth));
        public static void SendBeep(this Player p) => p.connection?.SendU(new BibikaPacket());
        public static void SendBotInfo(this Player p) => p.connection?.SendU(new BotInfoPacket(p.name, p.x, p.y, p.id));
        public static void SendBotUpdateInfo(this Player p) => p.connection?.SendU(new NickListPacket(new Dictionary<int, string> { [p.id] = p.name }));
        public static void SendLvl(this Player p) => p.connection?.SendU(new LevelPacket(p.skillslist.lvlsummary()));
        public static void SendOnline(this Player p) => p.connection?.SendU(new OnlinePacket(DataBase.activeplayers.Count, 0));
        public static void SendInventory(this Player p) => p.connection?.SendU(p.inventory.BuildInventoryPacket());
        public static void SendConfig(this Player p) => p.connection?.SendU(new ConfigPacket("oldprogramformat+"));
        public static void SendSettings(this Player p) => p.connection?.SendU(p.settings.BuildSettingPacket());
        public static void ShowInfo(this Player p, string message, string title = "Ошибка") => p.connection?.SendU(new OKPacket(title, message));
        public static void SendLocalPrivateMessage(this Player p, int id, int x, int y, string message) => p.connection?.SendB(new HBPacket([new HBChatPacket(id, x, y, message)]));

        public static void SendSpeed(this Player p)
        {
            var _serverPause = p.ServerPause * 1.7;
            p.connection?.SendU(new SpeedPacket((int)(_serverPause), (int)(_serverPause * 0.80), 100000));
        }

        public static void SendSkills(this Player p, Dictionary<string, int> skillProgress)
        {
            if (skillProgress.Count == 0 || p.connection == null) return;
            p.connection?.SendU(new SkillsPacket(skillProgress));
        }
        public static void UpdateProg(this Player p, Program? prog)
        {
            if (prog == null) return;
            p.connection?.SendU(new UpdateProgrammatorPacket(prog.id, prog.name, prog.data));
        }

        public static void OpenProg(this Player p, Program? prog)
        {
            if (p.connection == null) return;

            var packet = prog == null ?
                new OpenProgrammatorPacket(-1, "Программатор", "_") :
                new OpenProgrammatorPacket(prog.id, prog.name, prog.data);

            p.connection.SendU(packet);
        }
        public static void SendChat(this Player p)
        {
            if (p.connection == null) return;

            p.currentchat ??= DataBase.GetChatWithMessages("FED");

            if (p.currentchat == null) return;

            p.connection.SendU(new CurrentChatPacket(p.currentchat.tag, p.currentchat.Name));

            var message = p.currentchat.GetMessages();

            if (message.Length > 0)
            {
                p.connection.SendU(new ChatMessagesPacket(p.currentchat.tag, message));
            }
        }

        public static void SendNickList(this Player p)
        {
            if (p.connection == null) return;
            var nicks = DataBase.activeplayers
                .ToDictionary(pl => pl.id, pl => pl.name);
            p.connection.SendU(new NickListPacket(nicks));
        }

        public static void SendOnlineCount(this Player p)
        {
            if (p.connection == null) return;
            int playerCount = DataBase.activeplayers.Count;
            int progCount = DataBase.activeplayers.Count(pl => pl.HasActiveProgram);
            p.connection.SendU(new OnlinePacket(playerCount, progCount));
        }

        public static void SendAgrStatus(this Player p)
        {
            if (p.connection == null) return;
            p.connection.SendU(new AgressionPacket(p.agression));
        }

        public static void SendAutoRemStatus(this Player p)
        {
            if (p.connection == null) return;
            p.connection.SendU(new AutoRemPacket(p.autoRem));
        }

        public static void SendHandModeStatus(this Player p)
        {
            // Режим зелёной руки в во время программатора
            if (p.connection == null) return;
            p.connection.SendU(new HandModePacket(p.handMode));
        }

        public static void SendAdminStatus(this Player p)
        {
            if (p.connection == null) return;
            p.connection.SendU(new AdminStatusPacket(p.isAdmin));
        }

        public static void SendDailyReward(this Player p)
        {
            if (p.connection == null) return;
            p.connection.SendU(new DailyRewardPacket(p.dailyRewardClaimed));
        }

        public static void SendMissionStatus(this Player p)
        {
            if (p.connection == null) return;
            // p.connection.SendU(new NaviArrowPacket(0, 0));
        }
    }
}
