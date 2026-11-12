using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Cards;
using MinesServer.GameShit.GUI.Inputs;
using MinesServer.GameShit.GUI.List;
using MinesServer.GameShit.GUI.Rich;
using MinesServer.GameShit.GUI.Styles;
using MinesServer.GameShit.GUI.UP;
using MinesServer.Server;

namespace MinesServer.GameShit.Sys_Clan
{
    internal static class ClanQueryExtensions
    {
        public static IQueryable<Clan> WithGraph(this IQueryable<Clan> q) => q
            .Include(c => c.members).ThenInclude(m => m.clanrank)
            .Include(c => c.ranks)
            .Include(c => c.reqs).ThenInclude(r => r.player);

        public static IQueryable<Clan> WithGraphLight(this IQueryable<Clan> q) => q
            .Include(c => c.members)
            .Include(c => c.reqs);
    }

    public class Clan
    {
        public const int MinNameLength = 3;
        public const int MaxNameLength = 32;
        public const int MaxAbbreviationLength = 3;
        public const int OwnerPriority = 100;
        public const int OfficerPriority = 20;

        private const int MaxClanIcons = 218;

        public Clan()
        {
        }

        #region fields
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public int ownerid { get; set; }
        public int lvl { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string abbreviation { get; set; } = string.Empty;
        public List<Request> reqs { get; set; } = [];
        public List<Player> members { get; set; } = [];
        public List<Rank> ranks { get; set; } = [];
        public string ViewTextClan => $"<color=white>[{abbreviation}] {name}</color>";

        #endregion

        public readonly record struct Permission(bool Allowed, string? Error)
        {
            public static Permission Ok => new(true, null);
            public static Permission Deny(string message) => new(false, message);
        }

        private bool IsMember(Player actor) =>
            actor.clanrank is not null && actor.clanrank.ClanId == id;

        private bool HasAtLeast(Player actor, int priority) =>
            IsMember(actor) && actor.clanrank!.priority >= priority;

        private bool CanManageRequests(Player actor) => HasAtLeast(actor, OfficerPriority);

        private bool CanAssignRank(Player actor, Rank rank) =>
            IsMember(actor) && rank.priority < actor.clanrank!.priority;

        private bool CanActOn(Player actor, Player target) =>
            IsMember(actor)
            && target.id != ownerid
            && target.id != actor.id
            && actor.clanrank!.Outranks(target.clanrank);

        private Permission CheckManageRequests(Player actor) =>
            CanManageRequests(actor)
                ? Permission.Ok
                : Permission.Deny("Недостаточно прав для управления заявками.");

        private Permission CheckActOn(Player actor, Player target)
        {
            if (!IsMember(actor))
                return Permission.Deny("Вы не состоите в этом клане.");

            if (target.id == ownerid)
                return Permission.Deny("Нельзя воздействовать на владельца клана.");

            if (target.id == actor.id)
                return Permission.Deny("Нельзя воздействовать на себя.");

            if (!actor.clanrank!.Outranks(target.clanrank))
                return Permission.Deny("Нельзя воздействовать на игрока с равным или более высоким званием.");

            return Permission.Ok;
        }

        private Permission CheckAssignRank(Player actor, Player target, Rank rank)
        {
            var actCheck = CheckActOn(actor, target);
            if (!actCheck.Allowed)
                return actCheck;

            if (!CanAssignRank(actor, rank))
                return Permission.Deny("Нельзя выдать звание выше или равное своему.");
            return Permission.Ok;
        }

        private bool HasPendingRequest(int playerId) =>
            reqs.Any(r => r.PlayerId == playerId && !r.IsExpired);

        public static Task OpenCreateWindowAsync(Player p)
        {
            var text = new StringBuilder()
                .AppendLine("@@")
                .AppendLine("Ура! Вы собираетесь создать новый клан. После создания клана вы сможете")
                .AppendLine("выполнять клановые квесты, создавать свои фермы, вести войны с другими")
                .AppendLine("кланами, защищать и отбивать территории, и многое другое.")
                .AppendLine()
                .AppendLine("Создание клана - ответственное действие, значок и название клана нельзя")
                .AppendLine("будет изменить позже. Поэтому внимательно подумайте над тем, как будет")
                .AppendLine("звучать и выглядеть ваш клан в игре.")
                .AppendLine()
                .AppendLine("Создание клана требует залога в 1000 кредитов.")
                .ToString();

            p.win = new Window
            {
                Tabs =
                [
                    new Tab
                    {
                        Label = "Создание клана",
                        Action = "clancreate:1",
                        InitialPage = new Page
                        {
                            Title = "Создание клана",
                            Text = text,
                            Buttons =
                            [
                                new MButton("Выбрать значок клана", "chooseicon",
                                    async _ => await ChooseIconStepAsync(p))
                            ]
                        }
                    }
                ]
            };
            p.SendWindow();
            return Task.CompletedTask;
        }

        public static async Task OpenListAsync(Player p)
        {
            static ClanListEntry C(int id, string name, string rightText) =>
                new ClanListEntry(
                    new MButton(name, $"clan{id}"),
                    (byte)id,
                    rightText);

            var entries = new[]
            {
                C(48,  "<color=white>The Masons</color> [XXX]",       "<color=#00FF00>Приём открыт</color> (29|21) ур.97"),
                C(165, "<color=white>Империя Гроксы</color> [IMG]",                "<color=#00FF00>Приём открыт</color> (19|13) ур.92"),
                C(126, "<color=white>Euphoria</color> [HOR]",         "<color=#00FF00>Приём открыт</color> (7|7) ур.62"),
                C(62,  "<color=white>ANVIL</color> [ANV]",            "<color=#00FF00>Приём открыт</color> (10|8) ур.60"),
                C(43,  "<color=white>НЕНАВИСТЬ!</color> [N]",                  "<color=#00FF00>Приём открыт</color> (4|4) ур.53"),
                C(160, "<color=white>Цитадель</color> [ZTD]",                 "<color=#00FF00>Приём открыт</color> (16|16) ур.45"),
                C(100, "<color=white>Winx</color> [WNX]",             "<color=#00FF00>Приём открыт</color> (11|5) ур.43"),
                C(89,  "<color=white>Самый Лучший Клан</color> [TOP]","<color=#00FF00>Приём открыт</color> (16|10) ур.42"),
                C(7,   "<color=white>Бригада котиков</color> [RPG]",                "<color=#00FF00>Приём открыт</color> (8|68) ур.30"),
                C(131, "<color=white>КГБ</color> [КГБ]",                    "<color=#00FF00>Приём открыт</color> (7|4) ур.29"),
                C(18,  "<color=white>РОЙ</color> [ROY]",                "<color=#00FF00>Приём открыт</color> (0|1) ур.25"),
                C(19,  "<color=white>Nvidia power</color> [NVP]",     "<color=#00FF00>Приём открыт</color> (3|1) ур.19"),
                C(83,  "<color=white>Steel Brotherhood</color> [SB]", "<color=#00FF00>Приём открыт</color> (13|4) ур.18"),
                C(184, "<color=white>Wild Cats</color> [WLD]",        "<color=#00FF00>Приём открыт</color> (4|3) ур.14"),
                C(68,  "<color=white>Клан друзей FAISSa</color> [FIS]",          "<color=#00FF00>Приём открыт</color> (8|1) ур.14"),
                C(6,   "<color=white>А мы не ангелы парень!</color> [NIG]",            "<color=#00FF00>Приём открыт</color> (11|5) ур.12"),
                C(94,  "<color=white>ЗЕМЛЯ</color> [ZEM]",                 "<color=#00FF00>Приём открыт</color> (1|2) ур.12"),
                C(77,  "<color=white>The anarchists</color> [ANX]",   "<color=#00FF00>Приём открыт</color> (1|1) ур.9"),
                C(157, "<color=white>Инферно</color> [INF]",                 "<color=#00FF00>Приём открыт</color> (2|1) ур.8"),
                C(36,  "<color=white>sleeping knights</color> [SK]",  "<color=#00FF00>Приём открыт</color> (0|1) ур.4"),
                C(208, "<color=white>Faceless</color> [FCK]",         "<color=#00FF00>Приём открыт</color> (2|2) ур.2"),
            };

            static RichListEntry B(string rightText, string buton_name, string action) =>
                RichListEntry.Button(rightText, new MButton(buton_name, action));

            var type_upgrade = new[]
            {
                B("Топ по <color=white>Добыче</color>", "Посмотреть", "top_clan_dig"),
                B("Топ по <color=white>Валюте</color>", "Посмотреть", "top_clan_money"),
                B("Топ по <color=white>Кредитам</color>", "Посмотреть", "top_clan_creds"),
                B("Топ по <color=white>Уровню</color>", "Посмотреть", "top_clan_lvl"),
                B("Топ по <color=white>Живым Кристаллам</color>", "Посмотреть", "top_clan_alive_crystall"),
                B("Топ по <color=white>ОПП</color>", "Посмотреть", "top_clan_opp"),
                B("Топ по <color=white>Очкам Федерации</color>", "Посмотреть", "top_clan_point_federation"),
            };

            p.win = new Window
            {
                Title = "Кланы",
                ShowTabs = true,
                Tabs =
                [
                    new Tab
                    {
                        Label = "ОБЗОР",
                        Action = "clanstab",
                        InitialPage = new Page
                        {
                            Text = "@@Кланы шахт. Кликните на клан для подробной информации",
                            ClanList = entries,
                            Input = new InputConfig("Поиск", 50, IsConsole: true),
                            Buttons = []
                        }
                    },
                    new Tab
                    {
                        Label = "ДОСТИЖЕНИЕ КЛАНОВ",
                        Action = "achievementsclanstab",
                        InitialPage = new Page
                        {
                            Style = new Style()
                            {
                                Space = 1,
                            },
                            RichList = new(type_upgrade, false),
                            Buttons = []
                        }
                    },
                    new Tab
                    {
                        Label = "ДИПЛОМАТИЯ",
                        Action = "diplomacytab",
                        InitialPage = new Page
                        {
                            Buttons = []
                        }
                    }
                ]
            };
            p.SendWindow();
        }

        public static async Task OpenWinAsync(Player p)
        {
            if (p.clan is null)
                return;

            await using var db = new DataBase();
            var clan = await db.clans.WithGraph()
                .FirstOrDefaultAsync(c => c.id == p.clan.id);
            if (clan is null)
                return;

            var tabs = new List<Tab>
            {
                BuildOverviewTab(clan),
                BuildListTab(p, clan)
            };

            if (clan.reqs.Count > 0 && clan.CanManageRequests(p))
                tabs.Add(BuildRequestsTab(p, clan));

            p.win = new Window { Title = clan.name, ShowTabs = true, Tabs = tabs.ToArray() };
            p.SendWindow();
        }

        private static async Task OpenPreviewAsync(Player p, int clanId)
        {
            await using var db = new DataBase();
            var clan = await db.clans.WithGraphLight().AsNoTracking()
                .FirstOrDefaultAsync(c => c.id == clanId);

            if (clan is null) return;

            var text = string.Empty;
            MButton[] buttons = [];

            if (clan.HasPendingRequest(p.id))
            {
                text = "\nЗаявка уже подана.";
            }
            else if (p.clan is null)
            {
                buttons =
                [
                    new MButton("Подать заявку", "reqin",
                        async _ => await SubmitRequestAsync(p, clan))
                ];
            }

            var card = new StringBuilder()
                .Append("<color=white>")
                .Append(clan.name)
                .Append("</color>")
                .AppendLine()
                .Append("Участники: <color=white>")
                .Append(clan.members.Count)
                .Append("</color>")
                .ToString();

            p.win?.CurrentTab.Open(new Page
            {
                Text = text,
                Title = "Кланы",
                Card = new Card(CardImageType.Clan, clan.id.ToString(), card),
                Buttons = buttons
            });
            p.SendWindow();
        }

        private static Tab BuildOverviewTab(Clan clan)
        {
            var cardText = new StringBuilder()
                .Append(clan.ViewTextClan)
                .AppendLine()
                .Append("Уровень клана: <color=white>")
                .Append(clan.lvl)
                .Append("</color>")
                .AppendLine()
                .Append("Участники: <color=white>")
                .Append(clan.members.Count)
                .Append("</color>")
                .ToString();

            var text = new StringBuilder()
                .Append("@@")
                .Append(ClanMissions.GetMission(0))
                .AppendLine()
                .AppendLine()
                .AppendLine("СООБЩЕНИЕ ДЛЯ СОКЛАНОВ:")
                .AppendLine("<color=white>Руководство еще не оставило сообщение для сокланов!</color>")
                .ToString();

            return new Tab
            {
                Action = "view",
                Label = "Обзор",
                InitialPage = new Page
                {
                    Card = new Card(CardImageType.Clan, clan.id.ToString(), cardText),
                    Text = text,
                    Buttons = []
                }
            };
        }

        private static Tab BuildListTab(Player p, Clan clan)
        {
            var memberEntries = clan.members
                .Select(m => new ClanListEntry(
                    new MButton(
                        $"<color={m.clanrank?.colorhex}>{m.name}</color> - {m.clanrank?.name}",
                        $"listrow:{m.id}",
                        async _ => await OpenPlayerPreviewAsync(p, m.id)),
                    0,
                    m.online ? "онлайн" : "оффлайн"))
                .ToArray();

            return new Tab
            {
                Action = "list",
                Label = "Список",
                InitialPage = new Page
                {
                    ClanList = memberEntries,
                    Buttons =
                    [
                        new MButton("Покинуть клан", "leave",
                            async _ => await LeaveAsync(p))
                    ]
                }
            };
        }

        private static Tab BuildRequestsTab(Player actor, Clan clan)
        {
            var requestEntries = clan.reqs
                .Select((r, i) => new ListEntry(
                    $"{i + 1}.<color=white>{r.player.name}</color>",
                    new MButton("...", $"openreq:{r.PlayerId}",
                        async _ => await OpenRequestAsync(actor, r.id))))
                .ToArray();

            return new Tab
            {
                Action = "reqs",
                Label = "Заявки",
                InitialPage = new Page
                {
                    List = requestEntries,
                    Buttons = []
                }
            };
        }

        private static async Task OpenPlayerPreviewAsync(Player p, int targetId, bool changeRank = false)
        {
            if (p.clan is null)
                return;

            await using var db = new DataBase();
            var player = await db.players.Include(x => x.clanrank).AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == targetId);
            if (player is null)
                return;

            var clan = await db.clans.Include(c => c.ranks).AsNoTracking()
                .FirstOrDefaultAsync(c => c.id == p.clan.id);
            if (clan is null)
                return;

            var canAct = clan.CanActOn(p, player);

            var buttons = new List<MButton>
            {
                new("Прокачка", "skills", async _ => await OpenSkillsAsync(p, targetId))
            };

            RichListEntry[] rich = [];
            if (changeRank && canAct)
            {
                var available = clan.ranks
                    .Where(r => r.priority < p.clanrank!.priority)
                    .ToArray();

                rich =
                [
                    RichListEntry.DropDown("изменение звания", "changerankd",
                        available.Select(r => r.name).ToArray(), 0)
                ];

                buttons.Add(new MButton("Применить", "saverank",
                    async args =>
                    {
                        if (!int.TryParse(args.Input, out var idx)
                            || idx < 0 || idx >= available.Length)
                        {
                            p.ShowInfo("Некорректно выбрано звание.");
                            return;
                        }
                        await ChangeRankAsync(p, player, available[idx]);
                    }));
                buttons.Add(new MButton("Исключить", "kick",
                    async _ => await KickAsync(p, player)));
            }
            else if (canAct)
            {
                buttons.Add(new MButton("Изменить звание", "changerank",
                    async _ => await OpenPlayerPreviewAsync(p, targetId, true)));
            }

            var text = new StringBuilder()
                .AppendLine("@@Профиль соклана")
                .AppendLine()
                .Append("Имя: <color=")
                .Append(player.clanrank?.colorhex)
                .Append(">")
                .Append(player.name)
                .Append("</color>")
                .AppendLine()
                .Append("Звание: ")
                .Append(player.clanrank?.name)
                .AppendLine()
                .Append("ID: <color=white>")
                .Append(player.id)
                .Append("</color>")
                .ToString();

            p.win?.CurrentTab.Open(new Page
            {
                RichList = new RichListConfig(rich, true),
                Text = text,
                Buttons = buttons.ToArray()
            });
            p.SendWindow();
        }

        private static async Task OpenSkillsAsync(Player p, int targetId)
        {
            await using var db = new DataBase();
            var target = await db.players.Include(x => x.skillslist).AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == targetId);
            if (target is null)
                return;

            var text = new StringBuilder()
                .AppendLine("Просмотр скиллов игрока")
                .AppendLine()
                .Append("<color=white>")
                .Append(target.name)
                .Append("</color>")
                .AppendLine()
                .Append("ID <color=white>")
                .Append(target.id)
                .Append("</color>")
                .AppendLine()
                .Append("Общий уровень: <color=white>")
                .Append(target.skillslist.lvlsummary())
                .Append("</color>")
                .ToString();

            p.win?.CurrentTab.Open(new UpPage
            {
                Skills = target.skillslist.GetSkills(),
                SlotAmount = target.skillslist.slots,
                SkillsToInstall = [],
                Text = text
            });
            p.SendWindow();
        }

        private static async Task OpenRequestAsync(Player actor, int requestId)
        {
            if (actor.clan is null)
                return;

            await using var db = new DataBase();
            var request = await db.reqs.Include(r => r.player).AsNoTracking()
                .FirstOrDefaultAsync(r => r.id == requestId && r.ClanId == actor.clan.id);
            if (request is null || request.player is null)
                return;

            var text = new StringBuilder()
                .AppendLine("@@Заявка на прием в клан:")
                .AppendLine()
                .Append("Имя: <color=white>")
                .Append(request.player.name)
                .Append("</color>")
                .AppendLine()
                .Append("ID <color=white>")
                .Append(request.player.id)
                .Append("</color>")
                .AppendLine()
                .Append("Истекает через: ")
                .Append(request.TimeLeft.ToString(@"hh\:mm"))
                .ToString();

            actor.win = new Window
            {
                Title = "Заявка в клан",
                Tabs =
                [
                    new Tab
                    {
                        Action = "req",
                        Label = "req",
                        InitialPage = new Page
                        {
                            Text = text,
                            Buttons =
                            [
                                new MButton("Принять", "accept",
                                    async _ => await AcceptRequestAsync(actor, request)),
                                new MButton("Отклонить", "decline",
                                    async _ => await DeclineRequestAsync(actor, request)),
                                new MButton("Прокачка", "openskills",
                                    async _ => await OpenSkillsAsync(actor, request.PlayerId))
                            ]
                        }
                    }
                ]
            };
            actor.SendWindow();
        }

        private static async Task SubmitRequestAsync(Player p, Clan clan)
        {
            if (p.clan is not null)
            {
                p.ShowInfo("Вы уже состоите в клане.");
                return;
            }
            if (clan.HasPendingRequest(p.id))
            {
                p.ShowInfo("Заявка уже подана.");
                return;
            }

            var saved = await DataBase.SaveClanRequestAsync(clan.id, p.id);
            if (!saved)
            {
                p.ShowInfo("Клан не найден или заявка уже подана.");
                return;
            }

            await OpenPreviewAsync(p, clan.id);
        }

        private static async Task AcceptRequestAsync(Player p, Request request)
        {
            var perm = p.clan!.CheckManageRequests(p);
            if (!perm.Allowed)
            {
                p.ShowInfo(perm.Error!);
                return;
            }

            var ok = await DataBase.SaveAcceptRequestAsync(request.id);
            if (!ok)
            {
                p.ShowInfo("Заявка недействительна или игрок уже в клане.");
                return;
            }

            await OpenWinAsync(p);
        }

        private static async Task DeclineRequestAsync(Player p, Request request)
        {
            var perm = p.clan!.CheckManageRequests(p);
            if (!perm.Allowed)
            {
                p.ShowInfo(perm.Error!);
                return;
            }

            var ok = await DataBase.SaveDeclineRequestAsync(request.id);
            if (!ok)
            {
                p.ShowInfo("Заявка не найдена.");
                return;
            }

            await OpenWinAsync(p);
        }

        private static async Task LeaveAsync(Player p)
        {
            if (p.clan is null)
                return;

            var clanId = p.clan.id;
            await DataBase.SaveLeaveClanAsync(clanId, p.id);

            p.clan = null;
            p.clanrank = null;
            p.BotsRender();
            p.win = null;
            p.SendClan();
            p.SendWindow();
        }

        private static async Task KickAsync(Player p, Player target)
        {
            var perm = p.clan!.CheckActOn(p, target);
            if (!perm.Allowed)
            {
                p.ShowInfo(perm.Error!);
                return;
            }

            await DataBase.SaveKickAsync(target.id);

            await OpenWinAsync(p);
        }

        private static async Task ChangeRankAsync(Player p, Player target, Rank newRank)
        {
            var perm = p.clan!.CheckAssignRank(p, target, newRank);
            if (!perm.Allowed)
            {
                p.ShowInfo(perm.Error!);
                return;
            }

            await DataBase.SaveChangeRankAsync(target.id, newRank.id);

            await OpenPlayerPreviewAsync(p, target.id);
        }

        private static async Task ChooseIconStepAsync(Player p)
        {
            await using var db = new DataBase();
            var taken = (await db.clans.AsNoTracking().Select(c => c.id).ToListAsync()).ToHashSet();

            var free = Enumerable.Range(1, MaxClanIcons)
                .Where(id => !taken.Contains(id))
                .ToArray();

            if (free.Length == 0)
            {
                p.ShowInfo("Свободных значков клана не осталось.");
                return;
            }

            var entries = free
                .Select(id => new ClanListEntry(
                    new MButton($"Значок #{id}", $"pickicon:{id}",
                        async _ => await NameStepAsync(p, id)),
                    (byte)id,
                    "<color=#00FF00>свободен</color>"))
                .ToArray();

            var text = new StringBuilder()
                .AppendLine("@@")
                .AppendLine("Выберите свободный значок клана.")
                .Append("Свободно: <color=white>")
                .Append(free.Length)
                .Append("</color> из ")
                .Append(MaxClanIcons)
                .AppendLine(".")
                .ToString();

            p.win?.CurrentTab.Open(new Page
            {
                Title = "Выбор значка клана",
                Text = text,
                ClanList = entries,
                Buttons = []
            });
            p.SendWindow();
        }

        private static Task NameStepAsync(Player p, int iconId)
        {
            p.win?.CurrentTab.Open(new Page
            {
                Title = "Выбор названия клана",
                Text = "@@\nВыберите название клана.\n",
                Input = new InputConfig
                {
                    IsConsole = false,
                    Placeholder = "Наименование клана",
                    MaxLength = MaxNameLength
                },
                Buttons =
                [
                    new MButton("Продолжить", $"namechoose:{ActionMacros.Input}",
                        async args => await AbbreviationStepAsync(p, iconId, args.Input!))
                ]
            });
            p.SendWindow();
            return Task.CompletedTask;
        }

        private static Task AbbreviationStepAsync(Player p, int iconId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                p.ShowInfo("Название клана не может быть пустым.");
                return Task.CompletedTask;
            }

            var cardText = new StringBuilder()
                .Append("<color=white>")
                .Append(name)
                .Append("</color>")
                .AppendLine()
                .ToString();

            p.win?.CurrentTab.Open(new Page
            {
                Title = "Создание аббревиатуры",
                Text = "@@\nВыберите краткое имя клана...\n",
                Card = new Card(CardImageType.Clan, iconId.ToString(), cardText),
                Input = new InputConfig
                {
                    IsConsole = false,
                    Placeholder = "Аббревиатура: [XYZ]",
                    MaxLength = MaxAbbreviationLength
                },
                Buttons =
                [
                    new MButton("Далее", $"next:{ActionMacros.Input}",
                        async args => await ConfirmStepAsync(p, iconId, name, args.Input!))
                ]
            });
            p.SendWindow();
            return Task.CompletedTask;
        }

        private static Task ConfirmStepAsync(Player p, int iconId, string name, string abbr)
        {
            if (string.IsNullOrWhiteSpace(abbr))
            {
                p.ShowInfo("Аббревиатура не может быть пустой.");
                return Task.CompletedTask;
            }

            var cardText = new StringBuilder()
                .Append("<color=white>")
                .Append(name)
                .Append('[')
                .Append(abbr)
                .Append("]</color>")
                .AppendLine()
                .ToString();

            var text = new StringBuilder()
                .AppendLine("@@")
                .AppendLine("Все готово для создания клана. После его создания, изменить название клана будет невозможно.")
                .AppendLine("После выхода вам вернется 90% его стоимости, а клан расформирован.")
                .ToString();

            p.win?.CurrentTab.Open(new Page
            {
                Title = "Завершение создания клана",
                Text = text,
                Card = new Card(CardImageType.Clan, iconId.ToString(), cardText),
                Buttons =
                [
                    new MButton("<color=#ff8888ff>Принимаю условия</color>", "complete",
                        async _ => await CreateAsync(p, iconId, name, abbr))
                ]
            });
            p.SendWindow();
            return Task.CompletedTask;
        }

        private static async Task CreateAsync(Player p, int iconId, string name, string abbr)
        {
            name = name.Trim();
            abbr = abbr.Trim().ToUpperInvariant();

            if (name.Length < MinNameLength || name.Length > MaxNameLength)
            {
                p.ShowInfo($"Название клана должно быть от {MinNameLength} до {MaxNameLength} символов.");
                return;
            }
            if (abbr.Length is < 1 or > MaxAbbreviationLength)
            {
                p.ShowInfo($"Аббревиатура должна быть от 1 до {MaxAbbreviationLength} символов.");
                return;
            }
            if (!abbr.All(char.IsAsciiLetterUpper))
            {
                p.ShowInfo("Аббревиатура должна состоять только из заглавных латинских букв.");
                return;
            }

            var clan = await DataBase.SaveCreateClanAsync(iconId, name, abbr, p.id);
            if (clan is null)
            {
                p.ShowInfo("Не удалось создать клан (значок, имя заняты или вы уже в клане).");
                return;
            }

            p.clan = clan;
            p.SendClan();
            p.win = null;
            p.SendWindow();

            var message = new StringBuilder()
                .Append("Удачи в клановой игре. Помни: клан — это не один игрок, а семья. ")
                .Append("Держимся вместе, выручаем друг друга и получаем от игры удовольствие.")
                .ToString();

            p.ShowInfo(message, "Клан успешно создан!");
        }
    }
}
