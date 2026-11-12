using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GChat;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Sys_Clan;
using MinesServer.GameShit.Sys_Craft;
using MinesServer.GameShit.SysMarket;
using MinesServer.GameShit.VulkSystem;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.Server
{
    public static class DataBaseExt
    {
        public static void RemoveAll<T>(this DbSet<T> s) where T : class => s.RemoveRange(s);
    }

    public sealed class DataBase : DbContext
    {
        #region player
        public DbSet<Program> progs => Set<Program>();
        public DbSet<Player> players => Set<Player>();
        public DbSet<BotSpot> botspots => Set<BotSpot>();
        public DbSet<Inventory> inventories => Set<Inventory>();
        public DbSet<Basket> baskets => Set<Basket>();
        public DbSet<PlayerSkills> skills => Set<PlayerSkills>();
        public DbSet<Settings> settings => Set<Settings>();
        #endregion

        #region Utils
        public DbSet<LineChat> lines => Set<LineChat>();
        public DbSet<Chat> chats => Set<Chat>();
        public DbSet<Box> boxes => Set<Box>();
        public DbSet<Order> orders => Set<Order>();
        public DbSet<Clan> clans => Set<Clan>();
        public DbSet<Request> reqs => Set<Request>();
        public DbSet<Rank> ranks => Set<Rank>();
        public DbSet<CraftEntry> craftentries => Set<CraftEntry>();
        #endregion

        #region packs
        public DbSet<Spot> spots => Set<Spot>();
        public DbSet<Vulkan> vulkans => Set<Vulkan>();
        public DbSet<Resp> resps => Set<Resp>();
        public DbSet<Market> markets => Set<Market>();
        public DbSet<Up> ups => Set<Up>();
        public DbSet<Gun> guns => Set<Gun>();
        public DbSet<Storage> storages => Set<Storage>();
        public DbSet<Crafter> crafts => Set<Crafter>();
        public DbSet<Teleport> teleports => Set<Teleport>();
        public DbSet<Gate> gates => Set<Gate>();
        public DbSet<NC> ncs => Set<NC>();
        public DbSet<Observatory> observatory => Set<Observatory>();
        public DbSet<Mayak> mayak => Set<Mayak>();
        public DbSet<Jobs> jobs => Set<Jobs>();
        #endregion

        #region fields

        // TODO: Сделать получение через World, чтобы ServerTime не имел доступ к DataBase
        // Либо инкапсулировать полностью
        public static List<Player> activeplayers = [];
        public static List<BotSpot> botspotplayer = [];
        private static readonly Dictionary<int, string> _nicklist = [];

        #endregion


        public DataBase() : base() => Database.EnsureCreated();
        public void Delete() => Database.EnsureDeleted();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=M.db;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>()
                .Property(p => p.id)
                .ValueGeneratedNever();

            modelBuilder.Entity<BotSpot>()
                .Property(b => b.id)
                .ValueGeneratedNever();
        }
        public static void Save()
        {
            using var db = new DataBase();
            db.SaveChanges();
        }

        public static void ClearAll()
        {
            using var db = new DataBase();
            activeplayers = new List<Player>();
            foreach (var i in db.players) i.resp = null;
            db.boxes.RemoveAll();
            db.teleports.RemoveAll();
            db.resps.RemoveAll();
            db.ups.RemoveAll();
            db.storages.RemoveAll();
            db.vulkans.RemoveAll();
            db.markets.RemoveAll();
            db.guns.RemoveAll();
            db.gates.RemoveAll();
            db.SaveChanges();
        }

        public static int GetNextId()
        {
            using var db = new DataBase();
            return GetNextUniqueId(db);
        }

        private static int GetNextUniqueId(DataBase db)
        {
            var maxPlayerId = db.players.Max(p => (int?)p.id) ?? 0;
            var maxBotId = db.botspots.Max(b => (int?)b.id) ?? 0;
            return Math.Max(maxPlayerId, maxBotId) + 1;
        }

        public static Player? GetPlayer(int id)
        {
            var player = activeplayers.FirstOrDefault(p => p.id == id);
            if (player != null)
            {
                return player;
            }
            using var db = new DataBase();
            return db.players
                .Where(i => i.id == id)
                .Include(p => p.clanrank)
                .Include(p => p.clan)
                .Include(p => p.inventory)
                .Include(p => p.crys)
                .Include(p => p.skillslist)
                .Include(p => p.settings)
                .Include(p => p.resp)
                .FirstOrDefault();
        }

        public static BotSpot? GetBotSpot(int botSpotId)
        {
            var botSpot = botspotplayer.FirstOrDefault(p => p.id == botSpotId);
            if (botSpot != null)
                return botSpot;
            using var db = new DataBase();
            return db.botspots
                .FirstOrDefault(i => i.id == botSpotId);
        }

        public static Chat? GetChat(string tag)
        {
            using var db = new DataBase();
            return db.chats
                .FirstOrDefault(i => i.tag == tag);
        }

        public static Chat? GetChatWithMessages(string tag, int limit = 30)
        {
            using var db = new DataBase();
            return db.chats.Include(c => c.messages
                    .OrderByDescending(m => m.id)
                    .Take(limit))
                .FirstOrDefault(i => i.tag == tag);
        }

        public static LineChat? GetLineChat(int id)
        {
            using var db = new DataBase();
            return db.lines
                .FirstOrDefault(i => i.id == id);
        }

        public static string NickName(int id)
        {
            if (_nicklist.TryGetValue(id, out string? value))
                return value;

            var player = GetPlayer(id);
            if (player != null)
            {
                _nicklist[id] = player.name;
                return _nicklist[id];
            }

            var botspot = GetBotSpot(id);
            if (botspot != null)
            {
                _nicklist[id] = botspot.name;
                return _nicklist[id];
            }
            // Console.WriteLine("Aномалия игроков. Проверить");
            return "Aномальный ID игрока. Проверить";
        }

        public static Player? GetPlayer(string name)
        {
            var player = activeplayers.FirstOrDefault(p => p.name == name);
            if (player != null)
            {
                return player;
            }
            using var db = new DataBase();
            return db.players
            .Where(i => i.name == name)
            .Include(p => p.clanrank)
            .Include(p => p.clan)
            .Include(p => p.inventory)
            .Include(p => p.crys)
            .Include(p => p.skillslist)
            .Include(p => p.settings)
            .Include(p => p.resp)
            .FirstOrDefault();
        }

        public static bool PlayerExists(string name)
        {
            // Проверка в активных игроках
            if (activeplayers.Any(p => p.name == name))
                return true;

            // Проверка в базе данных
            using var db = new DataBase();
            return db.players.Any(i => i.name == name);
        }

        public static Box? GetBox(int x, int y)
        {
            using var db = new DataBase();
            return db.boxes
                .AsNoTracking()
                .FirstOrDefault(t => t.x == x && t.y == y);
        }

        public static List<Resp?> GetResp(int ownerid)
        {
            using var db = new DataBase();
            return db.resps.Where(r => r.ownerid == ownerid).ToList();
        }

        public static void DeleteProg(int id)
        {
            using var db = new DataBase();
            db.progs.Remove(db.progs.FirstOrDefault(i => i.id == id)!);
            db.SaveChanges();
        }

        public static Program? SaveAndGetProg((int id, string source) data)
        {
            using var db = new DataBase();
            Program? programm = db.progs.FirstOrDefault(i => i.id == data.id);
            if (programm != default)
            {
                programm.data = data.source;
                db.SaveChanges();
            }
            return programm;
        }

        public IEnumerable<IEnumerable<Pack>> GetAllPackCollections()
        {
            yield return spots.Cast<Pack>();
            yield return vulkans.Cast<Pack>();
            yield return resps.Cast<Pack>();
            yield return markets.Cast<Pack>();
            yield return ups.Cast<Pack>();
            yield return guns.Cast<Pack>();
            yield return storages.Cast<Pack>();
            yield return crafts.Cast<Pack>();
            yield return teleports.Cast<Pack>();
            yield return gates.Cast<Pack>();
            yield return ncs.Cast<Pack>();
            yield return observatory.Cast<Pack>();
            yield return mayak.Cast<Pack>();
            yield return jobs.Cast<Pack>();
            yield return boxes.Cast<Pack>();
        }

        private static void LoadPacksBuild(DataBase db)
        {
            foreach (var collection in db.GetAllPackCollections())
            {
                foreach (Pack item in collection)
                {
                    item.Build();
                }
            }
        }

        public static void Load()
        {
            using var db = new DataBase();
            try
            {
                LoadPacksBuild(db);
            }
            catch (Exception ex)
            {
                Console.WriteLine("WriteError caused error " + $"{ex}");
            }
        }

        /// <summary>
        /// Помечает только указанные поля как изменённые и сохраняет.
        /// </summary>
        public void Save<T>(
            T entity,
            params Expression<Func<T, object?>>[] fields)
            where T : class
        {
            var entry = Entry(entity);
            if (entry.State == EntityState.Detached)
                Attach(entity);

            foreach (var field in fields)
                entry.Property(field).IsModified = true;

            SaveChanges();
        }

        #region Clan saves

        /// <summary>
        /// Подача заявки в клан.
        /// </summary>
        public static async Task<bool> SaveClanRequestAsync(int clanId, int playerId)
        {
            await using var db = new DataBase();

            var clan = await db.clans.Include(c => c.reqs).FirstOrDefaultAsync(c => c.id == clanId);
            if (clan is null) return false;

            if (clan.reqs.Any(r => r.PlayerId == playerId && !r.IsExpired))
                return false;

            clan.reqs.Add(new Request
            {
                PlayerId = playerId,
                ClanId = clan.id,
                reqtime = DateTime.UtcNow
            });

            db.Save(clan, c => c.reqs);
            return true;
        }

        /// <summary>
        /// Принятие заявки: переносит игрока в клан с дефолтным рангом и удаляет заявку.
        /// </summary>
        public static async Task<bool> SaveAcceptRequestAsync(int requestId)
        {
            await using var db = new DataBase();

            var request = await db.reqs
                .Include(r => r.player)
                .FirstOrDefaultAsync(r => r.id == requestId);
            if (request is null || request.player is null) return false;

            var clan = await db.clans
                .Include(c => c.reqs)
                .Include(c => c.ranks)
                .Include(c => c.members)
                .FirstOrDefaultAsync(c => c.id == request.ClanId);
            if (clan is null) return false;

            var player = await db.players
                .Include(p => p.clan)
                .Include(p => p.clanrank)
                .FirstOrDefaultAsync(p => p.id == request.PlayerId);
            if (player is null) return false;

            if (player.clan is not null) return false;

            var rank = clan.ranks
                .OrderBy(r => r.priority)
                .FirstOrDefault();
            if (rank is null) return false;

            player.clan = clan;
            player.clanrank = rank;

            clan.reqs.Remove(request);

            db.Save(player, t => t.clan, t => t.clanrank);
            db.Save(clan, c => c.reqs);

            return true;
        }

        /// <summary>
        /// Отклонение заявки.
        /// </summary>
        public static async Task<bool> SaveDeclineRequestAsync(int requestId)
        {
            await using var db = new DataBase();

            var clan = await db.clans
                .Include(c => c.reqs)
                .FirstOrDefaultAsync(c => c.reqs.Any(r => r.id == requestId));
            if (clan is null) return false;

            var request = clan.reqs.FirstOrDefault(r => r.id == requestId);
            if (request is null) return false;

            clan.reqs.Remove(request);
            db.Save(clan, c => c.reqs);

            return true;
        }

        /// <summary>
        /// Выход игрока из клана, включая передачу владения или удаление клана.
        /// </summary>
        public static async Task<bool> SaveLeaveClanAsync(int clanId, int playerId)
        {
            await using var db = new DataBase();

            var clan = await db.clans
                .Include(c => c.members).ThenInclude(m => m.clanrank)
                .Include(c => c.ranks)
                .FirstOrDefaultAsync(c => c.id == clanId);
            if (clan is null) return false;

            var player = clan.members.FirstOrDefault(m => m.id == playerId);
            if (player is null) return false;

            player.clan = null;
            player.clanrank = null;

            if (clan.ownerid == playerId)
            {
                var heir = clan.members
                    .Where(m => m.id != playerId)
                    .OrderByDescending(m => m.clanrank?.priority ?? 0)
                    .FirstOrDefault();

                if (heir is null)
                {
                    db.clans.Remove(clan);
                    await db.SaveChangesAsync();
                    return true;
                }

                var rank = clan.ranks.FirstOrDefault(r => r.priority == Clan.OwnerPriority);
                if (rank is null) return false;

                clan.ownerid = heir.id;
                heir.clanrank = rank;

                db.Save(clan, c => c.ownerid);
                db.Save(heir, t => t.clanrank);
                db.Save(player, t => t.clan, t => t.clanrank);
            }
            else
            {
                db.Save(player, t => t.clan, t => t.clanrank);
            }

            return true;
        }

        /// <summary>
        /// Исключение игрока из клана.
        /// </summary>
        public static async Task<bool> SaveKickAsync(int playerId)
        {
            await using var db = new DataBase();

            var player = await db.players
                .Include(p => p.clan)
                .Include(p => p.clanrank)
                .FirstOrDefaultAsync(p => p.id == playerId);
            if (player is null) return false;

            player.clan = null;
            player.clanrank = null;

            db.Save(player, t => t.clan, t => t.clanrank);
            return true;
        }

        /// <summary>
        /// Смена звания игрока.
        /// </summary>
        public static async Task<bool> SaveChangeRankAsync(int playerId, int rankId)
        {
            await using var db = new DataBase();

            var player = await db.players
                .Include(p => p.clanrank)
                .FirstOrDefaultAsync(p => p.id == playerId);
            var rank = await db.ranks.FirstOrDefaultAsync(r => r.id == rankId);

            if (player is null || rank is null) return false;

            player.clanrank = rank;

            db.Save(player, t => t.clanrank);
            return true;
        }

        /// <summary>
        /// Создание клана. Клан собирается ВНУТРИ контекста, чтобы не тащить чужие экземпляры Player/Rank.
        /// </summary>
        public static async Task<Clan?> SaveCreateClanAsync(
            int iconId,
            string name,
            string abbreviation,
            int ownerId)
        {
            await using var db = new DataBase();

            if (await db.clans.AnyAsync(c => c.id == iconId)) return null;
            if (await db.clans.AnyAsync(c => EF.Functions.Collate(c.name, "NOCASE") == name)) return null;

            var player = await db.players
                .Include(p => p.clan)
                .FirstOrDefaultAsync(p => p.id == ownerId);
            if (player is null) return null;
            if (player.clan is not null) return null;

            var clan = new Clan
            {
                id = iconId,
                ownerid = ownerId,
                name = name,
                abbreviation = abbreviation,
                lvl = 0,
                ranks =
                [
                    new Rank { name = "Новобранец", priority = 0,                   colorhex = "#00FF00" },
                    new Rank { name = "Ветеран",    priority = Clan.OfficerPriority, colorhex = "#ff0000" },
                    new Rank { name = "Создатель",  priority = Clan.OwnerPriority,   colorhex = "#006400" }
                ]
            };

            var ownerRank = clan.ranks.First(r => r.priority == Clan.OwnerPriority);

            player.clan = clan;
            player.clanrank = ownerRank;
            clan.members.Add(player);

            db.clans.Add(clan);
            await db.SaveChangesAsync();

            return clan;
        }

        #endregion
    }
}