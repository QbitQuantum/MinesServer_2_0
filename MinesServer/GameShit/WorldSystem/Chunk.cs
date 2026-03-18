using System.Collections.Concurrent;
using System.Security.Cryptography;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.VulkSystem;
using MinesServer.Network.Constraints;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.WorldSystem
{
    public class Chunk
    {
        // ширина мира в чанках
        public const int ChunksW = 65;
        // высота мира в чанках
        public const int ChunksH = 105;

        // ширина чанка в клетках
        public const int ChunkWidth = 32;
        // высота чанка в клетках
        public const int ChunkHeight = 32;

        // всего чанков в мире
        public const int ChunksAmount = ChunksW * ChunksH;
        // клеток в одном чанке
        public const int ChunkVolume = ChunkWidth * ChunkHeight;
        
        private const int VIEW_RADIUS = 2;
        private const int ALIVE_UPDATE_MS = 5000;
        private const int SAND_UPDATE_MS = 400;
        private const int NOT_VISIBLE_TIMEOUT_MINUTES = 5;
        private const int CHUNK_SHIFT = 5;
        public ConcurrentDictionary<int, Player> bots { get; } = new();
        public (int x, int y) pos { get; }
        public bool[] packsprop { get; private set; }
        public Dictionary<int, Pack> packs { get; } = new();

        private bool ContainsAlive = false;
        private DateTime lastupdalive = ServerTime.Now;
        private DateTime sandandb = ServerTime.Now;
        private DateTime notvisibleupd = ServerTime.Now;
        public bool updlasttick = false;

        public Chunk((int x, int y) pos)
        {
            this.pos = pos;
        }
        public int WorldX => pos.x * ChunkWidth;
        public int WorldY => pos.y * ChunkHeight;

        private bool shouldbeloaded => ShouldBeLoadedBots() || ContainsAlive || updlasttick;

        public byte[] cells => Enumerable.Range(0, ChunkHeight)
            .SelectMany(y => Enumerable.Range(0, ChunkWidth)
                .Select(x => this[x, y]))
            .ToArray();

        private byte this[int x, int y]
        {
            get => World.GetCell(WorldX + x, WorldY + y);
            set => World.SetCell(WorldX + x, WorldY + y, value);
        }

        #region Чанк менеджмент

        public void Update()
        {
            var now = ServerTime.Now;

            if (shouldbeloaded)
            {
                CheckBots();
                updlasttick = false;
                UpdateCells();
                return;
            }

            if (now - notvisibleupd > TimeSpan.FromMinutes(NOT_VISIBLE_TIMEOUT_MINUTES))
            {
                UpdateNotVisible();
                notvisibleupd = now;
            }

            Dispose();
        }

        private void CheckBots()
        {
            var botsToRemove = bots.Values
                .Where(bot => {
                    var chunkPos = GetChunkPosByCoords(bot.x, bot.y);
                    return chunkPos.x != pos.x || chunkPos.y != pos.y ||
                           !DataBase.activeplayers.Contains(bot);
                })
                .Select(bot => bot.id)
                .ToList();

            foreach (var botId in botsToRemove)
                bots.TryRemove(botId, out _);
        }

        private void UpdateCells()
        {
            var now = ServerTime.Now;

            if (now - lastupdalive > TimeSpan.FromMilliseconds(ALIVE_UPDATE_MS))
            {
                UpdateAlive();
                lastupdalive = now;
            }

            if (now - sandandb > TimeSpan.FromMilliseconds(SAND_UPDATE_MS))
            {
                UpdateSandBoulders();
                sandandb = now;
            }
        }

        private bool ShouldBeLoadedBots()
        {
            return GetNeighboringChunks().Any(ch => ch.bots.Count > 0);
        }

        public void AddBot(Player player)
        {
            if (!bots.ContainsKey(player.id))
                bots[player.id] = player;
        }

        public void Dispose()
        {
            World.W.cells.Unload(pos.x, pos.y);
        }

        #endregion

        #region Обход соседних чанков

        public IEnumerable<Chunk> GetNeighboringChunks(int radius = VIEW_RADIUS)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int x = pos.x + dx;
                    int y = pos.y + dy;

                    if (World.ValidChunk(x, y))
                        yield return World.W.GetPosChunk(x, y);
                }
            }
        }

        public IEnumerable<(int x, int y)> GetNeighboringChunkCoordinates(int radius = VIEW_RADIUS)
        {
            foreach (var chunk in GetNeighboringChunks(radius))
                yield return (chunk.pos.x, chunk.pos.y);
        }

        // Для обратной совместимости
        [Obsolete("Use GetNeighboringChunks() instead")]
        private IEnumerable<Chunk> vChunksAround() => GetNeighboringChunks();

        #endregion

        #region Отправка сообщений

        public void SendDirectedFx(int fx, int x, int y, int dir, int bid = 0, int color = 0)
        {
            foreach (var chunk in GetNeighboringChunks())
            {
                foreach (var id in chunk.bots)
                {
                    DataBase.GetPlayer(id.Key)?.connection?.SendB(
                        new HBPacket([new HBDirectedFXPacket(id.Key, x, y, fx, dir, color)]));
                }
            }
        }

        public void SendFx(int x, int y, int fx)
        {
            foreach (var chunk in GetNeighboringChunks())
            {
                foreach (var id in chunk.bots)
                {
                    DataBase.GetPlayer(id.Key)?.connection?.SendB(
                        new HBPacket([new HBFXPacket(x, y, fx)]));
                }
            }
        }

        public void SendCellToBots(int x, int y, byte cell)
        {
            foreach (var chunk in GetNeighboringChunks())
            {
                foreach (var id in chunk.bots)
                {
                    DataBase.GetPlayer(id.Key)?.connection?.SendCell(x, y, cell);
                }
            }
        }

        public void SendPack(char type, int x, int y, int cid, int off)
        {
            foreach (var chunk in GetNeighboringChunks())
            {
                foreach (var id in chunk.bots)
                {
                    ClearPack(x, y);
                    if (type != (char)PackType.None)
                    {
                        var player = DataBase.GetPlayer(id.Key);
                        player?.connection?.SendB(new HBPacket([
                            new HBPacksPacket(PACKPOS(x, y),
                            [new HBPack(type, x, y, (byte)cid, (byte)off)])
                        ]));
                    }
                }
            }
        }

        public void ClearPack(int x, int y)
        {
            foreach (var chunk in GetNeighboringChunks())
            {
                foreach (var id in chunk.bots)
                {
                    DataBase.GetPlayer(id.Key)?.connection?.SendB(
                        new HBPacket([new HBPacksPacket(PACKPOS(x, y), [])]));
                }
            }
        }

        public void ResendPack(Pack p)
        {
            if (p.type != PackType.None)
            {
                SendPack((char)p.type, p.x, p.y, p.cid, p.off);
            }
        }

        #endregion

        #region Обновление клеток

        public void UpdateNotVisible()
        {
            for (int lx = 0; lx < ChunkWidth; lx++)
            {
                for (int ly = 0; ly < ChunkHeight; ly++)
                {
                    int worldX = WorldX + lx;
                    int worldY = WorldY + ly;

                    if (World.isCry(worldX, worldY))
                    {
                        int durability = (int)(World.GetDurability(worldX, worldY) + 1);
                        World.SetDurability(worldX, worldY, durability);
                    }
                }
            }
        }

        private void UpdateSandBoulders()
        {
            var cellsToUpdate = new List<(int x, int y, byte cell)>();

            for (int y = 0; y < ChunkWidth; y++)
            {
                for (int x = 0; x < ChunkHeight; x++)
                {
                    byte cell = this[x, y];
                    var prop = World.GetProp(cell);

                    if (prop.isSand || prop.isBoulder)
                        cellsToUpdate.Add((WorldX + x, WorldY + y, cell));
                }
            }

            foreach (var (worldX, worldY, cell) in cellsToUpdate)
            {
                var prop = World.GetProp(cell);
                if (prop.isSand && Physics.Sand(worldX, worldY))
                    updlasttick = true;
                else if (prop.isBoulder && Physics.Boulder(worldX, worldY))
                    updlasttick = true;
            }
        }

        private void UpdateAlive()
        {
            var cellsToUpdate = new List<(int x, int y, byte cell)>();

            for (int y = 0; y < ChunkWidth; y++)
            {
                for (int x = 0; x < ChunkHeight; x++)
                {
                    byte cell = this[x, y];
                    if (World.isAlive(x, y))
                        cellsToUpdate.Add((WorldX + x, WorldY + y, cell));
                }
            }

            foreach (var (worldX, worldY, cell) in cellsToUpdate)
            {
                if (World.isAlive(worldX, worldY) && Physics.Alive(worldX, worldY))
                    updlasttick = true;
            }
        }

        #endregion

        #region Pack менеджмент

        public void SetProp(int x, int y, bool packmesh = false)
        {
            LoadPackProps();
            packsprop[x + y * ChunkWidth] = packmesh;
            SendCellToBots(WorldX + x, WorldY + y, this[x, y]);
        }

        public void LoadPackProps()
        {
            if (packsprop != null)
                return;

            packsprop = new bool[ChunkVolume];
            foreach (var p in packs.Values)
                p.Build();
        }

        public void DestroyCell(int x, int y, DestroyCellType t)
        {
            SendCellToBots(WorldX + x, WorldY + y, this[x, y]);
        }

        public int PACKPOS(int x, int y) => x + y * ChunksW;

        public IHubPacket[] pPakcs()
        {
            var packGroups = new Dictionary<int, List<HBPack>>();

            foreach (var p in packs.Values.Where(p => p.type != PackType.None))
            {
                int pos = PACKPOS(p.x, p.y);
                if (!packGroups.ContainsKey(pos))
                    packGroups[pos] = new List<HBPack>();

                packGroups[pos].Add(new HBPack((char)p.type, p.x, p.y, (byte)p.cid, (byte)p.off));
            }

            return packGroups
                .Select(g => (IHubPacket)new HBPacksPacket(g.Key, g.Value.ToArray()))
                .ToArray();
        }

        public Pack? GetPack(int x, int y)
        {
            int key = x + y * ChunkWidth;
            return packs.TryGetValue(key, out var pack) ? pack : null;
        }

        public void SetPack(int x, int y, Pack p)
        {
            int key = x + y * ChunkWidth;
            packs[key] = p;

            if (p.type != PackType.None)
            {
                SendPack((char)p.type, WorldX + x, WorldY + y, p.cid, p.off);
            }
        }

        public void RemovePack(int x, int y)
        {
            int key = x + y * ChunkWidth;
            if (packs.Remove(key))
            {
                ClearPack(WorldX + x, WorldY + y);
            }
        }

        public HBMapPacket MapPacket()
        {
            return new HBMapPacket(
                WorldX,
                WorldY,
                ChunkWidth,
                ChunkHeight,
                cells);
        }

        /// <summary>
        /// Создаём пакеты для удаления всех паков в чанке
        /// </summary>
        public IEnumerable<IHubPacket> PackEmptyPacket()
        {
            foreach (var pack in packs.Values)
                yield return new HBPacksPacket(PACKPOS(pack.x, pack.y), []);
        }

        #endregion
        // Быстрое деление через сдвиг
        private static int GetChunkPosCoordsX(int x) => x >> CHUNK_SHIFT;
        private static int GetChunkPosCoordsY(int y) => y >> CHUNK_SHIFT;
        public static (int x, int y) GetChunkPosByCoords(int x, int y) => (GetChunkPosCoordsX(x), GetChunkPosCoordsY(y));
    }
}
