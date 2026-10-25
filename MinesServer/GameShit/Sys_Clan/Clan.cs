using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.GUI.UP;
using MinesServer.Server;

namespace MinesServer.GameShit.ClanSystem
{
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
        public Rank DefaultRank => ranks.OrderBy(r => r.priority).First();
        public Rank? OwnerRank => ranks.FirstOrDefault(r => r.priority == OwnerPriority);

        #endregion

        public bool IsOwner(int playerId) => ownerid == playerId;

        public bool HasPendingRequest(int playerId) =>
            reqs.Any(r => r.PlayerId == playerId && !r.IsExpired);

        public bool CanManage(Player actor) =>
            actor.clanrank is not null && actor.clanrank.ClanId == id;

        private static void Sync(Player target, Clan? clan, Rank? rank)
        {
            target.clan = clan;
            target.clanrank = rank;
            target.SendClan();
        }

        private static Rank EnsureOwnerRank(Clan clan) =>
            clan.OwnerRank ?? throw new InvalidOperationException(
                $"У клана {clan.id} отсутствует ранг с priority={OwnerPriority}");

        private static Task<Clan?> LoadActorClanAsync(DataBase db, Player actor) =>
            actor.clan is null
                ? Task.FromResult<Clan?>(null)
                : db.clans.WithGraph().FirstOrDefaultAsync(c => c.id == actor.clan.id);

        public static Task OpenCreateWindowAsync(Player p)
        {
            p.win = new Window
            {
                Tabs =
                [
                    new Tab
                {
                    Label = "",
                    Action = "clancreate:1",
                    InitialPage = new Page
                    {
                        Text = "@@\nУра! Вы собираетесь создать новый клан. После создания клана вы сможете\n" +
                               "выполнять клановые квесты, создавать свои фермы, вести войны с другими\n" +
                               "кланами, защищать и отбивать территории, и многое другое.\n\n" +
                               "Создание клана - ответственное действие, значок и название клана нельзя\n" +
                               "будет изменить позже. Поэтому внимательно подумайте над тем, как будет\n" +
                               "звучать и выглядеть ваш клан в игре.\n\n" +
                               "Создание клана требует залога в 1000 кредитов.\n",
                        Buttons =
                        [
                            new MButton("ВЫБРАТЬ ЗНАЧОК КЛАНА", "chooseicon",
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
            await using var db = new DataBase();
            var clans = await db.clans.WithGraphLight().ToListAsync();

            var entries = clans.Select(c => new ClanListEntry(
                new MButton($"<color=white>{c.name}</color> [{c.abbreviation}]",
                    $"clan{c.id}",
                    async _ => await OpenPreviewAsync(p, c.id)),
                (byte)c.id,
                "Приём открыт")).ToArray();

            p.win = new Window
            {
                Tabs =
                [
                    new Tab
                {
                    Action = "clanlist",
                    Label = "СПИСОК КЛАНОВ",
                    InitialPage = new Page
                    {
                        Title = "КЛАНЫ",
                        ClanList = entries,
                        Buttons = []
                    }
                }
                ]
            };
            p.SendWindow();
        }

        public static async Task OpenWinAsync(Player p)
        {
            if (p.clan is null) return;

            await using var db = new DataBase();
            var clan = await LoadActorClanAsync(db, p);
            if (clan is null) return;

            var tabs = new List<Tab> { BuildOverviewTab(clan), BuildListTab(p, clan) };

            if (clan.reqs.Count > 0 && (p.clanrank?.priority ?? 0) >= OfficerPriority)
                tabs.Add(BuildRequestsTab(p, clan));

            p.win = new Window { Title = clan.name, ShowTabs = true, Tabs = tabs.ToArray() };
            p.SendWindow();
        }


        public static async Task OpenPreviewAsync(Player p, int clanId)
        {
            await using var db = new DataBase();
            var clan = await db.clans.WithGraphLight().FirstOrDefaultAsync(c => c.id == clanId);
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
                Title = "КЛАНЫ",
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
                    $"<color=white>{clan.name}[{clan.abbreviation}]</color>\n" +
                    $"Уровень клана: <color=white>{clan.lvl}</color>\n" +
                    $"Участники: <color=white>{clan.members.Count}</color>\n"),
                Text = "@@\nПРОКАЧКА КЛАНА: НЕ ВЫБРАНО\n\n" +
                       "СООБЩЕНИЕ ДЛЯ СОКЛАНОВ:\n" +
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

        public static async Task OpenPlayerPreviewAsync(Player p, int targetId, bool changeRank = false)
        {
            if (p.clan is null) return;

            await using var db = new DataBase();
            var target = await db.players.Include(x => x.clanrank)
                .FirstOrDefaultAsync(x => x.id == targetId);
            if (target is null) return;

            var clan = await db.clans.Include(c => c.ranks)
                .FirstOrDefaultAsync(c => c.id == p.clan.id);
            if (clan is null) return;

            var canManage = clan.CanManage(p) && p.clanrank!.Outranks(target.clanrank);

            var buttons = new List<MButton>
        {
            new("Прокачка", "skills", async _ => await OpenSkillsAsync(p, targetId))
        };

            RichListEntry[] rich = [];
            if (changeRank && canManage)
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
                            || idx < 0 || idx >= available.Length) return;
                        await ChangeRankAsync(p, targetId, available[idx].id);
                    }));
                buttons.Add(new MButton("Исключить", "kick",
                    async _ => await KickAsync(p, targetId)));
            }
            else if (canManage)
            {
                buttons.Add(new MButton("Изменить звание", "changerank",
                    async _ => await OpenPlayerPreviewAsync(p, targetId, true)));
            }

            p.win?.CurrentTab.Open(new Page
            {
                RichList = new RichListConfig(rich, true),
                Text = $"@@ПРОФИЛЬ СОКЛАНА\n\n" +
                       $"Имя: <color={target.clanrank?.colorhex}>{target.name}</color>\n" +
                       $"Звание: {target.clanrank?.name}\n" +
                       $"ID: <color=white>{target.id}</color>",
                Buttons = buttons.ToArray()
            });
            p.SendWindow();
        }

        public static async Task OpenSkillsAsync(Player p, int targetId)
        {
            await using var db = new DataBase();
            var target = await db.players.Include(x => x.skillslist)
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

        public static async Task OpenRequestAsync(Player actor, int requestId)
        {
            if (actor.clan is null) return;

            await using var db = new DataBase();
            var request = await db.reqs.Include(r => r.player)
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

        public static async Task SubmitRequestAsync(Player p, int clanId)
        {
            if (p.clan is not null) return;

            await using var db = new DataBase();
            var clan = await db.clans.Include(c => c.reqs)
                .FirstOrDefaultAsync(c => c.id == clanId);
            if (clan is null || clan.HasPendingRequest(p.id)) return;

            clan.reqs.Add(new Request
            {
                PlayerId = p.id,
                player = p,
                ClanId = clan.id,
                reqtime = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            await OpenPreviewAsync(p, clanId);
        }

        public static async Task AcceptRequestAsync(Player actor, int requestId)
        {
            await using var db = new DataBase();
            var clan = await LoadActorClanAsync(db, actor);
            if (clan is null || !clan.CanManage(actor)) return;

            var request = clan.reqs.FirstOrDefault(r => r.id == requestId);
            if (request is null || request.IsExpired || request.player.clan is not null) return;

            request.player.clan = clan;
            request.player.clanrank = clan.DefaultRank;
            clan.reqs.Remove(request);

            await db.SaveChangesAsync();
            request.player.SendClan();

            await OpenWinAsync(actor);
        }

        public static async Task DeclineRequestAsync(Player actor, int requestId)
        {
            if (actor.clan is null) return;

            await using var db = new DataBase();
            var req = await db.reqs
                .FirstOrDefaultAsync(r => r.id == requestId && r.ClanId == actor.clan.id);
            if (req is null) return;

            db.reqs.Remove(req);
            await db.SaveChangesAsync();

            await OpenWinAsync(actor);
        }

        public static async Task LeaveAsync(Player p)
        {
            if (p.clan is null) return;

            await using var db = new DataBase();
            var clan = await LoadActorClanAsync(db, p);

            if (clan is null)
            {
                Sync(p, null, null);
                return;
            }

            var isOwner = clan.IsOwner(p.id);
            var player = clan.members.FirstOrDefault(m => m.id == p.id) ?? p;
            player.clan = null;
            player.clanrank = null;

            if (isOwner)
            {
                var heir = clan.members
                    .Where(m => m.id != p.id)
                    .OrderByDescending(m => m.clanrank?.priority ?? 0)
                    .FirstOrDefault();

                if (heir is null)
                {
                    db.clans.Remove(clan);
                }
                else
                {
                    clan.ownerid = heir.id;
                    heir.clanrank = EnsureOwnerRank(clan);
                }
            }

            await db.SaveChangesAsync();

            Sync(p, null, null);
            p.BotsRender();
            p.win = null;
            p.SendWindow();
        }

        public static async Task KickAsync(Player actor, int targetId)
        {
            await using var db = new DataBase();
            var clan = await LoadActorClanAsync(db, actor);
            if (clan is null || !clan.CanManage(actor)) return;
            if (targetId == clan.ownerid || targetId == actor.id) return;

            var target = clan.members.FirstOrDefault(m => m.id == targetId);
            if (target is null || !actor.clanrank!.Outranks(target.clanrank)) return;

            target.clan = null;
            target.clanrank = null;
            await db.SaveChangesAsync();
            target.SendClan();

            await OpenWinAsync(actor);
        }

        public static async Task ChangeRankAsync(Player actor, int targetId, int rankId)
        {
            await using var db = new DataBase();
            var clan = await LoadActorClanAsync(db, actor);
            if (clan is null || !clan.CanManage(actor)) return;

            var target = clan.members.FirstOrDefault(m => m.id == targetId);
            var newRank = clan.ranks.FirstOrDefault(r => r.id == rankId);
            if (target is null || newRank is null) return;

            if (newRank.priority >= actor.clanrank!.priority) return;
            if (!actor.clanrank.Outranks(target.clanrank)) return;

            target.clanrank = newRank;
            await db.SaveChangesAsync();
            target.SendClan();

            await OpenPlayerPreviewAsync(actor, targetId);
        }

        private static async Task ChooseIconStepAsync(Player p)
        {
            await using var db = new DataBase();
            var taken = (await db.clans.Select(c => c.id).ToListAsync()).ToHashSet();
            if (taken.Count >= 218) return;

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
                Title = "ВЫБОР ЗНАЧКА КЛАНА",
                Text = "@@Выберите значок клана...\n",
                Buttons = [new MButton("Другие", "nexticons",
                async _ => await ChooseIconStepAsync(p))],
                Inventory = icons.Select(i => InventoryItem.Clan(i, 0)).ToArray(),
                OnInventory = async i =>
                {
                    var iconId = i - 200;
                    if (iconId is < 1 or > 218) return;
                    await NameStepAsync(p, iconId);
                }
            });
            p.SendWindow();
        }

        private static Task NameStepAsync(Player p, int iconId)
        {
            p.win?.CurrentTab.Open(new Page
            {
                Title = "ВЫБОР НАЗВАНИЯ КЛАНА",
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
            p.win?.CurrentTab.Open(new Page
            {
                Title = "СОЗДАНИЕ АББРЕВИАТУРЫ",
                Text = "@@\nВыберите краткое имя клана...\n",
                Card = new Card(CardImageType.Clan, iconId.ToString(), $"<color=white>{name}</color>\n"),
                Input = new InputConfig
                {
                    IsConsole = false,
                    Placeholder = "Аббревиатура",
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
            p.win?.CurrentTab.Open(new Page
            {
                Title = "ЗАВЕРШЕНИЕ СОЗДАНИЯ КЛАНА",
                Text = "@@\nВсе готово для создания клана...\n",
                Card = new Card(CardImageType.Clan, iconId.ToString(),
                    $"<color=white>{name}[{abbr}]</color>\n"),
                Buttons =
                [
                    new MButton("<color=#ff8888ff>ПРИНИМАЮ УСЛОВИЯ</color>", "complete",
                    async _ => await CreateAsync(p, iconId, name, abbr))
                ]
            });
            p.SendWindow();
            return Task.CompletedTask;
        }

        public static async Task CreateAsync(Player p, int iconId, string name, string abbr)
        {
            name = name.Trim();
            abbr = abbr.Trim().ToUpperInvariant();

            if (name.Length < MinNameLength || name.Length > MaxNameLength) return;
            if (abbr.Length is < 1 or > MaxAbbreviationLength) return;
            if (!abbr.All(char.IsAsciiLetterUpper)) return;

            await using var db = new DataBase();

            if (await db.clans.AnyAsync(c => c.id == iconId)) return;
            if (await db.clans.AnyAsync(c => c.name.ToLower() == name.ToLower())) return;

            var player = await db.players.FirstOrDefaultAsync(x => x.id == p.id);
            if (player is null) return;

            var clan = new Clan
            {
                id = iconId,
                ownerid = player.id,
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
            foreach (var r in clan.ranks) r.owner = clan;

            var ownerRank = clan.ranks.First(r => r.priority == OwnerPriority);
            player.clan = clan;
            player.clanrank = ownerRank;

            db.clans.Add(clan);
            await db.SaveChangesAsync();

            p.clan = clan;
            p.clanrank = ownerRank;
            p.SendClan();

            p.win?.CurrentTab.Open(new Page { Title = "КЛАН СОЗДАН", Buttons = [] });
            p.SendWindow();
        }
    }
}
