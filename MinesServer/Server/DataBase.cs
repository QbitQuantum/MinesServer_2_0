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

        // TODO: Сделать получение через World, чтобы ServerTime не имел дотступ к DataBase
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

        private static void LoadBoxBuild(DataBase db)
        {
            foreach (var box in db.boxes)
            {
                World.SetCell(box.x, box.y, 90);
            }
        }

        public static void Load()
        {
            using var db = new DataBase();
            try
            {
                LoadBoxBuild(db);
                LoadPacksBuild(db);
            }
            catch (Exception ex)
            {
                Default.WriteError(ex.ToString());
            }
        }
    }
}
