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

            // Запланировать взрыв через 1 секунду
            World.ScheduleAction(TimeSpan.FromSeconds(1), () =>
            {
                // Всё выполняется в основном игровом потоке!
                for (int dx = -4; dx <= 4; dx++)
                {
                    for (int dy = -4; dy <= 4; dy++)
                    {
                        int tx = x + dx, ty = y + dy;
                        if (!World.W.ValidCoord(tx, ty)) continue;

                        if (Vector2.Distance(new Vector2(x, y), new Vector2(tx, ty)) <= 3.5f)
                        {
                            // Наносим урон игрокам
                            foreach (var p in World.W.GetPlayersFromPos(tx, ty))
                            {
                                p.Hurt(40);
                            }

                            // Разрушаем блоки
                            var c = World.GetCell(tx, ty);
                            if (World.GetProp(c).is_destructible && !World.PackPart(tx, ty))
                            {
                                if (c == 117 && Physics.r.Next(1, 101) > 98)
                                {
                                    World.SetCell(tx, ty, 118);
                                }
                                else if (c == 118)
                                {
                                    World.SetCell(tx, ty, 103);
                                }
                                else if (c != 117 && c != 118)
                                {
                                    World.Destroy(tx, ty, World.destroytype.CellAndRoad);
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

            World.ScheduleAction(TimeSpan.FromSeconds(2), () =>
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int tx = x + dx, ty = y + dy;
                        if (!World.W.ValidCoord(tx, ty)) continue;

                        if (Vector2.Distance(new Vector2(x, y), new Vector2(tx, ty)) <= 3.5f)
                        {
                            foreach (var p in World.W.GetPlayersFromPos(tx, ty))
                            {
                                p.Hurt(50);
                                // Уничтожаем гейты
                                if (World.ContainsPack(tx, ty, out var pack) && pack is Gate gate)
                                {
                                    gate.Destroy(p);
                                }
                            }
                            // Разрушаем блоки
                            var c = World.GetCell(tx, ty);
                            if (World.GetProp(c).is_destructible && !World.PackPart(tx, ty))
                            {
                                World.Destroy(tx, ty, World.destroytype.CellAndRoad);
                            }
                        }
                    }
                }
                ch.SendDirectedFx(1, x, y, 1, 0, 1);
                ch.ClearPack(x, y);
            });
            return true;
        }
        public static bool Raz(Player player)
        {
            var d = player.GetDirCord();
            int x = d.x, y = d.y;
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 2);

            World.ScheduleAction(TimeSpan.FromSeconds(5), () =>
            {
                // Создаём новый контекст БД — только внутри действия!
                using var db = new DataBase();

                for (int dx = -10; dx <= 10; dx++)
                {
                    for (int dy = -10; dy <= 10; dy++)
                    {
                        int tx = x + dx, ty = y + dy;
                        if (!World.W.ValidCoord(tx, ty)) continue;

                        if (Vector2.Distance(new Vector2(x, y), new Vector2(tx, ty)) <= 9.5f)
                        {
                            // Урон игрокам
                            foreach (var p in World.W.GetPlayersFromPos(tx, ty))
                            {
                                p.Hurt(500);
                            }

                            // Работа с повреждаемыми объектами
                            if (World.ContainsPack(tx, ty, out var pack) && pack is IDamagable damagable)
                            {
                                db.Attach(pack); // привязываем к контексту

                                if (damagable.CanDestroy())
                                    damagable.Destroy(player);
                                else
                                    damagable.Damage(10);

                                if (pack.charge == 0)
                                    World.W.GetChunk(pack.x, pack.y).ResendPack(pack);
                            }
                        }
                    }
                }

                db.SaveChanges(); // сохраняем изменения

                ch.SendDirectedFx(1, x, y, 9, 0, 2);
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
    }
}
