﻿using System.Runtime.CompilerServices;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using RcherNZ.AccidentalNoise;

namespace MinesServer.GameShit.Generator
{
    public class Sectors
    {
        private readonly Random rand = new Random();
        private readonly (int x, int y)[] dirs = [(0, 1), (0, -1), (-1, 0), (1, 0)];
        private readonly int seed;
        private double min, mid, max;
        private (int x, int y) size;
        public SectorCell[] map;

        public Sectors((int, int) size) : this(Environment.TickCount, size)
        {

        }

        public Sectors(int seed, (int, int) size)
        {
            this.size = size;
            this.seed = seed;
            this.rand = new Random(seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int size_index(int x, int y)
            => x * size.y + y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool valid_size(int x, int y)
            => x < size.x && x >= 0 && y < size.y && y >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SectorCell sector_map(int x, int y)
            => map[size_index(x, y)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float chs(int y)
            => 30f - (y * 0.0028f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static CellType GetCellType(int value) => value switch
        {
            1 => CellType.RedRock,
            2 => CellType.BlackRock,
            _ => CellType.Empty
        };

        public void DetectAndFillSectors()
        {
            List<Sector> sectors = [];
            List<SectorCell> seccells = [];
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
                        seccells.Add(cell);
                        cell.sector = sectors.Count;

                        foreach (var i in dirs)
                        {
                            var nx = cell.pos.x + i.x;
                            var ny = cell.pos.y + i.y;

                            if (!valid_size(nx, ny))
                                continue;

                            var ncell = sector_map(nx, ny);
                            if (ncell.sector == -1 && ncell.value == 0)
                            {
                                ncell.sector = sectors.Count;
                                que.Enqueue(ncell);
                            }
                        }
                    }

                    var s = new Sector(swidth, sheight, depth);

                    if (seccells.Count < 50)
                    {
                        continue;
                    }

                    Console.WriteLine($"{secnum} sector filling");
                    secnum++;

                    SectorFiller.CreateFillForCells(s, seccells);

                    Console.WriteLine("saving sector " + seccells.Count);

                    foreach (var c in seccells)
                    {
                        World.SetCell(c.pos.x, c.pos.y, (byte)c.type);
                    }

                    World.CommitWorld();
                    seccells = [];
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
                        if ((3 < ch && rand.Next(1, 101) > 60) || (e > 1))
                        {
                            _sector.value = 2;
                            if (rand.Next(1, 101) > 95 && b)
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

        private static ImplicitFractal NotTypedNoise(double _Frequency, double _Lacunarity, int _Seed, InterpolationType Interpolation)
        {
            var type = FractalType.RidgedMulti;
            var basis = BasisType.GradientValue;
            var interpolation = Interpolation;
            return new ImplicitFractal(type, basis, interpolation)
            {
                Octaves = 1,
                Frequency = _Frequency,
                Lacunarity = _Lacunarity,
                Seed = _Seed,
            };
        }

        public void GenerateENoise(double _Frequency = 25, double _Lacunarity = 1, InterpolationType Interpolation = InterpolationType.Cubic, float res = .45f)
        {
            var fr = NotTypedNoise(_Frequency, _Lacunarity, seed, Interpolation);

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

        public void End()
        {
            Console.WriteLine("ending");
            CleanCs(0, true);
            for (int i = 1; i < 6; i++)
                CleanCs(i);

            // Единый проход по всем секторам
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var sector = sector_map(x, y);

                    if (sector.value == 0)
                    {
                        // TODO: наверное стоит пропускать
                        // Так как по умолчанию и так пустой
                        // А ещё лучше использовать get
                        // Так как только для этого и используется
                        // А еще проще синхронизировать
                        sector.type = CellType.Empty;
                        continue;
                    }

                    // Шанс превратить RedRock в BlackRock
                    if (sector.value == 1 && rand.Next(1, 101) < chs(y))
                    {
                        sector.value = 2;
                    }

                    if (sector.value == 2 && rand.Next(1, 101) > 90)
                    {
                        sector.value = 0;
                    }
                    else if (sector.value == 1 && rand.Next(1, 101) > 95)
                    {
                        sector.value = 0;
                    }

                    sector.type = GetCellType((int)sector.value);
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

        private void Boom(int x, int y)
        {
            var b = rand.Next(3, 7);
            for (int xx = -b; xx <= b; xx++)
            {
                for (int yy = -b; yy <= b; yy++)
                {
                    var nx = x + xx; var ny = y + yy;

                    if (!valid_size(nx, ny)) continue;

                    var _sector = sector_map(nx, ny);

                    if ((_sector.value == 0 && rand.Next(1, 101) > 60) || 
                        (_sector.value == 1 && rand.Next(1, 101) < chs(y)))
                    {
                        _sector.value = 2;
                    }
                }
            }
        }

        private void resample(float res = .45f)
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
    }
}
