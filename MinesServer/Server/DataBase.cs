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
        public DbSet<Program> progs { get; set; }
        public DbSet<Player> players { get; set; }
        public DbSet<BotSpot> botspots { get; set; }
        public DbSet<Inventory> inventories { get; set; }
        public DbSet<Basket> baskets { get; set; }
        public DbSet<PlayerSkills> skills { get; set; }
        public DbSet<Settings> settings { get; set; }
        #endregion
        #region Utils
        public DbSet<GLine> lines { get; set; }
        public DbSet<Chat> chats { get; set; }
        public DbSet<Box> boxes { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Clan> clans { get; set; }
        public DbSet<Request> reqs { get; set; }
        public DbSet<Rank> ranks { get; set; }
        public DbSet<CraftEntry> craftentries { get; set; }
        #endregion
        #region packs
        public DbSet<Spot> spots { get; set; }
        public DbSet<Vulkan> vulkans { get; set; }
        public DbSet<Resp> resps { get; set; }
        public DbSet<Market> markets { get; set; }
        public DbSet<Up> ups { get; set; }
        public DbSet<Gun> guns { get; set; }
        public DbSet<Storage> storages { get; set; }
        public DbSet<Crafter> crafts { get; set; }
        public DbSet<Teleport> teleports { get; set; }
        public DbSet<Gate> gates { get; set; }
        public DbSet<NC> ncs { get; set; }
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

        public static Box? GetBox(int x, int y)
        {
            using var db = new DataBase();
            return db.boxes
                .AsNoTracking()
                .FirstOrDefault(t => t.x == x && t.y == y);
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
            }
            catch (Exception ex)
            {
                Default.WriteError(ex.ToString());
            }
            db.Dispose();
        }
    }
}
