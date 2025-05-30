﻿using MinesServer.GameShit.Buildings;
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
        public const int ChunksW = 260;
        public const int ChunksH = 420;
        public const int CellsWidth = ChunksW * ChunkWidth;
        public const int CellsHeight = ChunksH * ChunkHeight;
        public const int ChunkWidth = 32;
        public const int ChunkHeight = 32;
        public const int ChunkVolume = ChunkWidth * ChunkHeight;
        public const int TotalVolume = ChunksAmount * ChunkVolume;
        public const int ChunksAmount = ChunksW * ChunksH;

        public string name { get; private set; }
        public WorldLayer<byte> road;
        public WorldLayer<byte> cells;
        public WorldLayer<float> durability;
        public Chunk[,] chunks;
        public static World W;
        public Gen gen;
        private Dictionary<(int, int), CancellationTokenSource?> shit = new();
        public World(string name)
        {

            W = this;
            this.name = name;
            gen = new Gen(CellsWidth, CellsHeight);
            chunks = new Chunk[ChunksW, ChunksH];
            CreateChunks();
            if (!File.Exists($"{name}.mapb"))
            {
                cells = new($"{name}.mapb", (ChunksW, ChunksH));
                road = new($"{name}_road.mapb", (ChunksW, ChunksH));
                durability = new($"{name}_durability.mapb", (ChunksW, ChunksH));
                Console.WriteLine($"Creating World Preset {CellsWidth} x {CellsHeight}({ChunksW} x {ChunksH} chunks)");
                Console.WriteLine("EmptyMapGeneration");
                gen.StartGeneration();
                /*for(int x = 0;x < CellsWidth;x++)
                {
                    for (int y = 0; y < CellsHeight; y++)
                    {
                        SetCell(x, y, 32);
                    }
                }*/
                Console.WriteLine("Generation End");
            }
            else
            { 
                cells = new($"{name}.mapb", (ChunksW, ChunksH));
                road = new($"{name}_road.mapb", (ChunksW, ChunksH));
                durability = new($"{name}_durability.mapb", (ChunksW, ChunksH));
            }
            CommitWorld();
            CreateSpawns();
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
        public void CreateSpawns()
        {
            var r = new Random();
            using (var db = new DataBase())
            {
                var y = 10;
                var x = 10;
                if (db.reqs.Count() < 1)
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
        public void CreateChunks()
        {
            for (int chx = 0; chx < ChunksW; chx++)
            {
                for (int chy = 0; chy < ChunksH; chy++)
                {
                    chunks[chx, chy] = new Chunk((chx, chy));
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
        public enum destroytype
        {
            Cell,
            Road,
            CellAndRoad
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
        public static void Destroy(int x, int y, destroytype t = destroytype.Cell)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            switch (t)
            {
                case destroytype.Cell:
                    if (W.cells[x, y] != 0)
                    {
                        W.cells[x, y] = 0;
                        W.road[x, y] = W.road[x, y] == 0 ? 32 : W.road[x, y];
                    }
                    break;
                case destroytype.Road:
                    if (W.road[x, y] is not (32 or 37 or 36))
                    {
                        W.road[x, y] = 32;
                    }
                    break;
                case destroytype.CellAndRoad:
                    W.cells[x, y] = 0;
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
            if (!W.ValidCoord(x, y))
            {
                return 0f;
            }
            return W.durability[x, y]!.Value;
        }
        public static void SetDurability(int x, int y, float d)
        {
            if (!W.ValidCoord(x, y))
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
        public static Cell GetProp(int x, int y)
        {
            return W.ValidCoord(x,y) ? GetProp(GetCell(x, y)) : GetProp(0);
         }
        public static void MoveCell(int x, int y, int plusx, int plusy)
        {
            if (!W.ValidCoord(x + plusx, y + plusy)) return;
            var cell = GetCell(x, y);
            var durability = GetDurability(x, y);
            Destroy(x, y, destroytype.Cell);
            SetCell(x + plusx, y + plusy, cell);
            SetDurability(x + plusx, y + plusy, durability);
            W.GetChunk(x + plusx, y + plusy).updlasttick = true;
        }
        public static void SetCell(int x, int y, CellType type) => SetCell(x, y, (byte)type);
        public static void SetCell(int x, int y, byte cell, bool packmesh = false)
        {
            if (!W.ValidCoord(x, y))
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
            if (!W.ValidCoord(x, y))
            {
                return false;
            }
            var ch = W.GetChunk(x, y);
            ch.LoadPackProps();
            return ch.packsprop[x - ch.WorldX + (y - ch.WorldY) * 32];
        }
        public static void AddPack(int x, int y, Pack p)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.SetPack(x - ch.WorldX, y - ch.WorldY, p);
        }
        public static void RemovePack(int x, int y)
        {
            if (!W.ValidCoord(x, y))
            {
                return;
            }
            var ch = W.GetChunk(x, y);
            ch.RemovePack(x - ch.WorldX, y - ch.WorldY);
        }
        public static byte GetCell(int x, int y)
        {
            if (!W.ValidCoord(x, y)) return 0;
            var cell = W.cells[x, y] ?? 0;
            if (cell == 0)
            {
                var r = W.road[x, y] ?? 32;
                return r;
            }
            return cell;
        }
        public async void AsyncAction(int secdelay, Action act)
        {
            await Task.Run(delegate ()
            {
                Thread.Sleep(secdelay * 1000);
                act();
            });
        }
        public async void StupidAction(double delay, int x, int y, Action a)
        {
            CancellationTokenSource s = new();
            shit[(x, y)] = s;
            await Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(delay));
                if (!s.Token.IsCancellationRequested)
                    a();
                shit.Remove((x, y));
                s.Dispose();
            }, s.Token);
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
            if (!W.ValidCoord(x, y))
            {
                p = null;
                return true;
            }
            var chpos = W.GetChunkPosByCoords(x, y);
            var ch = W.chunks[chpos.Item1, chpos.Item2];
            p = ch.GetPack(x - ch.WorldX, y - ch.WorldY)!;
            if (p == null)
            {
                return false;
            }
            return true;
        }
        public static bool isBuildingBlock(byte cell)
        {
            return (CellType)cell switch
            {
                CellType.GreenBlock or CellType.YellowBlock or CellType.RedBlock or CellType.MilitaryBlockFrame or CellType.MilitaryBlock or CellType.Support or CellType.QuadBlock => true,
                _ => false
            };
        }
        public static bool isAlive(byte cell)
        {
            return (CellType)cell switch
            {
                CellType.AliveBlue or CellType.AliveCyan or CellType.AliveRed or CellType.AliveNigger or CellType.AliveViol or CellType.AliveWhite or CellType.AliveRainbow => true,
                _ => false
            };
        }
        public static bool isRoad(byte cell)
        {
            return (CellType)cell switch
            {
                CellType.Road or CellType.GoldenRoad or CellType.PolymerRoad or CellType.BuildingDoor => true,
                _ => false
            };
        }
        public static bool isCry(byte cell)
        {
            return (CellType)cell switch
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
        public bool ValidCoord(int x, int y) => x >= 0 && y >= 0 && x < CellsWidth && y < CellsHeight;
        public (int, int) GetChunkPosByCoords(int x, int y) => ((int)Math.Floor((float)x / 32), (int)Math.Floor((float)y / 32));
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
            var ret = true;
            var anygun = false;
            for (int chx = -21; chx <= 21; chx++)
            {
                for (int chy = -21; chy <= 21; chy++)
                {
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(x + chx, y + chy)) <= 20f)
                    {
                        if (W.ValidCoord(x + chx, y + chy) && ContainsPack(x + chx, y + chy, out var p) && p is Gun)
                        {
                            anygun = true;
                            var gun = p as Gun;
                            if (gun.charge > 0)
                            {
                                ret = ret && gun.cid == cid;
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
        public static void Update()
        {
            if (ServerTime.Now - lazyupd >= TimeSpan.FromMinutes(1))
            {
                foreach (var player in DataBase.activeplayers)
                {
                    player.connection?.SendU(new OnlinePacket(DataBase.activeplayers.Count, 0));
                }
                MarketSystem.GenerateRandomOrders();
                lazyupd = ServerTime.Now;
            }
            if (ServerTime.Now - lastpackupd >= TimeSpan.FromHours(1))
            {
                using var db = new DataBase();
                for (int chx = 0; chx < ChunksW; chx++)
                {
                    for (int chy = 0; chy < ChunksH; chy++)
                    {
                        foreach (var pack in W.chunks[chx, chy].packs)
                        {
                            if (pack.Value != null && pack.Value is IDamagable)
                            {
                                db.Attach(pack.Value);
                                var damagable = pack.Value as IDamagable;
                                damagable?.Damage(2);
                                if (damagable.NeedEffect())
                                {
                                    damagable.SendBrokenEffect();
                                }
                            }
                        }
                    }
                }
                db.SaveChanges();
                lastpackupd = ServerTime.Now;
            }

            if (ServerTime.Now - lastpackeffect >= TimeSpan.FromSeconds(0.5))
            {
                using var db = new DataBase();
                for (int chx = 0; chx < ChunksW; chx++)
                {
                    for (int chy = 0; chy < ChunksH; chy++)
                    {
                        foreach (var pack in W.chunks[chx, chy].packs)
                        {
                            if (pack.Value != null && pack.Value is IDamagable)
                            {
                                var damagable = pack.Value as IDamagable;
                                if (damagable.NeedEffect())
                                {
                                    damagable.SendBrokenEffect();
                                }
                                if (pack.Value != null && pack.Value is Gun)
                                {
                                    var gun = pack.Value as Gun;
                                    db.Attach(gun);
                                    gun.Update();
                                }
                                if (pack.Value != null && pack.Value is Crafter)
                                {
                                    (pack.Value as Crafter).Update();
                                }
                            }
                        }
                    }
                }
                db.SaveChanges();
                lastpackeffect = ServerTime.Now;
            }
            if (ServerTime.Now - lastcryupdate >= TimeSpan.FromHours(1))
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
                lastcryupdate = ServerTime.Now;
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
            var pos = GetChunkPosByCoords(x, y);
            return chunks[pos.Item1, pos.Item2];
        }
    }
}
