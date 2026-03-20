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
            int startX = d.x, startY = d.y;

            // Определяем координаты выстрела в зависимости от направления
            var target = GetShotTarget(startX, startY, p.dir, 9);

            // Проверка валидности координат
            if (!World.ValidCoord(target.x, target.y)) return false;

            // Отправка уведомления ботам
            p.SendDFToBots(7, target.x, target.y, p.id, 1);

            // Обработка линии выстрела
            ProcessShotLine(startX, startY, target.x, target.y);

            return true;
        }

        private static void ProcessShotLine(int startX, int startY, int targetX, int targetY)
        {
            // Определяем направление движения на основе разницы координат
            int stepX = Math.Sign(targetX - startX);
            int stepY = Math.Sign(targetY - startY);

            int currentX = startX;
            int currentY = startY;

            // Проходим по всем клеткам от старта до цели включительно
            while (currentX != targetX + stepX || currentY != targetY + stepY)
            {
                ProcessCell(currentX, currentY);

                currentX += stepX;
                currentY += stepY;
            }
        }

        private static void ProcessCell(int x, int y)
        {
            // Обработка игроков в текущей клетке
            foreach (var player in World.W.GetPlayersFromPos(x, y))
            {
                player.Hurt(20 + 60 * player.c190stacks);
                player.c190stacks++;
                player.lastc190hit = DateTime.Now;
            }

            // Повреждение клетки, если возможно
            if (World.CanDamageCell(x, y))
            {
                World.DamageCell(x, y, 50);
            }
        }

        private static (int x, int y) GetShotTarget(int startX, int startY, int direction, int radius)
        {
            return direction switch
            {
                0 => (startX, startY + radius),
                1 => (startX - radius, startY),
                2 => (startX, startY - radius),
                3 => (startX + radius, startY),
                _ => throw new ArgumentException($"Invalid direction: {direction}")
            };
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
            var (x, y) = player.GetDirCord();

            if (!World.AccessGun(x, y, player.cid).access) return false;
            
            var boom = new Bomb(player.id, x, y, BombType.PlasmaBomb);
            boom.Build();

            // Запланировать взрыв через 1 секунду
            World.ScheduleAction(TimeSpan.FromSeconds(1), () =>
            {
                for (int dx = -4; dx <= 4; dx++)
                {
                    for (int dy = -4; dy <= 4; dy++)
                    {
                        int tx = x + dx, ty = y + dy;
                        if (!World.ValidCoord(tx, ty)) continue;

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
                                    World.Destroy(tx, ty, DestroyCellType.CellAndRoad);
                                }
                            }
                        }
                    }
                }
                boom.Destroy(player);
                World.W.SendDirectedFx(1, x, y, 3, 0, 0);
            });
            return true;
        }
        public static bool Prot(Player player)
        {
            var (x, y) = player.GetDirCord();

            if (!World.AccessGun(x, y, player.cid).access) return false;
            var boom = new Bomb(player.id, x, y, BombType.ProtonBomb);
            boom.Build();

            World.ScheduleAction(TimeSpan.FromSeconds(2), () =>
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int tx = x + dx, ty = y + dy;
                        if (!World.ValidCoord(tx, ty)) continue;

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
                                World.Destroy(tx, ty, DestroyCellType.CellAndRoad);
                            }
                        }
                    }
                }
                boom.Destroy(player);
                World.W.SendDirectedFx(1, x, y, 1, 0, 1);
            });
            return true;
        }
        public static bool Raz(Player player)
        {
            var (x, y) = player.GetDirCord();

            var boom = new Bomb(player.id, x, y, BombType.DischargeBomb);
            boom.Build();

            World.ScheduleAction(TimeSpan.FromSeconds(5), () =>
            {
                // Создаём новый контекст БД — только внутри действия!
                using var db = new DataBase();

                for (int dx = -10; dx <= 10; dx++)
                {
                    for (int dy = -10; dy <= 10; dy++)
                    {
                        int tx = x + dx, ty = y + dy;
                        if (!World.ValidCoord(tx, ty)) continue;

                        if (Vector2.Distance(new Vector2(x, y), new Vector2(tx, ty)) <= 9.5f)
                        {
                            // Урон игрокам
                            foreach (var p in World.W.GetPlayersFromPos(tx, ty))
                            {
                                p.Hurt(500);
                            }

                            // Работа с повреждаемыми объектами
                            if (World.ContainsPack(tx, ty, out var pack) && pack is PackCharge damagable)
                            {
                                db.Attach(pack); // привязываем к контексту

                                if (damagable.CanDestroy())
                                    damagable.Destroy(player);
                                else
                                    damagable.Damage(10, DamageTypePacks.Raz);
                            }
                        }
                    }
                }
                db.SaveChanges(); // сохраняем изменения
                boom.Destroy(player);
                World.W.SendDirectedFx(1, x, y, 9, 0, 2);
            });

            return true;
        }
        public static bool Geopack(int type, Player player)
        {
            var (x, y) = player.GetDirCord();

            if (type == 10 /* Передано событие использование геопака */) {
                // Проверяем на пустоты клетки перед игроком
                if (World.TrueEmpty(x, y))
                    return false;

                var id = (CellType) World.GetCell(x, y) switch
                {
                    CellType.AliveCyan => 11,
                    CellType.AliveRed => 12,
                    CellType.AliveViol =>13,
                    CellType.AliveBlack =>14,
                    CellType.AliveWhite => 15,
                    CellType.AliveBlue => 16,
                    CellType.HypnoRock => 34,
                    CellType.BlackRock => 42,
                    CellType.RedRock => 43,
                    CellType.AliveRainbow => 46,
                    _ => -1
                };
                if (id == -1)
                    return false;

                World.Destroy(x, y);

                player.inventory[id]++;
                return true;
            }
            else // Передаем живки
            {
                if (World.TrueEmpty(x, y) )
                {
                    World.SetCell(x, y, type switch
                    {
                        11 => CellType.AliveCyan,
                        12 => CellType.AliveRed,
                        13 => CellType.AliveViol,
                        14 => CellType.AliveBlack,
                        15 => CellType.AliveWhite,
                        16 => CellType.AliveBlue,
                        34 => CellType.HypnoRock,
                        42 => CellType.BlackRock,
                        43 => CellType.RedRock,
                        46 => CellType.AliveRainbow
                    });
                    return true;
                }
            }
            return false;
        }
    }
}
