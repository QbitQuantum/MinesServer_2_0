using MinesServer.GameShit.Enums;
﻿using System.Runtime.CompilerServices;
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

        private int size_index(int x, int y)
            => x * size.y + y;

        private bool valid_size(int x, int y)
            => x < size.x && x >= 0 && y < size.y && y >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SectorCell sector_map(int x, int y)
            => map[size_index(x, y)];

        private static float chs(int y)
            => 30f - (y * 0.0028f);

        public void DetectAndFillSectors()
        {
            List<Sector> sectors = [];
            List<SectorCell> ce = [];
            Queue<SectorCell> que = [];
            int secnum = 0;

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var _sector = sector_map(x, y);
                    
                    if (_sector.sector != -1 || _sector.value != 0)
                        continue;

                    var swidth = 0;
                    var sheight = 0;
                    var depth = _sector.pos.y;
                    var startX = _sector.pos.x;
                    var startY = _sector.pos.y;


                    que.Enqueue(_sector);
                    while (que.Count > 0)
                    {
                        var cell = que.Dequeue();
                        depth = Math.Min(depth, cell.pos.y);
                        swidth = Math.Max(swidth, cell.pos.x - startX);
                        sheight = Math.Max(sheight, cell.pos.y - startY);
                        ce.Add(cell);
                        cell.sector = sectors.Count;

                        foreach (var i in dirs)
                        {
                            var nx = cell.pos.x + i.x;
                            var ny = cell.pos.y + i.y;

                            if (!valid_size(nx, ny))
                                continue;

                            var ncell = map[size_index(nx, ny)];
                            if (ncell.sector == -1 && ncell.value == 0)
                            {
                                ncell.sector = sectors.Count;
                                que.Enqueue(ncell);
                            }
                        }
                    }

                    var s = new Sector()
                    {
                        seccells = ce,
                        width = swidth,
                        height = sheight,
                        depth = depth
                    };

                    if (s.seccells.Count < 50)
                    {
                        continue;
                    }

                    Console.WriteLine($"{secnum} sector filling");
                    secnum++;
                    var inside = new SectorFiller();
                    bool gig = s.seccells.Count <= 40000;

                    inside.CreateFillForCells(s, gig, s.GenerateInsides());

                    Console.WriteLine("saving sector " + s.seccells.Count);
                    foreach (var c in s.seccells)
                    {
                        World.SetCell(c.pos.x, c.pos.y, (byte)c.type);
                    }

                    World.CommitWorld();
                    ce = [];
                }
            }

        }
        
        private void CleanCs(int j, bool b = false)
        {
            Console.WriteLine("filling cs to chs");
            for (int y = (j % 2 == 0 ? 0 : size.y - 1); (j % 2 == 0 ? y < size.y : y >= 0);)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var _sector = sector_map(x, y);
                    if (_sector.value == 1)
                    {
                        var c = 0; var ch = 0; var e = 0;
                        for (int xx = -2; xx <= 2; xx++)
                        {
                            for (int yy = -2; yy <= 2; yy++)
                            {
                                var nx = x + xx; var ny = y + yy;
                                if (valid_size(nx, ny))
                                {
                                    switch (_sector.value)
                                    {
                                        case 0: e++; break;
                                        case 1: c++; break;
                                        case 2: ch++; break;
                                    }
                                }
                            }
                        }
                        if ((3 < ch && r.Next(1, 101) > 60) || (e > 1))
                        {
                            _sector.value = 2;
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
            map = new SectorCell[size.x * size.y];
            max = (float)fr.Get(0, 0);
            min = (float)fr.Get(0, 0);
            var counter = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var v = (float)fr.Get((float)(x / (float)size.x), (float)(y / (float)size.y));
                    max = max < v ? v : max;
                    min = min < v ? min : v;
                    map[size_index(x, y)] = new SectorCell() { value = v, pos = (x, y), sector = -1 };
                    counter++;
                }
                Console.Write($"\r{counter}/{map.Length} setting base map");
            }
            Console.WriteLine("");
            Console.WriteLine(max);
            Console.WriteLine(min);
            mid = 0f;
            counter = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var _sector = sector_map(x, y);
                    _sector.value = (float)((_sector.value - min) / (max - min));
                    mid += _sector.value;
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
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var _sector = sector_map(x, y);

                    if (_sector.value == 2 && r.Next(1, 101) > 90)
                    {
                        _sector.value = 0;
                    }
                    else if (_sector.value == 1 && r.Next(1, 101) > 95)
                    {
                        _sector.value = 0;
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
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var _sector = sector_map(x, y);

                    CellType _type_sector;

                    switch (_sector.value)
                    {
                        case 1: _type_sector = CellType.RedRock; break;
                        case 2: _type_sector = CellType.BlackRock; break;
                        default: _type_sector = CellType.Empty; break;
                    }
                    _sector.type = _type_sector;
                }
            }
            Console.WriteLine("end");
        }
        public void AddW(double freq = 25, double lac = 1, InterpolationType t = InterpolationType.Cubic, float res = .45f)
        {
            // TODO: Кажется хрень происходит
            var temp = map;
            GenerateENoise(freq, lac, t, res);
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int inxex = size_index(x, y);
                    temp[inxex].value = temp[inxex].value == 0 ? map[inxex].value : temp[inxex].value;
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
            Console.WriteLine("adding black rock");
            var counter = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    counter++;
                    var _sector = sector_map(x, y);
                    if (_sector.value == 1)
                    {
                        if (r.Next(1, 101) < chs(y))
                        {
                            _sector.value = 2;
                        }
                    }
                }
                Console.Write($"\r{counter}/{map.Length} black rock");
            }
            Console.WriteLine("");
        }

        private void Boom(int x, int y)
        {
            var b = r.Next(3, 7);
            for (int xx = -b; xx <= b; xx++)
            {
                for (int yy = -b; yy <= b; yy++)
                {
                    var nx = x + xx; var ny = y + yy;

                    if (!valid_size(nx, ny)) continue;

                    var _sector = sector_map(nx, ny);

                    if ((_sector.value == 0 && r.Next(1, 101) > 60) || 
                        (_sector.value == 1 && r.Next(1, 101) < chs(y)))
                    {
                        _sector.value = 2;
                    }
                }
            }
        }
        public void resample(float res = .45f)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var _sector = sector_map(x, y);
                    _sector.value = (_sector.value < mid + res) ? 0 : 1;
                }
            }
        }
        private readonly (int x, int y)[] dirs = [(0, 1), (0, -1), (-1, 0), (1, 0)];
        private readonly int seed;
        private double min, mid, max;
        public SectorCell[] map;
        private ImplicitFractal fr;
        private (int x, int y) size;
        public Random r = new Random();
        
    }
}
