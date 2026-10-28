using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.GUI.UP;
using MinesServer.Network.GUI;
using MinesServer.Server;

namespace MinesServer.GameShit.ClanSystem
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

        public Clan()
        {
        }

        #region fields
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int id { get; set; }
        public int ownerid { get; set; }
        public int lvl { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string abbreviation { get; set; } = string.Empty;
        public List<Request> reqs { get; set; } = [];
        public List<Player> members { get; set; } = [];
        public List<Rank> ranks { get; set; } = [];
        public string ViewTextClan => $"<color=white>[{abbreviation}] {name}</color>";

        private Rank DefaultRank =>
            ranks.OrderBy(r => r.priority).FirstOrDefault()
            ?? throw new InvalidOperationException($"У клана {id} отсутствуют звания.");

        private Rank? OwnerRank => ranks.FirstOrDefault(r => r.priority == OwnerPriority);
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
            if (!actCheck.Allowed) return actCheck;
            if (!CanAssignRank(actor, rank))
                return Permission.Deny("Нельзя выдать звание выше или равное своему.");
            return Permission.Ok;
        }

        private bool HasPendingRequest(int playerId) =>
            reqs.Any(r => r.PlayerId == playerId && !r.IsExpired);

        private static void ShowInfo(Player p, string message, string title = "Ошибка")
        {
            p.connection?.SendU(new OKPacket(title, message));
        }

        public static Task OpenCreateWindowAsync(Player p)
        {
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
                            Text = "@@\nУра! Вы собираетесь создать новый клан. После создания клана вы сможете\n" +
                                   "выполнять клановые квесты, создавать свои фермы, вести войны с другими\n" +
                                   "кланами, защищать и отбивать территории, и многое другое.\n\n" +
                                   "Создание клана - ответственное действие, значок и название клана нельзя\n" +
                                   "будет изменить позже. Поэтому внимательно подумайте над тем, как будет\n" +
                                   "звучать и выглядеть ваш клан в игре.\n\n" +
                                   "Создание клана требует залога в 1000 кредитов.\n",
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
            // await using var db = new DataBase();
            // var clans = await db.clans.WithGraphLight().AsNoTracking().ToListAsync();

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

            p.win = new Window
            {
                Title = "Кланы",
                ShowTabs = true,
                Tabs =
                [
                    new Tab
                    {
                        Label = "ОБЗОР",
                        Action = "clans",
                        InitialPage = new Page
                        {
                            Text = "@@Кланы шахт. Кликните на клан для подробной информации",
                            ClanList = entries,
                            Input = new InputConfig("Поиск", 50, IsConsole: true),
                            Buttons = Array.Empty<MButton>()
                        }
                    },
                    new Tab
                    {
                        Label = "ДОСТИЖЕНИЕ КЛАНОВ",
                        Action = "clanlistt",
                        InitialPage = new Page { Buttons = Array.Empty<MButton>() }
                    },
                    new Tab
                    {
                        Label = "ДИПЛОМАТИЯ",
                        Action = "diplomacytab",
                        InitialPage = new Page { Buttons = Array.Empty<MButton>() }
                    }
                ]
            };
            p.SendWindow();
        }

        public static async Task OpenWinAsync(Player p)
        {
            if (p.clan is null) return;

            await using var db = new DataBase();
            var clan = await db.clans.WithGraph()
                .FirstOrDefaultAsync(c => c.id == p.clan.id);
            if (clan is null) return;

            var tabs = new List<Tab> { BuildOverviewTab(clan), BuildListTab(p, clan) };

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
                text = "\nЗаявка уже подана.";
            else if (p.clan is null)
                buttons = [new MButton("Подать заявку", "reqin",
                async _ => await SubmitRequestAsync(p, clanId))];

            p.win?.CurrentTab.Open(new Page
            {
                Text = text,
                Title = "Кланы",
                Card = new Card(CardImageType.Clan, clan.id.ToString(),
                    $"<color=white>{clan.name}</color>\nУчастники: <color=white>{clan.members.Count}</color>"),
                Buttons = buttons
            });
            p.SendWindow();
        }

        private static Tab BuildOverviewTab(Clan clan) => new()
        {
            Action = "view",
            Label = "Обзор",
            InitialPage = new Page
            {
                Card = new Card(CardImageType.Clan, clan.id.ToString(),
                    clan.ViewTextClan + "\n" +
                    $"Уровень клана: <color=white>{clan.lvl}</color>\n" +
                    $"Участники: <color=white>{clan.members.Count}</color>\n"),
                Text = "@@" + ClanMissions.GetMission(0) +
                "\n\nСООБЩЕНИЕ ДЛЯ СОКЛАНОВ:\n" +
                       "<color=white>Руководство еще не оставило сообщение для сокланов!</color>\n",
                Buttons = []
            }
        };

        private static Tab BuildListTab(Player p, Clan clan) => new()
        {
            Action = "list",
            Label = "Список",
            InitialPage = new Page
            {
                ClanList = clan.members.Select(m => new ClanListEntry(
                    new MButton(
                        $"<color={m.clanrank?.colorhex}>{m.name}</color> - {m.clanrank?.name}",
                        $"listrow:{m.id}",
                        async _ => await OpenPlayerPreviewAsync(p, m.id)),
                    0,
                    m.online ? "онлайн" : "оффлайн")).ToArray(),
                Buttons = [new MButton("Покинуть клан", "leave",
                async _ => await LeaveAsync(p))]
            }
        };

        private static Tab BuildRequestsTab(Player actor, Clan clan) => new()
        {
            Action = "reqs",
            Label = "Заявки",
            InitialPage = new Page
            {
                List = clan.reqs.Select((r, i) => new ListEntry(
                    $"{i + 1}.<color=white>{r.player.name}</color>",
                    new MButton("...", $"openreq:{r.PlayerId}",
                        async _ => await OpenRequestAsync(actor, r.id)))).ToArray(),
                Buttons = []
            }
        };

        private static async Task OpenPlayerPreviewAsync(Player p, int targetId, bool changeRank = false)
        {
            if (p.clan is null) return;

            await using var db = new DataBase();
            var target = await db.players.Include(x => x.clanrank).AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == targetId);
            if (target is null) return;

            var clan = await db.clans.Include(c => c.ranks).AsNoTracking()
                .FirstOrDefaultAsync(c => c.id == p.clan.id);
            if (clan is null) return;

            var canAct = clan.CanActOn(p, target);

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
                            ShowInfo(p, "Некорректно выбрано звание.");
                            return;
                        }
                        await ChangeRankAsync(p, targetId, available[idx].id);
                    }));
                buttons.Add(new MButton("Исключить", "kick", async _ => await KickAsync(p, targetId)));
            }
            else if (canAct)
            {
                buttons.Add(new MButton("Изменить звание", "changerank",
                    async _ => await OpenPlayerPreviewAsync(p, targetId, true)));
            }

            p.win?.CurrentTab.Open(new Page
            {
                RichList = new RichListConfig(rich, true),
                Text = $"@@Профиль соклана\n\n" +
                       $"Имя: <color={target.clanrank?.colorhex}>{target.name}</color>\n" +
                       $"Звание: {target.clanrank?.name}\n" +
                       $"ID: <color=white>{target.id}</color>",
                Buttons = buttons.ToArray()
            });
            p.SendWindow();
        }

        private static async Task OpenSkillsAsync(Player p, int targetId)
        {
            await using var db = new DataBase();
            var target = await db.players.Include(x => x.skillslist).AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == targetId);
            if (target is null) return;

            p.win?.CurrentTab.Open(new UpPage
            {
                Skills = target.skillslist.GetSkills(),
                SlotAmount = target.skillslist.slots,
                SkillsToInstall = [],
                Text = $"Просмотр скиллов игрока\n\n<color=white>{target.name}</color>\n" +
                       $"ID <color=white>{target.id}</color>\n" +
                       $"Общий уровень: <color=white>{target.skillslist.lvlsummary()}</color>"
            });
            p.SendWindow();
        }

        private static async Task OpenRequestAsync(Player actor, int requestId)
        {
            if (actor.clan is null) return;

            await using var db = new DataBase();
            var request = await db.reqs.Include(r => r.player).AsNoTracking()
                .FirstOrDefaultAsync(r => r.id == requestId && r.ClanId == actor.clan.id);
            if (request is null || request.player is null) return;

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
                            Text = $"@@Заявка на прием в клан:\n\n" +
                                   $"Имя: <color=white>{request.player.name}</color>\n" +
                                   $"ID <color=white>{request.player.id}</color>\n" +
                                   $"Истекает через: {request.TimeLeft:hh\\:mm}",
                            Buttons =
                            [
                                new MButton("Принять", "accept",
                                    async _ => await AcceptRequestAsync(actor, requestId)),
                                new MButton("Отклонить", "decline",
                                    async _ => await DeclineRequestAsync(actor, requestId)),
                                new MButton("Прокачка", "openskills",
                                    async _ => await OpenSkillsAsync(actor, request.PlayerId))
                            ]
                        }
                    }
                ]
            };
            actor.SendWindow();
        }
        private static async Task SubmitRequestAsync(Player p, int clanId)
        {
            if (p.clan is not null)
            {
                ShowInfo(p, "Вы уже состоите в клане.");
                return;
            }

            await using var db = new DataBase();
            var clan = await db.clans.Include(c => c.reqs).FirstOrDefaultAsync(c => c.id == clanId);
            if (clan is null)
            {
                ShowInfo(p, "Клан не найден.");
                return;
            }
            if (clan.HasPendingRequest(p.id))
            {
                ShowInfo(p, "Заявка уже подана.");
                return;
            }

            var saved = await DataBase.SaveClanRequestAsync(clanId, p.id);
            if (!saved)
            {
                ShowInfo(p, "Клан не найден.");
                return;
            }

            await OpenPreviewAsync(p, clanId);
        }

        private static async Task AcceptRequestAsync(Player actor, int requestId)
        {
            await using var db = new DataBase();
            var clan = await db.clans.WithGraph()
                .FirstOrDefaultAsync(c => c.id == actor.clan!.id);
            if (clan is null)
            {
                ShowInfo(actor, "Клан не найден.");
                return;
            }

            var perm = clan.CheckManageRequests(actor);
            if (!perm.Allowed)
            {
                ShowInfo(actor, perm.Error!);
                return;
            }

            var request = clan.reqs.FirstOrDefault(r => r.id == requestId);
            if (request is null || request.IsExpired || request.player.clan is not null)
            {
                ShowInfo(actor, "Заявка недействительна или игрок уже в клане.");
                return;
            }

            await DataBase.SaveAcceptRequestAsync(clan.id, requestId, clan.DefaultRank);

            await OpenWinAsync(actor);
        }

        private static async Task DeclineRequestAsync(Player actor, int requestId)
        {
            await using var db = new DataBase();
            var clan = await db.clans.WithGraph()
                .FirstOrDefaultAsync(c => c.id == actor.clan!.id);
            if (clan is null)
            {
                ShowInfo(actor, "Клан не найден.");
                return;
            }

            var perm = clan.CheckManageRequests(actor);
            if (!perm.Allowed)
            {
                ShowInfo(actor, perm.Error!);
                return;
            }

            var req = clan.reqs.FirstOrDefault(r => r.id == requestId);
            if (req is null)
            {
                ShowInfo(actor, "Заявка не найдена.");
                return;
            }

            await DataBase.SaveDeclineRequestAsync(clan.id, requestId);

            await OpenWinAsync(actor);
        }

        private static async Task LeaveAsync(Player p)
        {
            if (p.clan is null) return;

            await using var db = new DataBase();
            var clan = await db.clans.WithGraph()
                .FirstOrDefaultAsync(c => c.id == p.clan.id);

            if (clan is null)
            {
                p.clan = null;
                p.clanrank = null;
                p.SendClan();
                return;
            }

            await DataBase.SaveLeaveClanAsync(clan.id, p.id, clan.OwnerRank);

            p.clan = null;
            p.clanrank = null;
            p.BotsRender();
            p.win = null;
            p.SendClan();
            p.SendWindow();
        }

        private static async Task KickAsync(Player actor, int targetId)
        {
            await using var db = new DataBase();
            var clan = await db.clans.WithGraph()
                .FirstOrDefaultAsync(c => c.id == actor.clan!.id);
            if (clan is null)
            {
                ShowInfo(actor, "Клан не найден.");
                return;
            }

            var target = clan.members.FirstOrDefault(m => m.id == targetId);
            if (target is null)
            {
                ShowInfo(actor, "Игрок не найден в клане.");
                return;
            }

            var perm = clan.CheckActOn(actor, target);
            if (!perm.Allowed)
            {
                ShowInfo(actor, perm.Error!);
                return;
            }

            await DataBase.SaveKickAsync(clan.id, targetId);

            await OpenWinAsync(actor);
        }

        private static async Task ChangeRankAsync(Player actor, int targetId, int rankId)
        {
            await using var db = new DataBase();
            var clan = await db.clans.WithGraph().FirstOrDefaultAsync(c => c.id == actor.clan!.id);
            if (clan is null)
            {
                ShowInfo(actor, "Клан не найден.");
                return;
            }

            var target = clan.members.FirstOrDefault(m => m.id == targetId);
            var newRank = clan.ranks.FirstOrDefault(r => r.id == rankId);

            if (target is null || newRank is null)
            {
                ShowInfo(actor, "Игрок или звание не найдены.");
                return;
            }

            var perm = clan.CheckAssignRank(actor, target, newRank);
            if (!perm.Allowed)
            {
                ShowInfo(actor, perm.Error!);
                return;
            }

            await DataBase.SaveChangeRankAsync(clan.id, targetId, rankId);

            await OpenPlayerPreviewAsync(actor, targetId);
        }

        private static async Task ChooseIconStepAsync(Player p)
        {
            await using var db = new DataBase();
            var taken = (await db.clans.AsNoTracking().Select(c => c.id).ToListAsync()).ToHashSet();
            if (taken.Count >= 218)
            {
                ShowInfo(p, "Свободных значков клана не осталось.");
                return;
            }

            var icons = new List<byte>(8);
            var rng = Random.Shared;
            while (icons.Count < 8)
            {
                var id = (byte)rng.Next(1, 219);
                if (!taken.Contains(id) && !icons.Contains(id))
                    icons.Add(id);
            }

            p.win?.CurrentTab.Open(new Page
            {
                Title = "Выбор значка клана",
                Text = "@@\nВыберите значок клана...\n",
                Buttons = [new MButton("Другие", "nexticons",
                async _ => await ChooseIconStepAsync(p))],
                Inventory = icons.Select(i => InventoryItem.Clan(i, 0)).ToArray(),
                OnInventory = async i =>
                {
                    var iconId = i - 200;
                    if (iconId is < 1 or > 218)
                    {
                        ShowInfo(p, "Некорректный значок клана.");
                        return;
                    }
                    await NameStepAsync(p, iconId);
                }
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
                ShowInfo(p, "Название клана не может быть пустым.");
                return Task.CompletedTask;
            }

            p.win?.CurrentTab.Open(new Page
            {
                Title = "Создание аббревиатуры",
                Text = "@@\nВыберите краткое имя клана...\n",
                Card = new Card(CardImageType.Clan, iconId.ToString(), $"<color=white>{name}</color>\n"),
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
                ShowInfo(p, "Аббревиатура не может быть пустой.");
                return Task.CompletedTask;
            }

            p.win?.CurrentTab.Open(new Page
            {
                Title = "Завершение создания клана",
                Text = "@@\nВсе готово для создания клана. После его создания, изменить название клана будет невозможно." +
                "\nПосле выхода вам вернется 90% его стоимости, а клан расформирован.\n",
                Card = new Card(CardImageType.Clan, iconId.ToString(),
                    $"<color=white>{name}[{abbr}]</color>\n"),
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
                ShowInfo(p, $"Название клана должно быть от {MinNameLength} до {MaxNameLength} символов.");
                return;
            }
            if (abbr.Length is < 1 or > MaxAbbreviationLength)
            {
                ShowInfo(p, $"Аббревиатура должна быть от 1 до {MaxAbbreviationLength} символов.");
                return;
            }
            if (!abbr.All(char.IsAsciiLetterUpper))
            {
                ShowInfo(p, "Аббревиатура должна состоять только из заглавных латинских букв.");
                return;
            }

            await using var db = new DataBase();

            if (await db.clans.AnyAsync(c => c.id == iconId))
            {
                ShowInfo(p, "Этот значок уже занят другим кланом.");
                return;
            }
            if (await db.clans.AnyAsync(c => c.name.ToLower() == name.ToLower()))
            {
                ShowInfo(p, "Клан с таким названием уже существует.");
                return;
            }

            var clan = new Clan
            {
                id = iconId,
                ownerid = p.id,
                name = name,
                abbreviation = abbr,
                lvl = 0,
                ranks =
                [
                    new Rank { name = "Новобранец", priority = 0,             colorhex = "#00FF00" },
                    new Rank { name = "Ветеран",   priority = OfficerPriority, colorhex = "#ff0000" },
                    new Rank { name = "Создатель", priority = OwnerPriority,   colorhex = "#006400" }
                ]
            };

            var created = await DataBase.SaveCreateClanAsync(clan);
            if (!created)
            {
                ShowInfo(p, "Не удалось создать клан (значок или имя заняты).");
                return;
            }

            var ownerRank = clan.ranks.First(r => r.priority == OwnerPriority);
            p.clan = clan;
            p.clanrank = ownerRank;
            p.SendClan();

            p.win = null;
            p.SendWindow();

            ShowInfo(p, "Удачи в клановой игре. Помни: клан — это не один игрок, а семья. " +
                "Держитесь вместе, выручайте друг друга и получайте от игры удовольствие.", 
                "Клан успешно создан!");
        }
    }
}
