using MinesServer.GameShit.Enums;
using RcherNZ.AccidentalNoise;

namespace MinesServer.GameShit.Generator
{
    public class SectorFiller
    {
        private static readonly Random rand = new Random();

        private static ImplicitFractal NotTypedNoise()
        {
            var type = (FractalType)rand.Next(0, 5);
            var basis = (BasisType)rand.Next(0, 4);
            var interpol = (InterpolationType)rand.Next(0, 4);
            return new ImplicitFractal(type, basis, interpol)
            {
                Octaves = rand.Next(4, 20),
                Frequency = rand.Next(4, 20),
                Lacunarity = rand.Next(1, 20)

            };
        }

        private Dictionary<CellType, (float, float)> RandomSizedParts(CellType[] availableCellTypes)
        {
            var RandomCellTypes = new Dictionary<CellType, (float start, float end)>();
            foreach (var type in availableCellTypes)
            {
                float start = (float)rand.NextDouble();
                float end = start + (float)rand.NextDouble();
                while (RandomCellTypes.Values.Any(segment => segment.start <= end && segment.end >= start))
                {
                    start = (float)rand.NextDouble();
                    end = start + (float)rand.NextDouble();
                    continue;
                }
                RandomCellTypes[type] = (start, end);
            }
            return RandomCellTypes;
        }

        private (float min, float max) FillNoiseToSector(Sector s)
        {
            var fr = NotTypedNoise();
            float max = (float)fr.Get(0, 0);
            float min = (float)fr.Get(0, 0);
            double localoffsetx = rand.NextDouble();
            double localoffsety = rand.NextDouble();
            foreach (var c in s.seccells)
            {
                var x = c.pos.x == 0 ? 100 : c.pos.x;
                var y = c.pos.y == 0 ? 100 : c.pos.y;
                var widthx = (s.width) == 0 ? 100 : (s.width);
                var heighty = (s.height) == 0 ? 100 : (s.height);
                var v = (float)fr.Get((float)((float)x / (float)widthx), (float)((float)y / (float)heighty));
                while (v == double.NaN || v == 0)
                {
                    localoffsetx += rand.NextDouble();
                    localoffsety += rand.NextDouble();
                    if (localoffsetx > x)
                        localoffsetx = 0;
                    if (localoffsety > y)
                        localoffsety = 0;
                    v = (float)fr.Get((float)(x + localoffsetx) / (float)widthx, (float)(y + localoffsety) / (float)heighty);
                }
                max = max < v ? v : max;
                min = min < v ? min : v;
                c.value = v;
            }
            return (min, max);
        }

        private Dictionary<CellType, int> SampleAndFindTypes(Sector s, Dictionary<CellType, (float start, float min)> parts)
        {
            (float minvalue, float maxvalue) = FillNoiseToSector(s);
            var typesresult = new Dictionary<CellType, int>();
            foreach (var c in s.seccells)
            {
                c.value = ((c.value - minvalue) / (maxvalue - minvalue));
                for (int i = 0; i < parts.Count; i++)
                {
                    c.type = c.value >= parts.ElementAt(i).Value.start && c.value <= parts.ElementAt(i).Value.min ? parts.ElementAt(i).Key : c.type;
                    if (!typesresult.TryGetValue(c.type, out int value))
                        typesresult[c.type] = 1;
                    else
                    {
                        typesresult[c.type] = ++value;
                    }
                }
            }
            return typesresult;
        }

        public void CreateFillForCells(Sector s)
        {
            var availableCellTypes = s.GenerateInsides();
            bool gig = s.seccells.Count <= 40000;
            var partsseccells = s.seccells.Count * 0.4;

            Console.WriteLine("");
            var segmentsmall = 0;
            var notenouthparts = 0;
            var empty = 0;

            restart:

            var parts = RandomSizedParts(availableCellTypes);
            while(parts.Count < availableCellTypes.Length)
            {
                parts = RandomSizedParts(availableCellTypes);
            }

        refillnoise:
            var result = SampleAndFindTypes(s, parts);
            Console.Write("\r                                                                                  ");
            if (result.Count < parts.Count)
            {
                notenouthparts++;
                if (notenouthparts > 2)
                {
                    notenouthparts = 0;
                    Console.Write("\rrestarted");
                    goto restart;
                }
                Console.Write("\rto small result");
                goto refillnoise;
            }
            if (result.TryGetValue(CellType.Empty, out int value) && partsseccells < value)
            {
                empty++;
                if (empty > 4)
                {
                    Console.Write("\rtoo empty");
                    empty = 0;
                    goto restart;
                }
                goto refillnoise;
            }
            foreach (var i in result)
            {
                var check = (partsseccells / parts.Count) > i.Value;
                if (check)
                {
                    segmentsmall++;
                    if (segmentsmall > 2)
                    {
                        segmentsmall = 0;
                        Console.Write("\rOneOfsegmentstosmall restart");
                        goto restart;
                    }
                    Console.Write($"\rOneOfsegmentstosmall resample {segmentsmall}");
                    goto refillnoise;
                }
            }
            if (gig)
            {
                var ft = availableCellTypes[rand.Next(0, availableCellTypes.Length - 1)];
                foreach (var c in s.seccells)
                {
                    if (c.type == CellType.Empty)
                    {
                        c.type = ft;
                    }
                    if (alive(s.seccells.Count) > rand.Next(1, 101))
                    {
                        //c.type = randalive
                    }
                }
            }
            Console.WriteLine("");
        }

        private static int alive(int x)
        {
            return 90 + x / 1000;
        }
    }

}
