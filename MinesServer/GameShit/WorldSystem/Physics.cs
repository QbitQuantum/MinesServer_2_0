using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.WorldSystem
{
    public static class Physics
    {
        private static readonly (int x, int y)[] _directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];
        public static readonly Random r = new();

        public static bool Boulder(int x, int y)
        {
            var isEmpty = World.TrueEmpty;

            // Проверка падения через ворота
            if (World.GetCell(x, y + 1) == (byte)CellType.Gate && isEmpty(x, y + 2))
            {
                World.MoveCell(x, y, 0, 2);
                return true;
            }

            // Падение вниз
            if (isEmpty(x, y + 1))
            {
                World.MoveCell(x, y, 0, 1);
                return true;
            }

            // Скатывание по бокам
            var belowCell = World.GetCell(x, y + 1);
            if (World.GetProp(belowCell).isBoulder || World.GetProp(belowCell).isSand)
            {
                if (r.Next(1, 101) > 50 && isEmpty(x + 1, y + 1) && isEmpty(x + 1, y))
                {
                    World.MoveCell(x, y, 1, 1);
                    return true;
                }

                if (isEmpty(x - 1, y + 1) && isEmpty(x - 1, y))
                {
                    World.MoveCell(x, y, -1, 1);
                    return true;
                }
            }
            return false;
        }
        public static bool Sand(int x, int y)
        {
            var isEmpty = World.TrueEmpty;

            // Проверка падения через ворота
            if (World.GetCell(x, y + 1) == (byte)CellType.Gate && isEmpty(x, y + 2))
            {
                World.MoveCell(x, y, 0, 2);
                return true;
            }

            // Падение вниз
            if (isEmpty(x, y + 1))
            {
                World.MoveCell(x, y, 0, 1);
                return true;
            }

            // Рассыпание по бокам
            var belowCell = World.GetCell(x, y + 1);
            if (World.GetProp(belowCell).isSand || World.GetProp(belowCell).isBoulder)
            {
                if (isEmpty(x + 1, y + 1) && isEmpty(x - 1, y + 1))
                {
                    World.MoveCell(x, y, r.Next(2) == 0 ? 1 : -1, 1);
                    return true;
                }

                if (isEmpty(x + 1, y + 1))
                {
                    World.MoveCell(x, y, 1, 1);
                    return true;
                }

                if (isEmpty(x - 1, y + 1))
                {
                    World.MoveCell(x, y, -1, 1);
                    return true;
                }
            }
            return false;
        }
        public static bool Alive(int x, int y)
        {
            var cell = World.GetCell(x, y);
            var modifier = 1;

            // Подсчет соседних живых клеток (тип 119)
            foreach (var dir in _directions)
            {
                if (World.GetCell(x + dir.x, y + dir.y) == 119)
                {
                    modifier += 2;
                }
            }

            if (modifier > 1)
            {
                modifier--;
            }
            return (CellType)cell switch
            {
                CellType.AliveViol => AliveViol(x, y, modifier),
                CellType.AliveRainbow => AliveRainbow(x, y, modifier),
                CellType.AliveBlue => AliveBlue(x, y, modifier),
                CellType.AliveRed => AliveRed(x, y, modifier),
                CellType.AliveCyan => AliveCyan(x, y, modifier),
                CellType.AliveBlack => AliveBlack(x, y, modifier),
                CellType.AliveWhite => AliveWhite(x, y, modifier),
                _ => false
            };
        }

        private static bool AliveBlue(int x, int y, int modifier)
        {
            foreach (var dir in _directions)
            {
                if (r.Next(100) < 20 && World.IsEmptyForPlace(x + dir.x, y + dir.y))
                {
                    World.MoveCell(x, y, dir.x, dir.y);
                    World.SetCell(x, y, 109);
                    World.DamageCell(x, y, 20 * modifier, Operator.Unknown);
                    return true;
                }
            }
            return false;
        }

        private static bool AliveWhite(int x, int y, int modifier)
        {
            if (World.GetProp(x, y - 1).isSand)
            {
                for (int wx = -1; wx <= 1; wx++)
                {
                    for (int wy = -1; wy <= 1; wy++)
                    {
                        if (World.IsEmptyForPlace(x + wx, y + wy))
                        {
                            World.SetCell(x + wx, y + wy, (byte)CellType.White);
                            World.DamageCell(x + wx, y + wy, 9 * modifier, Operator.Unknown);
                        }
                    }
                }

                if (r.Next(100) < 20)
                {
                    World.Destroy(x, y - 1);
                }
            }
            return true;
        }

        private static bool AliveBlack(int x, int y, int modifier)
        {
            int count = 0;

            for (int ax = -1; ax <= 1; ax++)
            {
                for (int ay = -1; ay <= 1; ay++)
                {
                    if (World.GetCell(x + ax, y + ay) == (byte)CellType.AliveBlack)
                    {
                        count++;
                    }
                }
            }

            if (count >= 6)
            {
                World.SetCell(x, y, 114);
                return true;
            }

            if (count > 0)
            {
                foreach (var dir in _directions)
                {
                    if (World.GetCell(x + dir.x, y + dir.y) == (byte)CellType.AliveBlack &&
                        World.IsEmptyForPlace(x - dir.x, y - dir.y))
                    {
                        World.SetCell(x - dir.x, y - dir.y,
                            r.Next(2) == 0 ? (byte)CellType.Red : (byte)CellType.Cyan);
                        World.DamageCell(x + dir.x, y + dir.y,
                            r.Next(2) == 0 ? 3 * modifier : 2 * modifier, Operator.Unknown);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool AliveCyan(int x, int y, int modifier)
        {
            bool moved = false;

            foreach (var dir in _directions)
            {
                if (World.IsEmptyForPlace(x + dir.x, y + dir.y))
                {
                    World.SetCell(x + dir.x, y + dir.y, (byte)CellType.Cyan);
                    World.DamageCell(x + dir.x, y + dir.y, 2 * modifier, Operator.Unknown);
                    moved = true;
                }
            }

            return moved;
        }

        private static bool AliveRainbow(int x, int y, int modifier)
        {
            bool moved = false;

            foreach (var dir in _directions)
            {
                if (World.IsEmptyForPlace(x + dir.x, y + dir.y) &&
                    !World.isAlive(x - dir.x, y - dir.y) &&
                    !World.GetProp(x - dir.x, y - dir.y).isEmpty &&
                    World.IsForDigging(x - dir.x, y - dir.y))
                {
                    World.SetCell(x + dir.x, y + dir.y, World.GetCell(x - dir.x, y - dir.y));
                    World.DamageCell(x + dir.x, y + dir.y,
                        World.GetProp(x + dir.x, y + dir.y).durability * modifier, Operator.Unknown);
                    moved = true;
                }
            }

            return moved;
        }

        private static bool AliveRed(int x, int y, int modifier)
        {
            // Проверка наличия черной скалы рядом
            bool hasBlackRock = false;
            for (int cx = -1; cx <= 1 && !hasBlackRock; cx++)
            {
                for (int cy = -1; cy <= 1 && !hasBlackRock; cy++)
                {
                    if (World.GetCell(x + cx, y + cy) == (byte)CellType.BlackRock)
                    {
                        hasBlackRock = true;
                    }
                }
            }

            if (!hasBlackRock) return false;

            bool moved = false;
            foreach (var dir in _directions)
            {
                if (World.IsEmptyForPlace(x + dir.x, y + dir.y))
                {
                    World.SetCell(x + dir.x, y + dir.y, (byte)CellType.Red);
                    World.DamageCell(x + dir.x, y + dir.y, 3 * modifier, Operator.Unknown);
                    moved = true;
                }
            }

            return moved;
        }

        private static bool AliveViol(int x, int y, int modifier)
        {
            // Проверка наличия черной скалы рядом
            bool hasBlackRock = false;
            for (int cx = -1; cx <= 1 && !hasBlackRock; cx++)
            {
                for (int cy = -1; cy <= 1 && !hasBlackRock; cy++)
                {
                    if (World.GetCell(x + cx, y + cy) == (byte)CellType.BlackRock)
                    {
                        hasBlackRock = true;
                    }
                }
            }

            if (!hasBlackRock) return false;

            bool moved = false;
            foreach (var dir in _directions)
            {
                if (World.IsEmptyForPlace(x + dir.x, y + dir.y))
                {
                    World.SetCell(x + dir.x, y + dir.y, (byte)CellType.Violet);
                    World.DamageCell(x + dir.x, y + dir.y, 2 * modifier, Operator.Unknown);
                    moved = true;
                }
            }

            return moved;
        }
    }
}