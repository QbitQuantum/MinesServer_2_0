using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace MinesServer.GameShit.Consumables
{
    public static class ShitClass
    {
        public static bool C190Shot(Player p)
        {
            var d = p.GetDirCord();
            int x = d.x, y = d.y;
            var valid = (byte cell) => !World.isAlive(cell) && World.GetProp(cell).is_diggable && World.GetProp(cell).is_destructible && !World.isBuildingBlock(cell);
            int shotx = 0;
            int shoty = 0;
            switch (p.dir)
            {
                case 0:
                    shoty = y + 9;
                    if (!World.W.ValidCoord(0, shoty)) return false;
                    p.SendDFToBots(7, x, shoty, p.id, 1);
                    for (; y <= shoty; y++)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (valid(c))
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    return true;
                case 1:
                    shotx = x - 9;
                    if (!World.W.ValidCoord(shotx, 0)) return false;
                    p.SendDFToBots(7, shotx, y, p.id, 1);
                    for (; x >= shotx; x--)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (valid(c))
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    return true;
                case 2:
                    shoty = y - 9;
                    if (!World.W.ValidCoord(0, shoty)) return false;
                    p.SendDFToBots(7, x, shoty, p.id, 1);
                    for (; y >= shoty; y--)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (valid(c))
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    return true;
                case 3:
                    shotx = x + 9;
                    if (!World.W.ValidCoord(shotx, 0)) return false;
                    p.SendDFToBots(7, shotx, y, p.id, 1);
                    for (; x <= shotx; x++)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (valid(c))
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    return true;

            }
            return false;
        }
        public static void Gate(int x,int y,Player p)
        {
            using var db = new DataBase();
            db.gates.Add(new Gate(x, y, p.cid));
            db.SaveChanges();
        }
        public static bool Poli(Player p)
        {
            var d = p.GetDirCord();
            int x = d.x, y = d.y;
            if (!World.AccessGun(x, y, p.cid).access) return false;
            if (World.TrueEmpty(x, y))
                World.SetCell(x, y, CellType.PolymerRoad);
            return false;
        }
        public static bool Boom(Player player)
        {
            var d = player.GetDirCord();
            int x = d.x, y = d.y;
            if (!World.AccessGun(x, y, player.cid).access) return false;
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 0);
            World.W.AsyncAction(1, () =>
            {
                for (int _x = -4; _x <= 4; _x++)
                {
                    for (int _y = -4; _y <= 4; _y++)
                    {
                        if (World.W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2(x + _x, y + _y)) <= 3.5f)
                        {
                            foreach (var p in World.W.GetPlayersFromPos(x + _x, y + _y))
                            {
                                p.Hurt(40);
                            }
                            var c = World.GetCell(x + _x, y + _y);
                            if (World.GetProp(c).is_destructible && !World.PackPart(x + _x, y + _y))
                            {
                                if (c == 117 && Physics.r.Next(1, 101) > 98)
                                {
                                    World.SetCell(x + _x, y + _y, 118);
                                }
                                else if (c == 118)
                                {
                                    World.SetCell(x + _x, y + _y, 103);
                                }
                                else if (c != 117 && c != 118)
                                {
                                    World.Destroy(x + _x, y + _y, World.destroytype.CellAndRoad);
                                }
                            }
                        }
                    }
                }
                ch.SendDirectedFx(1, x, y, 3, 0, 0);
                ch.ClearPack(x, y);
            });
            return true;
        }
        public static bool Prot(Player player)
        {
            var d = player.GetDirCord();
            int x = d.x, y = d.y;
            if (!World.AccessGun(x, y, player.cid).access) return false;
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 1);
            World.W.AsyncAction(2, () =>
            {
                for (int _x = -1; _x <= 1; _x++)
                {
                    for (int _y = -1; _y <= 1; _y++)
                    {
                        if (World.W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2(x + _x, y + _y)) <= 3.5f)
                        {
                            foreach (var p in World.W.GetPlayersFromPos(x + _x, y + _y))
                            {
                                p.Hurt(50);
                            }
                            if (World.ContainsPack(x + _x, y + _y, out var pack) && pack is Gate) (pack as Gate).Destroy();
                            var c = World.GetCell(x + _x, y + _y);
                            if (World.GetProp(c).is_destructible && !World.PackPart(x + _x, y + _y))
                            {
                                World.Destroy(x + _x, y + _y, World.destroytype.CellAndRoad);
                            }
                        }
                    }
                }
                ch.SendDirectedFx(1, x, y, 1, 0, 1);
                ch.ClearPack(x, y);
            });
            return true;
        }
        public static bool Geopack(int type,Player p)
        {
            var d = p.GetDirCord();
            int x = d.x, y = d.y;
            var cell = World.GetCell(x, y);
            if (World.TrueEmpty(x,y) && type != 10)
            {
                World.SetCell(x, y, type switch
                {
                    11 => CellType.AliveCyan,
                    12 => CellType.AliveRed,
                    13 => CellType.AliveViol,
                    14 => CellType.AliveNigger,
                    15 => CellType.AliveWhite,
                    16 => CellType.AliveBlue,
                    34 => CellType.HypnoRock,
                    42 => CellType.NiggerRock,
                    43 => CellType.RedRock,
                    46 => CellType.AliveRainbow
                });
                return true;
            }
            else if (World.isAlive(cell))
            {
                var id = (CellType)cell switch
                {
                    CellType.AliveCyan => 11,
                    CellType.AliveRed => 12,
                    CellType.AliveViol =>13,
                    CellType.AliveNigger =>14,
                    CellType.AliveWhite => 15,
                    CellType.AliveBlue => 16,
                    CellType.HypnoRock => 34,
                    CellType.NiggerRock => 42,
                    CellType.RedRock => 43,
                    CellType.AliveRainbow => 46
                };
                World.Destroy(x, y);
                p.inventory[id]++;
                return true;
            }
            return false;
        }
        public static bool Raz(Player p)
        {
            var d = p.GetDirCord();
            int x = d.x, y = d.y;
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 2);
            World.W.AsyncAction(5, () =>
            {
                using var db = new DataBase();
                for (int _x = -10; _x <= 10; _x++)
                {
                    for (int _y = -10; _y <= 10; _y++)
                    {
                        if (World.W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2(x + _x, y + _y)) <= 9.5f)
                        {
                            if (World.ContainsPack(x + _x, y + _y, out var pack) && pack is IDamagable)
                            {
                                var damagable = pack as IDamagable;
                                db.Attach(pack);

                                if (damagable.CanDestroy()) damagable.Destroy(p);
                                else damagable.Damage(10);
                                if (pack.charge == 0)
                                    World.W.GetChunk(pack.x, pack.y).ResendPack(pack);
                            }
                            foreach (var player in World.W.GetPlayersFromPos(x + _x, y + _y))
                                player.Hurt(500);
                        }
                    }
                }
                db.SaveChanges();
                ch.SendDirectedFx(1, x, y, 9, 0, 2);
                ch.ClearPack(x, y);
            });
            return true;
        }
    }
}
