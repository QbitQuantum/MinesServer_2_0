using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.VulkSystem;
using MinesServer.Network.Constraints;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.World;
using MinesServer.Server;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace MinesServer.GameShit.WorldSystem
{
    public class Chunk
    {
        public ConcurrentDictionary<int, Player> bots = new();
        public (int x, int y) pos;
        public bool[] packsprop;
        public Chunk((int, int) pos) => this.pos = pos;
        bool ContainsAlive = false;
        public Dictionary<int, Pack> packs = new();
        private byte this[int x, int y]
        {
            get => World.GetCell(WorldX + x, WorldY + y);
            set => World.SetCell(WorldX + x, WorldY + y, value);
        }
        public int WorldX
        {
            get => pos.x * 32;
        }
        public int WorldY
        {
            get => pos.y * 32;
        }
        private DateTime lastupdalive = ServerTime.Now;
        private DateTime sandandb = ServerTime.Now;
        private DateTime notvisibleupd = ServerTime.Now;
        bool shouldbeloaded => ShouldBeLoadedBots() || ContainsAlive || updlasttick;
        public byte[] cells => Enumerable.Range(0, World.ChunkHeight).SelectMany(y => Enumerable.Range(0, World.ChunkWidth).Select(x => this[x, y])).ToArray();
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
            else if (now - notvisibleupd > TimeSpan.FromMinutes(5))
            {
                UpdateNotVisible();
                notvisibleupd = now;
            }
            Dispose();
        }
        public void SetProp(int x, int y, bool packmesh = false)
        {
            LoadPackProps();
            packsprop[x + y * 32] = packmesh ? true : false;
            SendCellToBots(WorldX + x, WorldY + y, this[x, y]);
        }
        public void UpdateNotVisible()
        {
            
            for (int lx = 0; lx < 32; lx++)
            {
                for (int ly = 0; ly < 32; ly++)
                {
                    (int x, int y) d = (WorldX + lx, WorldY + ly);
                    if (World.isCry(World.GetCell(d.x, d.y)))
                    {
                        World.SetDurability(d.x, d.y, World.GetDurability(d.x, d.y) + 1);
                    }
                        /*StaticVulkan.CheckSpace(d.x, d.y).ContinueWith(i =>
                        {
                            if (i.Result > 500)
                                new Vulkan(d.x, d.y).Build();
                        });*/
                }
            }
        }
        public void LoadPackProps()
        {
            if (packsprop is null)
            {
                packsprop = new bool[1024];
                foreach (var p in packs.Values) p.Build();
            }
        }
        public void DestroyCell(int x, int y, World.destroytype t)
        {
                SendCellToBots(WorldX + x, WorldY + y, this[x, y]);
        }
        public void SendDirectedFx(int fx, int x, int y, int dir, int bid = 0, int color = 0)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = pos.x + xxx;
                    var cy = pos.y + yyy;
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        foreach (var id in ch.bots)
                        {
                            DataBase.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBDirectedFXPacket(id.Key, x, y, fx, dir, color)]));
                        }
                    }
                }
            }
        }
        public void SendFx(int x, int y, int fx)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = pos.Item1 + xxx;
                    var cy = pos.Item2 + yyy;
                    if (valid(cx, cy))
                    {
                        var ch = World.W.chunks[cx, cy];
                        foreach (var id in ch.bots)
                        {
                            DataBase.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBFXPacket(x, y, fx)]));
                        }
                    }
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
        public void SendPack(char type, int x, int y, int cid, int off)
        {

                foreach (var ch in vChunksAround()) 
                foreach (var id in ch.bots)
                {
                    ClearPack(x, y);
                    if (type != (char)PackType.None)
                    {
                        var player = DataBase.GetPlayer(id.Key);
                        player?.connection?.SendB(new HBPacket([new HBPacksPacket(PACKPOS(x, y), [new HBPack(type, x, y, (byte)cid, (byte)off)])]));
                    }
                }
        }
        public void ClearDelay(int x, int y)
        {
                foreach (var ch in vChunksAround())
                foreach (var id in ch.bots)
                {
                    var player = DataBase.GetPlayer(id.Key);
                    player?.connection?.SendB(new HBPacket([new HBPacksPacket(PACKPOS( x, y), [])]));
                }
            
        }
        public void ClearPack(int x,int y)
        {
            foreach (var ch in vChunksAround())
                foreach (var id in ch.bots)
                {
                    var player = DataBase.GetPlayer(id.Key);
                    player?.connection?.SendB(new HBPacket([new HBPacksPacket(PACKPOS(x, y), [])]));
                }
        }
        public int PACKPOS(int x, int y) => x + y * World.ChunksW;
        IEnumerable<Chunk> vChunksAround()
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    var cx = pos.x + xxx;
                    var cy = pos.y + yyy;
                    if (valid(cx, cy)) yield return World.W.chunks[cx,cy];
                }
            }
            yield break;
        }
        public IHubPacket[] pPakcs(Player player)
        {
            Dictionary<int, List<HBPack>> l = new();
            foreach (var p in packs.Values)
                if (p.type != PackType.None)
                {
                    var pos = PACKPOS(p.x, p.y);
                    if (!l.ContainsKey(pos)) l.Add(pos, new List<HBPack>());
                        l[pos].Add(new HBPack((char)p.type, p.x, p.y, (byte)p.cid, (byte)p.off));
                };
            return l.Select(i => (IHubPacket)new HBPacksPacket(i.Key, i.Value.ToArray())).ToArray();
        }
        public Pack? GetPack(int x, int y) => packs.ContainsKey(x + y * 32) ? packs[x + y * 32] : null;
        public void SetPack(int x, int y, Pack p)
        {
            packs[x + y * 32] = p;
            if (p.type != PackType.None)
            {
                SendPack((char)p.type, WorldX + x, WorldY + y, p.cid, p.off);
            }
        }
        public void RemovePack(int x, int y)
        {
            if (packs.ContainsKey(x + y * 32))
            {
                packs.Remove(x + y * 32);
                ClearPack(WorldX + x, WorldY + y);
            }
        }
        private void CheckBots()
        {
            foreach (var i in bots)
            {
                if (i.Value.ChunkX != pos.x || i.Value.ChunkY != pos.y || 
                    !DataBase.activeplayers.Contains(i.Value))
                {
                    bots.Remove(i.Value.id, out var p);
                }
            }
        }
        private void UpdateSandBoulders()
        {
            List<(int, int, byte)> cellstoupd = new();
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var prop = World.GetProp(this[x, y]);
                    if (prop.isSand || prop.isBoulder)
                        cellstoupd.Add((WorldX + x, WorldY + y, this[x, y]));
                }
            }
            foreach (var c in cellstoupd)
            {
                if (World.GetProp(c.Item3).isSand && Physics.Sand(c.Item1, c.Item2))
                    updlasttick = true;
                else if (World.GetProp(c.Item3).isBoulder && Physics.Boulder(c.Item1, c.Item2))
                    updlasttick = true;
            }
        }
        private void UpdateAlive()
        {
            List<(int, int, byte)> cellstoupd = new();
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                    if (World.isAlive(this[x, y])) cellstoupd.Add((WorldX + x, WorldY + y, this[x, y]));
            foreach (var c in cellstoupd)
            {
                if (World.isAlive(c.Item3) && Physics.Alive(c.Item1, c.Item2)) 
                    updlasttick = true;
            }
        }
        private void UpdateCells()
        {
            var now = ServerTime.Now;
            if (now - lastupdalive > TimeSpan.FromMilliseconds(5000))
            {
                UpdateAlive();
                lastupdalive = now;
            }
            if (now - sandandb > TimeSpan.FromMilliseconds(400))    
            {
                UpdateSandBoulders();
                sandandb = now;
            }
        }
        public bool updlasttick = false;
        public void AddBot(Player player)
        {
            if (this != null && !bots.ContainsKey(player.id)) 
                bots[player.id] = player;
        }
        public void Dispose()
        {
            World.W.cells.Unload(pos.Item1, pos.Item2);
        }
        private bool ShouldBeLoadedBots()
        {
            foreach (var ch in vChunksAround()) 
                if (ch.bots.Count > 0)
                        return true;
            return false;
        }
        public static bool valid(int x, int y) => x >= 0 && y >= 0 && x < World.ChunksW && y < World.ChunksH;
        private void SendCellToBots(int x, int y, byte cell)
        {
            foreach (var ch in vChunksAround())
                foreach (var id in ch.bots) 
                    DataBase.GetPlayer(id.Key)?.connection?.SendCell(x, y, cell);
        }
    }
}
