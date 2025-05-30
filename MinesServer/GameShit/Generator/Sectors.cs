﻿using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using RcherNZ.AccidentalNoise;

namespace MinesServer.GameShit.Generator
{
    public class Sectors
    {
        public Sectors(int seed, (int, int) size)
        {
            this.size = size;
            this.seed = seed;
            r = new Random(seed);
        }
        public Sectors((int, int) size)
        {
            this.size = size;
            seed = Environment.TickCount;
            r = new Random(seed);
        }
        public void DetectAndFillSectors()
        {
            List<Sector> sectors = new List<Sector>();
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            var ce = new List<SectorCell>();
            var que = new Queue<SectorCell>();
            int secnum = 0;
            for (int y = 0; y < size.Item2; y++)
            {
                for (int x = 0; x < size.Item1; x++)
                {
                    if (map[x * size.Item2 + y].sector == -1 && map[x * size.Item2 + y].value == 0) 
                    {
                        var swidth = 0;
                        var sheight = 0;
                        var depth = map[x * size.Item2 + y].pos.Item2;
                        que.Enqueue(map[x * size.Item2 + y]);
                        while (que.Count > 0)
                        {
                            var cell = que.Dequeue();
                            depth = depth > cell.pos.Item2 ? cell.pos.Item2 : depth;
                            swidth = swidth > (cell.pos.Item1 - map[x * size.Item2 + y].pos.Item1) ? swidth : (map[x * size.Item2 + y].pos.Item1 - map[x * size.Item2 + y].pos.Item2);
                            sheight = sheight > (cell.pos.Item2 - map[x * size.Item2 + y].pos.Item2) ? sheight : (cell.pos.Item1 - map[x * size.Item2 + y].pos.Item2);
                            ce.Add(cell);
                            cell.sector = sectors.Count;
                            foreach (var i in dirs)
                            {
                                var nx = cell.pos.Item1 + i.Item1; var ny = cell.pos.Item2 + i.Item2;
                                if (v(nx, ny))
                                {
                                    var ncell = map[nx * size.Item2 + ny];
                                    if (ncell.sector == -1 && ncell.value == 0) 
                                    {
                                        ncell.sector = sectors.Count; 
                                        que.Enqueue(ncell);
                                    }
                                }
                            }
                        }
                        var s = new Sector() { seccells = ce, width = swidth, height = sheight, depth = depth };
                        
                        if (s.seccells.Count < 50)
                        {
                            continue;
                        }
                        Console.WriteLine($"{secnum} sector filling");
                        secnum++;
                        var inside = new SectorFiller();
                        if (s.seccells.Count > 40000)
                        {
                            inside.CreateFillForCells(s, false, s.GenerateInsides());
                        }
                        else if (s.seccells.Count <= 40000)
                        {
                            inside.CreateFillForCells(s, true, s.GenerateInsides());
                        }
                        Console.WriteLine("saving sector " + s.seccells.Count);
                        foreach (var c in s.seccells)
                        {
                            var ty = c.type == CellType.Empty ? (byte)0 : (byte)c.type;
                            if (ty != 0)
                            {
                                World.SetCell(c.pos.Item1, c.pos.Item2, ty);
                            }
                            else
                            {
                                World.SetCell(c.pos.Item1, c.pos.Item2, 32);
                            }
                        }
                        World.CommitWorld();
                        ce = new List<SectorCell>();
                    }
                }
            }

        }
        private float chs(int y)
        {
            return 30f - ((float)y * 0.0028f);
        }
        private void CleanCs(int j, bool b = false)
        {
            Console.WriteLine("filling cs to chs");
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            for (int y = (j % 2 == 0 ? 0 : size.Item2 - 1); (j % 2 == 0 ? y < size.Item2 : y >= 0);)
            {
                for (int x = 0; x < size.Item1; x++)
                {
                    if (map[x * size.Item2 + y].value == 1)
                    {
                        var c = 0; var ch = 0; var e = 0;
                        for (int xx = -2; xx <= 2; xx++)
                        {
                            for (int yy = -2; yy <= 2; yy++)
                            {
                                var nx = x + xx; var ny = y + yy;
                                if (v(nx, ny))
                                {
                                    if (map[nx * size.Item2 + ny].value == 1)
                                    {
                                        c++;
                                    }
                                    else if (map[nx * size.Item2 + ny].value == 2)
                                    {
                                        ch++;
                                    }
                                    else if (map[nx * size.Item2 + ny].value == 0)
                                    {
                                        e++;
                                    }
                                }
                            }
                        }
                        if ((3 < ch && r.Next(1, 101) > 60) || (e > 1))
                        {
                            map[x * size.Item2 + y].value = 2;
                            if (r.Next(1, 101) > 95 && b)
                            {
                                Boom(x, y);
                            }
                        }
                    }
                }
                if (j % 2 == 0)
                {
                    y++;
                    continue;
                }
                y--;
            }
        }
        public void GenerateENoise(double freq = 25, double lac = 1, InterpolationType t = InterpolationType.Cubic, float res = .45f)
        {
            fr = new ImplicitFractal(FractalType.RidgedMulti, BasisType.GradientValue, t)
            {
                Octaves = 1,
                Frequency = freq,
                Lacunarity = lac,
                Seed = seed
            };
            Console.WriteLine(fr.Type);
            map = new SectorCell[size.Item1 * size.Item2];
            max = (float)fr.Get(0, 0);
            min = (float)fr.Get(0, 0);
            var counter = 0;
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    var v = (float)fr.Get((float)(x / (float)size.Item1), (float)(y / (float)size.Item2));
                    max = max < v ? v : max;
                    min = min < v ? min : v;
                    map[x * size.Item2 + y] = new SectorCell() { value = v, pos = (x, y), sector = -1 };
                    counter++;
                }
                Console.Write($"\r{counter}/{map.Length} setting base map");
            }
            Console.WriteLine("");
            Console.WriteLine(max);
            Console.WriteLine(min);
            mid = 0f;
            counter = 0;
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    map[x * size.Item2 + y].value = (float)((map[x * size.Item2 + y].value - min) / (max - min));
                    mid += map[x * size.Item2 + y].value;
                    counter++;
                }
                Console.Write($"\r{counter}/{map.Length} sampling map");
            }
            Console.WriteLine("");
            mid /= map.Length;
            Console.WriteLine(mid);
            resample(res);
        }
        private void Clean()
        {
            Console.WriteLine("adding empty space");
            var c = 0;
            for (int y = 0; y < size.Item2; y++)
            {
                for (int x = 0; x < size.Item1; x++)
                {
                    if (map[x * size.Item2 + y].value == 2 && r.Next(1, 101) > 90)
                    {
                        map[x * size.Item2 + y].value = 0;
                    }
                    else if (map[x * size.Item2 + y].value == 1 && r.Next(1, 101) > 95)
                    {
                        map[x * size.Item2 + y].value = 0;
                    }
                    c++;
                }
                Console.Write($"\r{c}/{map.Length} empty space");
            }
            Console.Write($"");
        }
        public void End()
        {
            Console.WriteLine("ending");
            Add();
            Clean();
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    map[x * size.Item2 + y].type = map[x * size.Item2 + y].value == 2 ? CellType.NiggerRock : (map[x * size.Item2 + y].value == 1 ? CellType.RedRock : CellType.Empty);
                }
            }
            Console.WriteLine("end");
        }
        public void AddW(double freq = 25, double lac = 1, InterpolationType t = InterpolationType.Cubic, float res = .45f)
        {
            var temp = map;
            GenerateENoise(freq, lac, t, res);
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    temp[x * size.Item2 + y].value = temp[x * size.Item2 + y].value == 0 ? map[x * size.Item2 + y].value : temp[x * size.Item2 + y].value;
                }
            }
            map = temp;
        }
        private void Add()
        {
            CleanCs(0, true);
            for (int i = 1; i < 6; i++)
            {
                CleanCs(i);
            }
            Console.WriteLine("adding NIGGERrock");
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            var counter = 0;
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    counter++;
                    if (map[x * size.Item2 + y].value == 1)
                    {
                        if (r.Next(1, 101) < chs(y))
                        {
                            map[x * size.Item2 + y].value = 2;
                        }
                    }
                }
                Console.Write($"\r{counter}/{map.Length} nigger rock");
            }
            Console.WriteLine("");
        }
        private void Boom(int x, int y)
        {
            var b = r.Next(3, 7);
            var v = bool (int x, int y) => x < size.Item1 && x >= 0 && y < size.Item2 && y >= 0;
            for (int xx = -b; xx <= b; xx++)
            {
                for (int yy = -b; yy <= b; yy++)
                {
                    var nx = x + xx; var ny = y + yy;
                    if (v(nx, ny) && ((map[nx * size.Item2 + ny].value == 0 && r.Next(1, 101) > 60) || (map[nx * size.Item2 + ny].value == 1 && r.Next(1, 101) < chs(y))))
                    {
                        map[nx * size.Item2 + ny].value = 2;
                    }
                }
            }
        }
        public void resample(float res = .45f)
        {
            for (int x = 0; x < size.Item1; x++)
            {
                for (int y = 0; y < size.Item2; y++)
                {
                    if (map[x * size.Item2 + y].value < mid + res)
                    {
                        map[x * size.Item2 + y].value = 0;
                    }
                    else if (map[x * size.Item2 + y].value >= mid + res)
                    {
                        map[x * size.Item2 + y].value = 1;
                    }
                }
            }
        }
        private (int, int)[] dirs = { (0, 1), (0, -1), (-1, 0), (1, 0) };
        public double min, mid, max;
        public SectorCell[] map { get; private set; }
        private ImplicitFractal fr;
        public (int, int) size { get; private set; }
        public Random r = new Random();
        public int seed { private set; get; }
    }
}
