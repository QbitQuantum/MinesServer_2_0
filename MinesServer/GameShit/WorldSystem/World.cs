using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Generator;
using MinesServer.GameShit.SysMarket;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.IO.Pipes;
using System.Numerics;
using System.Security.Cryptography;

namespace MinesServer.GameShit.WorldSystem
{
    public class World
    {
        public string name { get; private set; }
        public WorldLayer<byte> road;
        public WorldLayer<byte> cells;
        public WorldLayer<float> durability;
        public Chunk[,] chunks;
        public static World W;

        // ширина мира в клетках
        public const int CellsWidth = Chunk.ChunksW * Chunk.ChunkWidth;
        // высота мира в клетках
        public const int CellsHeight = Chunk.ChunksH * Chunk.ChunkHeight;
        // всего клеток в мире
        public const int TotalVolume = CellsWidth * CellsHeight;

        public Gen gen;
        private Dictionary<(int, int), CancellationTokenSource?> shit = new();
        public World(string name)
        {

            W = this;
            this.name = name;
            gen = new Gen(CellsWidth, CellsHeight);
            chunks = Chunk.CreateAllChunks();

            cells = new($"{name}.mapb", (Chunk.ChunksW, Chunk.ChunksH));
            road = new($"{name}_road.mapb", (Chunk.ChunksW, Chunk.ChunksH));
            durability = new($"{name}_durability.mapb", (Chunk.ChunksW, Chunk.ChunksH));

            if (!File.Exists($"{name}.mapb"))
            {
                Console.WriteLine($"Creating World Preset {CellsWidth} x {CellsHeight}({Chunk.ChunksW} x {Chunk.ChunksH} chunks)");
                Console.WriteLine("EmptyMapGeneration");
                gen.StartGeneration();
                Console.WriteLine("Generation End");
            }
            CreateSpawns();
            CommitWorld();
            using var db = new DataBase();
            if (db.chats.FirstOrDefault(i => i.Name == "FED") == default)
            {
                db.chats.Add(new GChat.Chat("FED", "Федеральный чат"));
                db.chats.Add(new GChat.Chat("DNO", "Дно"));
                db.SaveChanges();
            }
            DataBase.Load();
            Console.WriteLine("Creating chunkmesh");
            Console.WriteLine("LoadConfirmed");
            Console.WriteLine("Started");
            CommitWorld();
            MServer.started = true;
        }
        public static void CommitWorld()
        {
            W.cells.Commit();
            W.road.Commit();
            W.durability.Commit();
        }
        public void DeleteWorld()
        {
            cells.Delete();
            road.Delete();
            durability.Delete();
        }
        private static readonly List<(DateTime TriggerTime, Action Action)> _delayedActions = new();

        public static void ScheduleAction(TimeSpan delay, Action action)
        {
            _delayedActions.Add((ServerTime.Now + delay, action));
        }
        public void CreateSpawns()
        {
            using (var db = new DataBase())
            {
                var y = 10;
                var x = 10;

                // Проверяем только наличие респауна (как ключевой структуры)
                if (!db.resps.Any(r => r.x == x - 8 && r.y == y + 7))
                {
                    for (int rx = -10; rx <= 10; rx++)
                    {
                        for (int ry = -10; ry <= 10; ry++)
                        {
                            SetCell(x + rx, y + ry, 36);
                        }
                    }
                    new Market(x - 7, y - 4, 0).Build();
                    new Resp(x - 8, y + 7, 0).Build();
                    new Up(x, y - 4, 0).Build();
                }
            }
        }
        public bool CanBuildPack(int left, int right, int bottom, int top, int x, int y, Player player, bool ignoreplace = false)
        {
            var h = 0;
            List<IHubPacket> packets = new();
            for (int cx = left; cx <= right; cx++)
            {
                for (int cy = bottom; cy <= top; cy++)
                {
                    var p = GetProp(GetCell(x + cx, y + cy));
                    if (!ValidCoord(x + cx, y + cy) || ignoreplace && (!p.is_diggable || !p.is_destructible || GetCell(x + cx, y + cy) == 36) || PackPart(x + cx, y + cy) || ((player != null) ? !AccessGun(x, y, player.cid).access : false) || (!p.can_place_over || !p.isEmpty) && !ignoreplace)
                    {
                        if (player != null && ValidCoord(x + cx, y + cy))
                        {
                            packets.Add(new HBFXPacket(x + cx, y + cy, 0));
                        }

                        h++;
                    }
                }
            }
            if (h > 0)
            {
                if (packets.Count > 0)
                {
                    player?.connection?.SendB(new HBPacket(packets.ToArray()));
                }
                return false;
            }
            return true;
        }

        public static bool DamageCell(int x, int y, float dmg)
        {
            var d = GetDurability(x, y);
            if (d - dmg <= 0)
            {
                SetDurability(x, y, 0);
                Destroy(x, y);
                return true;
            }
            SetDurability(x, y, d - dmg);
            return false;
        }
        public static void Destroy(int x, int y, DestroyCellType t = DestroyCellType.Cell)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            switch (t)
            {
                case DestroyCellType.Cell:
                    if (W.cells[x, y] != 0)
                    {
                        W.cells[x, y] = 32;
                        W.road[x, y] = W.road[x, y] == 0 ? 32 : W.road[x, y];
                    }
                    break;
                case DestroyCellType.Road:
                    if (W.road[x, y] is not (32 or 37 or 36))
                    {
                        W.road[x, y] = 32;
                    }
                    break;
                case DestroyCellType.CellAndRoad:
                    W.cells[x, y] = 32;
                    if (W.road[x, y] is not (32 or 37 or 36))
                    {
                        W.road[x, y] = 32;
                    }
                    break;
            }
            ch.DestroyCell(x - ch.WorldX, y - ch.WorldY, t);
        }
        public static float GetDurability(int x, int y)
        {
            if (!ValidCoord(x, y))
            {
                return 0f;
            }
            return W.durability[x, y]!.Value;
        }
        public static void SetDurability(int x, int y, float d)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
            W.durability[x, y] = d;
        }
        public void CreateEmptyMap(byte cell)
        {
            int cells = 0;
            var j = DateTime.Now;
            for (int x = 0; x < CellsWidth; x++)
            {
                for (int y = 0; y < CellsHeight; y++)
                {
                    cells += 1;
                    SetCell(x, y, cell);
                }
                if (DateTime.Now - j > TimeSpan.FromSeconds(2))
                {
                    Console.Write($"\r{cells}/{TotalVolume}");
                    j = DateTime.Now;
                }
            }
            Console.Write($"\r{cells}/{TotalVolume}");
            Console.WriteLine("");
        }
        public static Cell GetProp(byte type) => CellsSerializer.cells[type];
        public static bool IsEmpty(int x, int y)
        {
            return GetProp(x, y).isEmpty && !PackPart(x, y);
        }
        public static bool TrueEmpty(int x, int y) => GetProp(x, y).isEmpty && !PackPart(x, y) && GetCell(x, y) is not (36 or 37 or 0 or 39);

        public static bool IsValidEmptyCell(int x, int y)
            => ValidCoord(x, y) && GetProp(x, y).isEmpty && !PackPart(x, y);

        public static Cell GetProp(int x, int y)
        {
            return ValidCoord(x,y) ? GetProp(GetCell(x, y)) : GetProp(0);
        }

        public static bool CanDamageCell(int x, int y)
        {
            var Cell = GetProp(x, y);
            return 
                !isAlive(x, y) && 
                !isBuildingBlock(x, y) &&
                Cell.is_diggable &&
                Cell.is_destructible;
        }

        public static (int x, int y) FindEmptyForBox(int x, int y)
        {
            var dirs = new (int dx, int dy)[] { (0, 1), (1, 0), (-1, 0), (0, -1) };
            var q = new Queue<(int x, int y)>();

            if (IsValidEmptyCell(x, y))
                return (x, y);

            q.Enqueue((x, y));
            var visited = new HashSet<(int, int)> { (x, y) };

            while (q.Count > 0)
            {
                var (cx, cy) = q.Dequeue();
                foreach (var (dx, dy) in dirs)
                {
                    int nx = cx + dx, ny = cy + dy;
                    if (visited.Contains((nx, ny))) continue;

                    if (IsValidEmptyCell(nx, ny))
                        return (nx, ny);

                    visited.Add((nx, ny));
                    q.Enqueue((nx, ny));
                }
            }
            return (x, y);
        }

        public static void MoveCell(int x, int y, int plusx, int plusy)
        {
            if (!ValidCoord(x + plusx, y + plusy)) return;
            var cell = GetCell(x, y);
            var durability = GetDurability(x, y);
            Destroy(x, y, DestroyCellType.Cell);
            SetCell(x + plusx, y + plusy, cell);
            SetDurability(x + plusx, y + plusy, durability);
            W.GetChunk(x + plusx, y + plusy).updlasttick = true;
        }
        public static void SetCell(int x, int y, CellType type) => SetCell(x, y, (byte)type);
        public static void SetCell(int x, int y, byte cell, bool packmesh = false)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            if (GetProp(cell).isEmpty)
            {
                W.cells[x, y] = 0;
                W.road[x, y] = cell;
            }
            else
            {
                W.cells[x, y] = cell;
                W.durability[x, y] = GetProp(cell).durability;
            }
            if (W.shit.ContainsKey((x, y)))
                W.shit[(x, y)]?.Cancel();
            ch.SetProp(x - ch.WorldX, y - ch.WorldY, packmesh);
        }
        public static bool PackPart(int x, int y)
        {
            if (!ValidCoord(x, y))
            {
                return false;
            }
            var ch = W.GetChunk(x, y);
            ch.LoadPackProps();
            return ch.packsprop[x - ch.WorldX + (y - ch.WorldY) * 32];
        }
        public static void AddPack(int x, int y, Pack p)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.SetPack(x - ch.WorldX, y - ch.WorldY, p);
        }
        public static void RemovePack(int x, int y)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.RemovePack(x - ch.WorldX, y - ch.WorldY);
        }
        public static byte GetCell(int x, int y)
        {
            if (!ValidCoord(x, y)) return 0;
            var cell = W.cells[x, y] ?? 0;
            if (cell == 0)
            {
                var r = W.road[x, y] ?? 32;
                return r;
            }
            return cell;
        }
        public Stack<Player> GetPlayersFromPos(int x, int y)
        {
            var st = new Stack<Player>();
            foreach (var id in GetChunk(x, y).bots.Keys)
            {
                var p = DataBase.GetPlayer(id);
                if (p == null)
                    continue;
                if (p.x == x && p.y == y)
                    st.Push(p);

            }
            return st;
        }
        public static bool ContainsPack(int x, int y, out Pack p)
        {
            if (!ValidCoord(x, y))
            {
                p = null;
                return true;
            }
            var ch = W.GetChunk(x, y);
            p = ch.GetPack(x - ch.WorldX, y - ch.WorldY)!;
            if (p == null)
            {
                return false;
            }
            return true;
        }
        public static bool isBuildingBlock(int x, int y)
        {
            return (CellType)(GetCell(x, y)) switch
            {
                CellType.GreenBlock or CellType.YellowBlock or CellType.RedBlock or CellType.MilitaryBlockFrame or CellType.MilitaryBlock or CellType.Support or CellType.QuadBlock => true,
                _ => false
            };
        }
        public static bool isAlive(int x, int y)
        {
            return (CellType)(GetCell(x, y)) switch
            {
                CellType.AliveBlue or CellType.AliveCyan or CellType.AliveRed or CellType.AliveNigger or CellType.AliveViol or CellType.AliveWhite or CellType.AliveRainbow => true,
                _ => false
            };
        }
        public static bool isRoad(int x, int y)
        {
            return (CellType)(GetCell(x, y)) switch
            {
                CellType.Road or CellType.GoldenRoad or CellType.PolymerRoad or CellType.BuildingDoor => true,
                _ => false
            };
        }
        public static bool isCry(int x, int y)
        {
            return (CellType)(GetCell(x, y)) switch
            {
                CellType.XGreen or CellType.Green => true,
                CellType.XBlue or CellType.Blue => true,
                CellType.XRed or CellType.Red => true,
                CellType.XViolet or CellType.Violet => true,
                CellType.White => true,
                CellType.XCyan or CellType.Cyan => true,
                _ => false
            };
        }
        public static bool ValidCoord(int x, int y) => x >= 0 && y >= 0 && x < CellsWidth && y < CellsHeight;
        public static bool ValidChunk(int x, int y) => x >= 0 && y >= 0 && x < Chunk.ChunksW && y < Chunk.ChunksH;
        public void UpdateChunkByCoords(int x, int y)
        {
            var ch = GetChunk(x, y);
            if (ch != null)
            {
                ch.Update();
            }
        }
        public static (bool access,bool anygun) AccessGun(int x, int y, int cid)
        {
            bool ret = true;
            bool anygun = false;

            int minX = Math.Max(0, x - 21);
            int maxX = Math.Min(Chunk.ChunksW - 1, x + 21);
            int minY = Math.Max(0, y - 21);
            int maxY = Math.Min(Chunk.ChunksH - 1, y + 21);

            // Перебираем координаты в найденных пределах
            for (int checkX = minX; checkX <= maxX; checkX++)
            {
                int dx = checkX - x;
                int dxSqr = dx * dx;

                for (int checkY = minY; checkY <= maxY; checkY++)
                {
                    int dy = checkY - y;

                    // Проверяем квадрат расстояния
                    if (dxSqr + dy * dy <= Gun.sqrRadius)
                    {
                        if (ValidCoord(checkX, checkY) && ContainsPack(checkX, checkY, out var p) && p is Gun)
                        {
                            anygun = true;
                            var gun = (Gun)p;
                            if (gun.charge > 0 && gun.cid != cid)
                            {
                                return (false, true);
                            }
                        }
                    }
                }
            }
            return (ret,anygun);
        }
        private static DateTime lastpackupd = ServerTime.Now;
        private static DateTime lastpackeffect = ServerTime.Now;
        private static DateTime lazyupd = ServerTime.Now;
        private static void UpdatePacks(TimeSpan interval, ref DateTime lastUpdate, bool shouldDamage)
        {
            if (ServerTime.Now - lastUpdate < interval)
                return;
            using var db = new DataBase();
            for (int chx = 0; chx < Chunk.ChunksW; chx++)
            {
                for (int chy = 0; chy < Chunk.ChunksH; chy++)
                {
                    foreach (var pack in W.chunks[chx, chy].packs)
                    {
                        if (pack.Value != null && pack.Value is IDamagable damagable)
                        {
                            db.Attach(pack.Value);
                            if (shouldDamage)
                            {
                                damagable?.Damage(2);
                            }
                            if (damagable != null && damagable.NeedEffect())
                            {
                                damagable.SendBrokenEffect();
                            }
                            pack.Value.Update();
                        }
                    }
                }
            }
            db.SaveChanges();
            lastUpdate = ServerTime.Now;
        }

        private static void UpdateOnlineStatus(TimeSpan interval, ref DateTime lastUpdate)
        {
            if (ServerTime.Now - lastUpdate < interval)
                return;
            foreach (var player in DataBase.activeplayers)
            {
                player.connection?.SendU(new OnlinePacket(DataBase.activeplayers.Count, 0));
            }
            lastUpdate = ServerTime.Now;
        }

        private static void UpdateCry(TimeSpan interval, ref DateTime lastUpdate)
        {
            for (int i = 0; i < W.cryscostmod.Length; i++)
            {
                var p = (W.summary[i] + W.summary.Sum()) / 100;
                if (p > 0)
                {
                    if (p > 20 && W.cryscostbase[i] + W.cryscostmod[i] > W.cryscostbase[i])
                    {
                        W.cryscostmod[i] -= 1;
                    }
                    else if (p < 10 && W.cryscostbase[i] + W.cryscostmod[i] < 70)
                    {
                        W.cryscostmod[i] += 1;
                    }
                }
            }
            W.summary = new long[6];
            lastUpdate = ServerTime.Now;
        }

        private static void UpdateDelay()
        {
            for (int i = _delayedActions.Count - 1; i >= 0; i--)
            {
                var (triggerTime, action) = _delayedActions[i];
                if (ServerTime.Now >= triggerTime)
                {
                    _delayedActions.RemoveAt(i);
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Delayed action failed: {ex}");
                    }
                }
            }
        }

        public static void Update()
        {
            // Обновление значение количество онлайн игроков
            UpdateOnlineStatus(TimeSpan.FromMinutes(1), ref lazyupd);

            // Обновление паков с интервалом 1 час (с повреждением)
            UpdatePacks(TimeSpan.FromHours(1), ref lastpackupd, shouldDamage: true);

            // Обновление паков с интервалом 0.5 секунд (без повреждения, только эффекты)
            UpdatePacks(TimeSpan.FromSeconds(0.5), ref lastpackeffect, shouldDamage: false);

            // Обновление цен кристаллов
            UpdateCry(TimeSpan.FromHours(1), ref lastcryupdate);

            // Обновление отложенный действий
            UpdateDelay();
        }

        public static void ChunkUpdate()
        {
            for (int x = 0; x < Chunk.ChunksW; x++)
            {
                for (int y = 0; y < Chunk.ChunksH; y++)
                {
                    W.chunks[x, y]?.Update();
                }
            }
        }

        public static DateTime lastcryupdate = DateTime.MinValue;
        public static int GetCrysCost(int i)
        {
            return W.cryscostbase[i] + W.cryscostmod[i];
        }
        public static void AddDob(int t, long dob)
        {
            W.summary[t] += dob;
        }
        public int[] cryscostmod = { 10, 10, 15, 10, 15, 15 };
        public int[] cryscostbase = { 8, 16, 24, 26, 24, 40 };
        public long[] summary = new long[6];
        public Chunk GetChunk(int x, int y)
        {
            var pos = Chunk.GetChunkPosByCoords(x, y);
            return chunks[pos.x, pos.y];
        }
    }
}
