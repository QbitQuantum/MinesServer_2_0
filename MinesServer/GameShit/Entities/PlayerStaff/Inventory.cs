using System.Numerics;
using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Rich;
using MinesServer.GameShit.VulkSystem;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.GUI;
using MinesServer.Server;
using Newtonsoft.Json;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Inventory
    {
        public Inventory()
        {
            itemstobd ??= JsonConvert.SerializeObject(new Dictionary<int, int>());
            _itemUsages = CreateItemUsages();
        }

        private const int MiniQueueCapacity = 4;
        private const int DischargeBombRadius = 10;

        public int id { get; set; }
        public bool minv = true;
        public int selected = -1;
        public string? itemstobd { get; set; }
        private readonly List<int> _miniQueue = [];
        private readonly Dictionary<int, ItemUsage> _itemUsages;

        private delegate bool ItemUsage(Player player);

        private Dictionary<int, int>? _itemsCache;

        public int this[int itemId]
        {
            get
            {
                if (!Items.TryGetValue(itemId, out var count))
                    Items[itemId] = count = 0;
                return count;
            }
            set
            {
                Items[itemId] = value;

                using var db = new DataBase();
                DataBase.GetPlayer(id)?.SendInventory();
                db.Save(this, x => x.itemstobd);
            }
        }

        private Dictionary<int, int> Items
        {
            get => _itemsCache ??= DeserializeItems(itemstobd);
            set
            {
                _itemsCache = value;
                itemstobd = JsonConvert.SerializeObject(value);
            }
        }

        public InventoryPacket BuildInventoryPacket()
        {
            var slots = CountNonEmptySlots();

            if (_miniQueue.Count < MiniQueueCapacity && slots > 0)
                foreach (var itemId in Items.Keys.ToList())
                    AddToMiniQueue(itemId);

            var grid = minv
                ? _miniQueue.ToDictionary(itemId => itemId, itemId => this[itemId])
                : Items;

            return new InventoryPacket(new InventoryShowPacket(grid, selected, CountNonEmptySlots()));
        }

        public void UseSelectedItem(Player player)
        {
            if (!player.HandleUseInventory() || selected < 0)
                return;

            if (!_itemUsages.TryGetValue(selected, out var usage))
                return;

            var (x, y) = player.GetDirCord();
            var selectedItem = ItemTypeExt.GetItemById(selected);

            var canPlaceHere =
                !World.ContainsPack(x, y, out _) &&
                (World.GetProp(x, y).can_place_over || selectedItem.CanBePlacedAnywhere()) &&
                this[selected] > 0;

            if (!canPlaceHere || !usage(player))
                return;

            this[selected]--;
            if (this[selected] == 0)
                _miniQueue.Remove(selected);

            player.SendInventory();
        }

        public void SelectItem(int itemId, Player player)
        {
            AddToMiniQueue(itemId);
            selected = itemId;

            var packet = itemId == -1
                ? InventoryPacket.Close()
                : InventoryPacket.Choose(
                    "Выбран предмет - " + ItemTypeExt.PackName(itemId),
                    new bool[0, 0], 123, 123, 12);

            player.SendInventory();
            player.connection?.SendU(packet);
        }

        private static Dictionary<int, int> DeserializeItems(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return [];
            return JsonConvert.DeserializeObject<Dictionary<int, int>>(json) ?? [];
        }

        private int CountNonEmptySlots()
        {
            var emptyKeys = Items
                .Where(kv => kv.Value <= 0)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var key in emptyKeys)
                Items.Remove(key);

            return Items.Count;
        }

        private void AddToMiniQueue(int itemId)
        {
            if (itemId == -1 || _miniQueue.Contains(itemId))
                return;

            if (_miniQueue.Count >= MiniQueueCapacity)
                _miniQueue.RemoveAt(0);

            _miniQueue.Add(itemId);
        }

        private static Dictionary<int, ItemUsage> CreateItemUsages()
        {
            return new Dictionary<int, ItemUsage>
            {
                { (int)Item.Teleport,        p => TryBuildPack(p, (x, y) => new Teleport(x, y, p.id), -2, 2, -2, 1) },
                { (int)Item.Respawn,         p => TryBuildPack(p, (x, y) => new Resp(x, y, p.id),     -2, 6, -2, 3) },
                { (int)Item.SpotBot,         p => TryBuildPack(p, (x, y) => new Spot(x, y, p.id),      1, 1,  1, 1) },
                { (int)Item.Up,              p => TryBuildPack(p, (x, y) => new Up(x, y, p.id),       -2, 2, -3, 4) },
                { (int)Item.Market,          p => TryBuildPack(p, (x, y) => new Market(x, y, p.id),   -3, 3, -3, 3) },
                { (int)Item.ScienceCenter,   p => TryBuildPack(p, (x, y) => new NC(x, y, p.id),        1, 1,  1, 1) },
                { (int)Item.Crafter,         p => TryBuildPack(p, (x, y) => new Crafter(x, y, p.id),  -2, 2, -2, 2) },
                { (int)Item.Storage,         p => TryBuildPack(p, (x, y) => new Storage(x, y, p.id),  -2, 2, -2, 1) },

                { (int)Item.Gun, p =>
                    {
                        if (p.clan == null) return false;
                        var (x, y) = p.GetDirCord(true);
                        if (!World.CanBuildPack(-2, 2, -2, 2, x, y, p)) return false;
                        new Gun(x, y, p.id, p.cid).Build();
                        return true;
                    }
                },

                { (int)Item.ClanGate, p =>
                    {
                        if (p.clan == null) return false;
                        var (x, y) = p.GetDirCord();
                        var access = World.AccessGun(x, y, p.cid);
                        if (!access.access || !access.anygun) return false;

                        var gate = new Gate(x, y, p.cid);
                        gate.Build();
                        using var db = new DataBase();
                        db.gates.Add(gate);
                        db.SaveChanges();
                        return true;
                    }
                },

                { (int)Item.Disassembler, p =>
                    {
                        var (x, y) = p.GetDirCord(true);
                        var pack = World.GetPack(x, y);
                        if (pack == null) return false;
                        if (p.id != pack.ownerid && p.cid != pack.cid) return false;

                        var owner = p.id == pack.ownerid ? p : DataBase.GetPlayer(pack.ownerid);
                        if (owner == null) return false;

                        pack.Destroy(owner);
                        return true;
                    }
                },

                { (int)Item.PlasmaBomb,      DetonatePlasmaBomb },
                { (int)Item.ProtonBomb,      DetonateProtonBomb },
                { (int)Item.DischargeBomb,   DetonateDischargeBomb },
                { (int)Item.Polymer,         PlacePolymer },
                { (int)Item.C190,            FireC190 },

                { (int)Item.Geopack,         p => HandleGeopack(p, GeopackMode.Collect) },
                { (int)Item.GeopackBlue,     p => HandleGeopack(p, GeopackMode.Place, CellType.AliveBlue) },
                { (int)Item.GeopackRed,      p => HandleGeopack(p, GeopackMode.Place, CellType.AliveRed) },
                { (int)Item.GeopackPurple,   p => HandleGeopack(p, GeopackMode.Place, CellType.AliveViol) },
                { (int)Item.GeopackBlack,    p => HandleGeopack(p, GeopackMode.Place, CellType.AliveBlack) },
                { (int)Item.GeopackWhite,    p => HandleGeopack(p, GeopackMode.Place, CellType.AliveWhite) },
                { (int)Item.GeopackCyan,     p => HandleGeopack(p, GeopackMode.Place, CellType.AliveCyan) },
                { (int)Item.GeopackHypnoskal, p => HandleGeopack(p, GeopackMode.Place, CellType.HypnoRock) },
                { (int)Item.GeopackBlackSkal, p => HandleGeopack(p, GeopackMode.Place, CellType.BlackRock) },
                { (int)Item.GeopackRedSkal,   p => HandleGeopack(p, GeopackMode.Place, CellType.RedRock) },
                { (int)Item.GeopackRainbow,   p => HandleGeopack(p, GeopackMode.Place, CellType.AliveRainbow) },

                { (int)Item.DefenseCharge, _ => false },

                { (int)Item.NanoBot, p =>
                    {
                        p.AddHp(50);
                        p.SendDFToBots(5, 0, 0, p.id, 0);
                        return true;
                    }
                },

                { (int)Item.RepairBot, p =>
                    {
                        p.ResetHp();
                        p.SendDFToBots(5, 0, 0, p.id, 0);
                        return true;
                    }
                },

                { (int)Item.VolcanoRadar, p => ShowVolcanoInfo(p) },
            };
        }

        private static bool TryBuildPack(
            Player player,
            Func<int, int, Pack> factory,
            int minX, int maxX, int minY, int maxY)
        {
            var (x, y) = player.GetDirCord(true);
            if (!World.CanBuildPack(minX, maxX, minY, maxY, x, y, player))
                return false;

            factory(x, y).Build();
            return true;
        }

        private static bool DetonatePlasmaBomb(Player player)
        {
            var (x, y) = player.GetDirCord();
            if (!World.AccessGun(x, y, player.cid).access) return false;

            var bomb = new Bomb(player.id, x, y, BombType.PlasmaBomb);
            bomb.Build();

            World.ScheduleAction(TimeSpan.FromSeconds(1), () =>
            {
                ForEachCellInRadius(x, y, 3.5f, (tx, ty) =>
                {
                    foreach (var target in World.W.GetPlayersFromPos(tx, ty))
                        target.Hurt(40);

                    if (!World.GetProp(World.GetCell(tx, ty)).is_destructible || World.PackPart(tx, ty))
                        return;

                    ApplyPlasmaDestruction(tx, ty);
                });

                bomb.Destroy(player);
                World.W.SendDirectedFx(1, x, y, 3, 0);
            });

            return true;
        }

        private static void ApplyPlasmaDestruction(int x, int y)
        {
            var cell = World.GetCell(x, y);
            switch (cell)
            {
                case 117:
                    if (Physics.r.Next(1, 101) > 98)
                        World.SetCell(x, y, 118);
                    break;
                case 118:
                    World.SetCell(x, y, 103);
                    break;
                default:
                    World.Destroy(x, y, DestroyCellType.CellAndRoad);
                    break;
            }
        }

        private static bool DetonateProtonBomb(Player player)
        {
            var (x, y) = player.GetDirCord();
            if (!World.AccessGun(x, y, player.cid).access) return false;

            var bomb = new Bomb(player.id, x, y, BombType.ProtonBomb);
            bomb.Build();

            World.ScheduleAction(TimeSpan.FromSeconds(2), () =>
            {
                ForEachCellInRadius(x, y, 3.5f, (tx, ty) =>
                {
                    foreach (var target in World.W.GetPlayersFromPos(tx, ty))
                    {
                        target.Hurt(50);
                        if (World.ContainsPack(tx, ty, out var pack) && pack is Gate gate)
                            gate.Destroy(target);
                    }

                    if (World.GetProp(World.GetCell(tx, ty)).is_destructible && !World.PackPart(tx, ty))
                        World.Destroy(tx, ty, DestroyCellType.CellAndRoad);
                });

                bomb.Destroy(player);
                World.W.SendDirectedFx(1, x, y, 1, 1);
            });

            return true;
        }

        private static bool DetonateDischargeBomb(Player player)
        {
            var (x, y) = player.GetDirCord();

            var bomb = new Bomb(player.id, x, y, BombType.DischargeBomb);
            bomb.Build();

            World.ScheduleAction(TimeSpan.FromSeconds(5), () =>
            {
                using var db = new DataBase();

                ForEachCellInRadius(x, y, DischargeBombRadius - 0.5f, (tx, ty) =>
                {
                    foreach (var target in World.W.GetPlayersFromPos(tx, ty))
                        target.Hurt(500);

                    if (!World.ContainsPack(tx, ty, out var pack) || pack is not PackCharge charge)
                        return;

                    if (charge.CanDestroy())
                        charge.Destroy(player);
                    else
                        charge.Damage(10, DamageTypePacks.Raz);
                });

                db.SaveChanges();
                bomb.Destroy(player);
                World.W.SendDirectedFx(1, x, y, 9, 2);
            });

            return true;
        }

        private static void ForEachCellInRadius(int cx, int cy, float radius, Action<int, int> action)
        {
            int intRadius = (int)MathF.Ceiling(radius);
            var center = new Vector2(cx, cy);

            for (int dx = -intRadius; dx <= intRadius; dx++)
                for (int dy = -intRadius; dy <= intRadius; dy++)
                {
                    int tx = cx + dx, ty = cy + dy;
                    if (!World.ValidCoord(tx, ty)) continue;
                    if (Vector2.Distance(center, new Vector2(tx, ty)) > radius) continue;
                    action(tx, ty);
                }
        }

        private enum GeopackMode { Collect, Place }

        private static bool HandleGeopack(Player player, GeopackMode mode, CellType placeCell = default)
        {
            var (x, y) = player.GetDirCord();

            return mode switch
            {
                GeopackMode.Collect => CollectGeopack(player, x, y),
                GeopackMode.Place => PlaceGeopack(player, x, y, placeCell),
                _ => false,
            };
        }

        private static bool CollectGeopack(Player player, int x, int y)
        {
            if (World.TrueEmpty(x, y)) return false;

            var cell = (CellType)World.GetCell(x, y);
            var itemId = MapCellToGeopackItem(cell);
            if (itemId == -1) return false;

            World.Destroy(x, y);
            player.inventory[itemId]++;
            return true;
        }

        private static bool PlaceGeopack(Player player, int x, int y, CellType cell)
        {
            if (!World.TrueEmpty(x, y)) return false;
            World.SetCell(x, y, cell);
            return true;
        }

        private static int MapCellToGeopackItem(CellType cell) => cell switch
        {
            CellType.AliveCyan => (int)Item.GeopackCyan,
            CellType.AliveRed => (int)Item.GeopackRed,
            CellType.AliveViol => (int)Item.GeopackPurple,
            CellType.AliveBlack => (int)Item.GeopackBlack,
            CellType.AliveWhite => (int)Item.GeopackWhite,
            CellType.AliveBlue => (int)Item.GeopackBlue,
            CellType.HypnoRock => (int)Item.GeopackHypnoskal,
            CellType.BlackRock => (int)Item.GeopackBlackSkal,
            CellType.RedRock => (int)Item.GeopackRedSkal,
            CellType.AliveRainbow => (int)Item.GeopackRainbow,
            _ => -1,
        };

        private static bool PlacePolymer(Player player)
        {
            var (x, y) = player.GetDirCord();
            if (!World.AccessGun(x, y, player.cid).access) return false;
            if (!World.TrueEmpty(x, y)) return false;

            World.SetCell(x, y, CellType.PolymerRoad);
            return false;
        }

        private static bool FireC190(Player player)
        {
            var (startX, startY) = player.GetDirCord();
            var (targetX, targetY) = GetShotTargetByDirection(startX, startY, player.dir, 9);

            if (!World.ValidCoord(targetX, targetY)) return false;

            player.SendDFToBots(7, targetX, targetY, player.id, 1);
            ProcessShotLine(startX, startY, targetX, targetY);
            return true;
        }

        private static bool ShowVolcanoInfo(Player player)
        {
            using var db = new DataBase();
            var vulkans = db.vulkans.ToList();

            if (vulkans.Count == 0)
            {
                player.win = new Window()
                {
                    Title = "Радар вулканов",
                    Tabs = [new Tab()
                    {
                        Action = "volcano_radar",
                        Label = "Вулканы",
                        InitialPage = new Page()
                        {
                            Buttons = [new MButton()],
                            Text = "Вулканов не обнаружено"
                        }
                    }]
                };
                player.SendWindow();
                return true;
            }

            var entries = new List<RichListEntry>();

            foreach (var v in vulkans)
            {
                string state = v.state switch
                {
                    VulkanState.Sleeping => "<color=gray>СПИТ</color>",
                    VulkanState.Warning => "<color=yellow>ПРЕДУПРЕЖДЕНИЕ</color>",
                    _ => "?"
                };

                int dist = Math.Abs(player.x - v.x) + Math.Abs(player.y - v.y);
                string timeInfo = v.state switch
                {
                    VulkanState.Sleeping => $"До пробуждения: {Math.Max(0, (int)((v.wakeTime - ServerTime.Now).TotalMinutes))} мин.",
                    VulkanState.Warning => $"До взрыва: {Math.Max(0, (int)((v.explodeTime - ServerTime.Now).TotalMinutes))} мин.",
                    _ => "?"
                };

                entries.Add(RichListEntry.Text(
                    $"Вулкан [{v.x},{v.y}]",
                    $"Дистанция: {dist} кл.<br/>Статус: {state}<br/>Интенсивность: {v.intensity}/5<br/>{timeInfo}"));
            }

            player.win = new Window()
            {
                Title = "Радар вулканов",
                Tabs = [new Tab()
                {
                    Action = "volcano_radar",
                    Label = "Вулканы",
                    InitialPage = new Page()
                    {
                        Buttons = [new MButton()],
                        RichList = new RichListConfig()
                        {
                            Entries = entries.ToArray()
                        }
                    }
                }]
            };
            player.SendWindow();
            return true;
        }

        private static void ProcessShotLine(int startX, int startY, int targetX, int targetY)
        {
            int stepX = Math.Sign(targetX - startX);
            int stepY = Math.Sign(targetY - startY);

            int x = startX, y = startY;
            while (true)
            {
                ProcessShotCell(x, y);
                if (x == targetX && y == targetY) break;
                x += stepX;
                y += stepY;
            }
        }

        private static void ProcessShotCell(int x, int y)
        {
            foreach (var target in World.W.GetPlayersFromPos(x, y))
                target.HurtC190Stacks();

            if (World.CanDamageCell(x, y))
                World.DamageCell(x, y, 50);
        }

        private static (int x, int y) GetShotTargetByDirection(int startX, int startY, int direction, int radius)
            => direction switch
            {
                0 => (startX, startY + radius),
                1 => (startX - radius, startY),
                2 => (startX, startY - radius),
                3 => (startX + radius, startY),
                _ => throw new ArgumentException($"Invalid direction: {direction}", nameof(direction)),
            };
    }
}