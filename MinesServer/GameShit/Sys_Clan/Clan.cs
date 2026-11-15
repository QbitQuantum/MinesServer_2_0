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
        /// <summary>
        /// Загружает клан со всеми связанными данными: участниками (и их званиями),
        /// званиями клана и заявками (и их игроками).
        /// </summary>
        public static IQueryable<Clan> WithGraph(this IQueryable<Clan> q) => q
            .Include(c => c.members).ThenInclude(m => m.clanrank)
            .Include(c => c.ranks)
            .Include(c => c.reqs).ThenInclude(r => r.player);

        /// <summary>
        /// Загружает клан только с участниками и заявками (без званий и игроков заявок).
        /// Используется для облегчённого предпросмотра.
        /// </summary>
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

        /// <summary>
        /// Проверяет права актора в клане. При target == null — только членство
        /// и минимальный приоритет; при target != null — дополнительно запрет
        /// воздействия на владельца/себя и требование строго старшего звания.
        /// </summary>
        private Permission CheckAction(Player actor, Player? target = null, int requiredPriority = 0)
        {
            if (actor.clanrank is not { } rank || rank.owner.id != id)
                return Permission.Deny("Вы не состоите в этом клане.");

            if (target is null)
            {
                return rank.priority >= requiredPriority
                    ? Permission.Ok
                    : Permission.Deny("Недостаточно прав.");
            }

            if (target.id == ownerid)
                return Permission.Deny("Нельзя воздействовать на владельца клана.");
            
            if (target.id == actor.id)
                return Permission.Deny("Нельзя воздействовать на себя.");
            
            if (!rank.Outranks(target.clanrank))
                return Permission.Deny("Нельзя воздействовать на игрока с равным или более высоким званием.");

            return Permission.Ok;
        }

        /// <summary>
        /// Проверяет право на управление заявками (приоритет не ниже офицерского).
        /// </summary>
        private Permission CheckManageRequests(Player actor) =>
            CheckAction(actor, requiredPriority: OfficerPriority);

        /// <summary>
        /// Проверяет право выдать участнику указанное звание: сначала общая проверка
        /// воздействия, затем — что звание ниже собственного.
        /// </summary>
        private Permission CheckAssignRank(Player actor, Player target, Rank rank)
        {
            var act = CheckAction(actor, target);
            if (!act.Allowed) return act;

            return rank.priority < actor.clanrank!.priority
                ? Permission.Ok
                : Permission.Deny("Нельзя выдать звание выше или равное своему.");
        }

        /// <summary>
        /// Проверяет, может ли игрок управлять заявками (приоритет не ниже офицерского).
        /// </summary>
        private bool CanManageRequests(Player actor) => CheckManageRequests(actor).Allowed;

        /// <summary>
        /// Проверяет, есть ли у игрока непросроченная заявка в этот клан.
        /// </summary>
        private bool HasPendingRequest(int playerId) =>
            reqs.Any(r => r.PlayerId == playerId && !r.IsExpired);

        /// <summary>
        /// Проверяет длину названия клана.
        /// </summary>
        private static Permission ValidateName(string name)
        {
            name = name.Trim();
            return name.Length >= MinNameLength && name.Length <= MaxNameLength
                ? Permission.Ok
                : Permission.Deny($"Название клана должно быть от {MinNameLength} до {MaxNameLength} символов.");
        }

        /// <summary>
        /// Проверяет аббревиатуру: длину и допустимые символы (заглавные латинские буквы).
        /// </summary>
        private static Permission ValidateAbbreviation(string abbr)
        {
            abbr = abbr.Trim().ToUpperInvariant();
            if (abbr.Length is < 1 or > MaxAbbreviationLength)
                return Permission.Deny($"Аббревиатура должна быть от 1 до {MaxAbbreviationLength} символов.");
            if (!abbr.All(char.IsAsciiLetterUpper))
                return Permission.Deny("Аббревиатура должна состоять только из заглавных латинских букв.");
            return Permission.Ok;
        }

        /// <summary>
        /// Открывает окно с ознакомительным текстом и кнопкой перехода к выбору значка клана.
        /// </summary>
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

        /// <summary>
        /// Открывает общее окно со списком кланов, вкладкой достижений и вкладкой дипломатии.
        /// </summary>
        public static async Task OpenListAsync(Player p)
        {
            static ClanListEntry C(int id, string name, string rightText) =>
                new ClanListEntry(
                    new MButton(name, $"clan{id}"),
                    (byte)id,
                    rightText);

            await using var db = new DataBase();

            var clans = await db.clans
                .AsNoTracking()
                .OrderByDescending(c => c.lvl)
                .Select(c => new
                {
                    c.id,
                    c.name,
                    c.abbreviation,
                    c.lvl,
                    Members = c.members.Count,
                    Requests = c.reqs.Count
                })
                .ToListAsync();

            var entries = clans
            .Select(c => new ClanListEntry(
                new MButton(
                    $"<color=white>{c.name}</color> [{c.abbreviation}]",
                    $"clan{c.id}",
                    async _ => await OpenPreviewAsync(p, c.id)),
                (byte)c.id,
                $"<color=#00FF00>Приём открыт</color> ({c.Requests}|{c.Members}) ур.{c.lvl}"))
            .ToArray();

            static RichListEntry B(string rightText, string buttonName, string action) =>
                RichListEntry.Button(rightText, new MButton(buttonName, action));

            var typeUpgrade = new[]
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
                            RichList = new(typeUpgrade, false),
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

        /// <summary>
        /// Открывает главное окно клана игрока.
        /// </summary>
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

        /// <summary>
        /// Открывает страницу предпросмотра клана для постороннего игрока:
        /// показывает карточку клана и (если возможно) кнопку подачи заявки.
        /// </summary>
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
                text = "\nЗаявка подана.";
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

        /// <summary>
        /// Строит вкладку "Обзор": карточка клана (название, уровень, число участников)
        /// и текст с текущей клановой миссией и сообщением для сокланов.
        /// </summary>
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

        /// <summary>
        /// Строит вкладку "Список": перечень участников клана с их званиями
        /// и статусом онлайн, плюс кнопку "Покинуть клан".
        /// </summary>
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

        /// <summary>
        /// Строит вкладку "Заявки": нумерованный список игроков, подавших заявку,
        /// с кнопкой открытия деталей каждой заявки.
        /// </summary>
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

        /// <summary>
        /// Открывает профиль соклана. В обычном режиме — кнопки "Прокачка" и (при наличии прав)
        /// "Изменить звание". В режиме changeRank — выпадающий список званий и кнопки
        /// "Применить" и "Исключить".
        /// </summary>
        private static async Task OpenPlayerPreviewAsync(Player p, int targetId, bool OpenChangeRankPage = false)
        {
            if (p.clan is null) return;

            await using var db = new DataBase();
            var player = await db.players.Include(x => x.clanrank).AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == targetId);

            if (player is null) return;

            var clan = await db.clans.Include(c => c.ranks).AsNoTracking()
                .FirstOrDefaultAsync(c => c.id == p.clan.id);

            if (clan is null) return;

            var canAct = clan.CheckAction(p, player).Allowed;

            var buttons = !OpenChangeRankPage ? new List<MButton>
            {
                new("Прокачка", "skills", async _ => await OpenSkillsAsync(p, targetId)),
                new("Исключить", "kick", async _ => await KickAsync(p, player))
            } : [];

            RichListEntry[] rich = [];

            if (canAct)
            {
                if (OpenChangeRankPage)
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
                }
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

        /// <summary>
        /// Открывает страницу просмотра скиллов указанного игрока (только чтение)
        /// с общим уровнем и списком слотов.
        /// </summary>
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

        /// <summary>
        /// Открывает детали конкретной заявки: имя и ID игрока, оставшееся время,
        /// а также кнопки "Принять", "Отклонить" и "Прокачка".
        /// </summary>
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

        /// <summary>
        /// Подаёт заявку на вступление в клан от имени игрока: проверяет, что игрок
        /// без клана и заявка ещё не подана, сохраняет её и обновляет страницу предпросмотра.
        /// </summary>
        private static async Task SubmitRequestAsync(Player p, Clan clan)
        {
            if (p.clan is not null)
            {
                p.ShowInfo("Вы уже состоите в клане.");
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

        /// <summary>
        /// Принимает заявку: проверяет права актора, применяет изменение в БД
        /// и перерисовывает окно клана.
        /// </summary>
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

        /// <summary>
        /// Отклоняет заявку: проверяет права актора, удаляет заявку из БД
        /// и перерисовывает окно клана.
        /// </summary>
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

        /// <summary>
        /// Выход игрока из клана: сохраняет изменение в БД, сбрасывает клан/звание
        /// у игрока, перерисовывает ботов и закрывает окно клана.
        /// </summary>
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

        /// <summary>
        /// Исключает участника из клана: проверяет права актора (не владелец, не себя,
        /// выше по званию), сохраняет изменение и перерисовывает окно клана.
        /// </summary>
        private static async Task KickAsync(Player p, Player target)
        {
            var perm = p.clan!.CheckAction(p, target);
            if (!perm.Allowed)
            {
                p.ShowInfo(perm.Error!);
                return;
            }

            await DataBase.SaveKickAsync(target.id);

            await OpenWinAsync(p);
        }

        /// <summary>
        /// Меняет звание участника: проверяет права на выдачу указанного звания,
        /// сохраняет изменение и переоткрывает профиль участника.
        /// </summary>
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

        /// <summary>
        /// Шаг выбора значка клана: вычисляет свободные значки (1..MaxClanIcons, ещё не занятые),
        /// показывает их списком; при отсутствии свободных — сообщает об этом.
        /// </summary>
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

        /// <summary>
        /// Шаг ввода названия клана: показывает поле ввода с ограничением длины
        /// и переходит к шагу аббревиатуры после подтверждения.
        /// </summary>
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

        /// <summary>
        /// Шаг ввода аббревиатуры: проверяет непустое название, показывает карточку
        /// клана с введённым именем и поле ввода аббревиатуры; затем переход к подтверждению.
        /// </summary>
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

        /// <summary>
        /// Финальный шаг подтверждения: проверяет непустую аббревиатуру, показывает
        /// итоговую карточку (имя + аббревиатура) и текст с условиями, а также кнопку
        /// "Принимаю условия", ведущую к созданию.
        /// </summary>
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

        /// <summary>
        /// Создаёт клан: валидирует имя и аббревиатуру (длина, заглавные латинские буквы),
        /// сохраняет клан в БД, назначает игрока владельцем, обновляет состояние
        /// и показывает поздравительное сообщение.
        /// </summary>
        private static async Task CreateAsync(Player p, int iconId, string name, string abbr)
        {
            name = name.Trim();
            abbr = abbr.Trim().ToUpperInvariant();

            var nameCheck = ValidateName(name);
            if (!nameCheck.Allowed)
            {
                p.ShowInfo(nameCheck.Error!);
                return;
            }

            var abbrCheck = ValidateAbbreviation(abbr);
            if (!abbrCheck.Allowed)
            {
                p.ShowInfo(abbrCheck.Error!);
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
                .AppendLine("Удачи в клановой игре. Помни: клан — это не один игрок, а семья.")
                .Append("Держимся вместе, выручаем друг друга и получаем от игры удовольствие.")
                .ToString();

            p.ShowInfo(message, "Клан успешно создан!");
        }
    }
}