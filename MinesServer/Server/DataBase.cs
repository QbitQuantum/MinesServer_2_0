using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.ClanSystem;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GChat;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Sys_Craft;
using MinesServer.GameShit.SysMarket;
using MinesServer.GameShit.VulkSystem;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.Server
{
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
        public DbSet<GLine> lines => Set<GLine>();
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

            modelBuilder.Entity<Clan>()
                .Navigation(c => c.members)
                .AutoInclude();
            modelBuilder.Entity<Clan>()
               .Navigation(c => c.reqs)
               .AutoInclude();
            modelBuilder.Entity<Clan>()
               .Navigation(c => c.ranks)
               .AutoInclude();
            modelBuilder.Entity<Program>()
               .Navigation(c => c.owner)
               .AutoInclude();
            var b = modelBuilder.Entity<Player>();
            b.HasOne(i => i.resp).WithMany().OnDelete(DeleteBehavior.SetNull);
                b.Navigation(c => c.programs)
                .AutoInclude();
            modelBuilder.Entity<Request>()
                .Navigation(c => c.player)
                .AutoInclude();
            modelBuilder.Entity<Crafter>()
                .Navigation(c => c.currentcraft)
                .AutoInclude();
            modelBuilder.Entity<Chat>()
                .Navigation(c => c.messages)
                .AutoInclude();
        }
        public static void Save()
        {
            using var db = new DataBase();
            db.SaveChanges();
            db.Dispose();
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

        public static IQueryable<Resp?> GetResp(int ownerid)
        {
            using var db = new DataBase();
            return db.resps.Where(r => r.ownerid == ownerid);
        }

        // TODO: Сделать получение через World, чтобы ServerTime не имел дотступ к DataBase
        // Либо инкапсулировать полностью
        public static List<Player> activeplayers = new();
        public static List<BotSpot> botspotplayer = new();
        private static readonly Dictionary<int, string> _nicklist = new();
        public static void Load()
        {
            using var db = new DataBase();
            try
            {
                foreach (var i in db.boxes)
                {
                    World.SetCell(i.x, i.y, 90);
                }
                foreach (var i in db.gates)
                {
                    i.Build();
                }
                foreach (var i in db.vulkans)
                {
                    i.Build();
                }
                foreach (var i in db.resps)
                {
                    i.Build();
                }
                foreach (var i in db.markets)
                {
                    i.Build();
                }
                foreach (var i in db.ups)
                {
                    i.Build();
                }
                foreach (var i in db.guns)
                {
                    i.Build();
                }
                foreach (var i in db.storages)
                {
                    i.Build();
                }
                foreach (var i in db.crafts)
                {
                    i.Build();
                }
                foreach (var i in db.teleports)
                {
                    i.Build();
                }
                foreach (var i in db.spots)
                {
                    i.Build();
                }
                foreach (var i in db.ncs)
                {
                    i.Build();
                }
                foreach (var i in db.observatory)
                {
                    i.Build();
                }
                foreach (var i in db.mayak)
                {
                    i.Build();
                }
                foreach (var i in db.jobs)
                {
                    i.Build();
                }
            }
            catch (Exception ex)
            {
                Default.WriteError(ex.ToString());
            }
            db.Dispose();
        }
    }
}
