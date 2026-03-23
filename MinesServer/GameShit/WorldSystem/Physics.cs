using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.WorldSystem
{
    public static class Physics
    {
        private static readonly (int x, int y)[] _directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];

        public static Random r = new Random();
        public static bool Boulder(int x, int y)
        {
            var v = World.TrueEmpty;
            if (World.GetCell(x, y + 1) == (byte)CellType.Gate && v(x, y + 2))
            {
                World.MoveCell(x, y, 0, 2);
            }
            else if (v(x, y + 1))
            {
                World.MoveCell(x, y, 0, 1);
                return true;
            }
            if (World.GetProp(World.GetCell(x, y + 1)).isBoulder || World.GetProp(World.GetCell(x, y + 1)).isSand)
            {
                if (r.Next(1, 101) > 50 && v(x + 1, y + 1) && v(x + 1, y))
                {
                    World.MoveCell(x, y, 1, 1);
                    return true;
                }
                else if (v(x - 1, y + 1) && v(x - 1, y))
                {
                    World.MoveCell(x, y, -1, 1);
                    return true;
                }
            }
            return false;
        }
        public static bool Sand(int x, int y)
        {
            var v = World.TrueEmpty;
            if (World.GetCell(x,y+1) == (byte)CellType.Gate && v(x, y + 2))
            {
                World.MoveCell(x, y, 0, 2);
            }
            else if (v(x, y + 1))
            {
                World.MoveCell(x, y, 0, 1);
                return true;
            }
            else if (World.GetProp(World.GetCell(x, y + 1)).isSand || World.GetProp(World.GetCell(x, y + 1)).isBoulder)
            {
                if (v(x + 1, y + 1) && v(x - 1, y + 1))
                {
                    if (r.Next(1, 101) > 50)
                        World.MoveCell(x, y, 1, 1);
                    else
                        World.MoveCell(x, y, -1, 1);
                }
                else if (v(x + 1, y + 1))
                {
                    World.MoveCell(x, y, 1, 1);
                    return true;
                }
                else if (v(x - 1, y + 1))
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
            var mod = 1;
            foreach (var dir in _directions)
            {
                if (World.GetCell(x + dir.x, y + dir.y) == 119)
                {
                    mod += 2;
                }
            }
            if (mod > 1)
            {
                mod -= 1;
            }
            return (CellType)cell switch
            {
                CellType.AliveViol => AliveViol(x, y, mod),
                CellType.AliveRainbow => AliveRainbow(x, y, mod),
                CellType.AliveBlue => AliveBlue(x, y, mod),
                CellType.AliveRed => AliveRed(x, y, mod),
                CellType.AliveCyan => AliveCyan(x, y, mod),
                CellType.AliveBlack => AliveBlack(x, y, mod),
                CellType.AliveWhite => AliveWhite(x, y, mod),
                _ => false
            };
        }

        private static bool AliveBlue(int x, int y, int mod)
        {
            foreach (var dir in _directions)
            {
                if (r.Next(1, 101) < 20 && World.W.GetPlayersFromPos(x + dir.x, y + dir.y).Count == 0 && World.IsEmpty(x + dir.x, y + dir.y))
                {
                    World.MoveCell(x, y, dir.x, dir.y);
                    World.SetCell(x, y, 109);
                    World.SetDurability(x, y, 20 * mod);
                    return true;
                }
            }
            return false;
        }
        private static bool AliveWhite(int x, int y, int mod)
        {
            if (World.GetProp(x, y - 1).isSand)
            {
                for (int wx = -1; wx <= 1; wx++)
                {
                    for (int wy = -1; wy <= 1; wy++)
                    {
                        if (World.IsEmpty(x + wx, y + wy) && World.W.GetPlayersFromPos(x + wx, y + wy).Count == 0)
                        {
                            World.SetCell(x + wx, y + wy, (byte)CellType.White);
                            World.SetDurability(x + wx, y + wy, 9 * mod);
                        }
                    }
                }
                if (r.Next(1, 101) < 20)
                {
                    World.Destroy(x, y - 1);
                }
                return true;
            }
            return true;
        }
        private static bool AliveBlack(int x, int y, int mod)
        {
            var c = 0;
            for (int ax = -1; ax <= 1; ax++)
            {
                for (int ay = -1; ay <= 1; ay++)
                {
                    var cell = World.GetCell(x + ax, y + ay);
                    if (cell == (byte)CellType.AliveBlack)
                    {
                        c++;
                    }
                }
            }
            if (c >= 6)
            {
                World.SetCell(x, y, 114);
                return true;
            }
            if (c > 0)
            {
                foreach (var i in _directions)
                {
                    var cell = World.GetCell(x + i.x, y + i.y);
                    if (cell == (byte)CellType.AliveBlack && World.IsEmpty(x + -i.x, y + -i.y) && World.W.GetPlayersFromPos(x + -i.x, y + -i.Item2).Count == 0)
                    {
                        if (r.Next(1, 101) > 50)
                        {
                            World.SetCell(x + -i.x, y + -i.y, (byte)CellType.Red);
                            World.SetDurability(x + i.x, y + i.y, 3 * mod);
                        }
                        else
                        {
                            World.SetCell(x + -i.x, y + -i.y, (byte)CellType.Cyan);
                            World.SetDurability(x + i.x, y + i.y, 2 * mod);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool AliveCyan(int x, int y, int mod)
        {
            var c = 0;
            foreach (var i in _directions)
            {
                if (World.IsEmpty(x + i.x, y + i.y) && World.W.GetPlayersFromPos(x + i.x, y + i.y).Count == 0)
                {
                    World.SetCell(x + i.x, y + i.y, (byte)CellType.Cyan);
                    World.SetDurability(x + i.x, y + i.y, 2 * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        private static bool AliveRainbow(int x, int y, int mod)
        {
            var c = 0;
            foreach (var dir in _directions)
            {
                if (World.IsEmpty(x + dir.x, y + dir.y) && 
                    World.W.GetPlayersFromPos(x + dir.x, y + dir.y).Count == 0 
                    && !World.isAlive(x + -dir.x, y + -dir.y) 
                    && !World.GetProp(x + -dir.x, y + -dir.y).isEmpty 
                    && World.IsForDigging(x + -dir.x, y + -dir.y))
                {
                    World.SetCell(x + dir.x, y + dir.y, World.GetCell(x + -dir.x, y + -dir.y));
                    World.SetDurability(x + dir.x, y + dir.y, World.GetProp(x + dir.x, y + dir.y).durability * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        private static bool AliveRed(int x, int y, int mod)
        {
            var c = 0;
            var chs = 0;
            for (int cx = -1; cx <= 1; cx++)
            {
                for (int cy = -1; cy <= 1; cy++)
                {
                    if (World.GetCell(x + cx, y + cy) == (byte)CellType.BlackRock)
                    {
                        chs++;
                    }
                }
            }
            if (chs < 1)
            {
                return false;
            }
            foreach (var i in _directions)
            {
                if (World.IsEmpty(x + i.x, y + i.y) && World.W.GetPlayersFromPos(x + i.x, y + i.y).Count == 0)
                {
                    World.SetCell(x + i.x, y + i.y, (byte)CellType.Red);
                    World.SetDurability(x + i.x, y + i.y, 3 * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
        private static bool AliveViol(int x, int y, int mod)
        {
            var c = 0;
            var chs = 0;
            for (int cx = -1; cx <= 1; cx++)
            {
                for (int cy = -1; cy <= 1; cy++)
                {
                    if (World.GetCell(x + cx, y + cy) == (byte)CellType.BlackRock)
                    {
                        chs++;
                    }
                }
            }
            if (chs < 1)
            {
                return false;
            }
            foreach (var i in _directions)
            {
                if (World.IsEmpty(x + i.x, y + i.y) && World.W.GetPlayersFromPos(x + i.x, y + i.y).Count == 0)
                {
                    World.SetCell(x + i.x, y + i.y, (byte)CellType.Violet);
                    World.SetDurability(x + i.x, y + i.y, 2 * mod);
                    c++;
                }
            }
            if (c > 0)
                return true;
            return false;
        }
    }
}
