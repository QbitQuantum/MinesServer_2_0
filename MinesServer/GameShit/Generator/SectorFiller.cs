using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Generator
{
    public sealed class DepthSet
    {
        public CellType[] CrystalsGig { get; }
        public CellType[] CrystalsNormal { get; }
        public CellType[] Minerals { get; }

        public DepthSet(CellType[] crystalsGig, CellType[] crystalsNormal, CellType[] minerals)
        {
            CrystalsGig = crystalsGig;
            CrystalsNormal = crystalsNormal;
            Minerals = minerals;
        }
    }

    public sealed class SectorFiller
    {
        private readonly int _seed;
        private readonly int _worldWidth;
        private readonly int _worldHeight;
        private readonly int _gigChancePercent;
        private readonly float _fillFrequency;
        private readonly int _fillOctaves;
        private readonly float _fillLacunarity;
        private readonly float _fillGain;
        private readonly float _fillSharpness;
        private readonly DepthSet[] _sets;

        public SectorFiller(int worldWidth, int worldHeight, int seed, in GenerationParameters parameters)
        {
            _worldWidth = worldWidth;
            _worldHeight = worldHeight;
            _seed = seed;
            _gigChancePercent = parameters.GigChancePercent;
            _fillFrequency = parameters.FillFrequency;
            _fillOctaves = Math.Max(1, parameters.FillOctaves);
            _fillLacunarity = parameters.FillLacunarity;
            _fillGain = parameters.FillGain;
            _fillSharpness = Math.Clamp(parameters.FillSharpness, 0f, 1f);
            _sets = BuildSets();
        }

        public void Fill(CellType[] cells, IReadOnlyList<GeneratedSector> sectors)
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                FillSector(cells, sectors[i]);
            }
        }

        private void FillSector(CellType[] cells, GeneratedSector sector)
        {
            var rand = new Random(_seed ^ (sector.Id * 7919));

            // ── Интервал глубины сектора ─────────────────────────────────────
            float depthTop = _worldHeight > 0
                ? (float)sector.MinY / _worldHeight
                : 0f;
            float depthBottom = _worldHeight > 0
                ? (float)sector.MaxY / _worldHeight
                : 0f;

            int setTop = (int)(depthTop * _sets.Length);
            int setBottom = (int)(depthBottom * _sets.Length);
            if (setTop < 0) setTop = 0;
            if (setTop >= _sets.Length) setTop = _sets.Length - 1;
            if (setBottom < 0) setBottom = 0;
            if (setBottom >= _sets.Length) setBottom = _sets.Length - 1;

            // Если сектор целиком в одном сете — заливка как раньше.
            if (setTop == setBottom)
            {
                FillWithSingleSet(cells, sector, _sets[setTop], rand);
                return;
            }

            // Иначе — смешивание двух сетов по интервалу.
            FillWithTwoSets(cells, sector, setTop, setBottom, rand);
        }

        /// <summary>
        /// Сектор в одном сете — заливка одним датасетом.
        /// </summary>
        private void FillWithSingleSet(
            CellType[] cells,
            GeneratedSector sector,
            DepthSet set,
            Random rand)
        {
            bool gig = rand.Next(1, 101) <= _gigChancePercent;

            CellType[] palette = BuildPalette(set, gig);
            if (palette.Length == 0) palette = new[] { CellType.Rock };

            int fillSeed = _seed + (sector.Id * 31337);
            int paletteLen = palette.Length;

            for (int i = 0; i < sector.CellIndices.Length; i++)
            {
                int index = sector.CellIndices[i];
                int x = index % _worldWidth;
                int y = index / _worldWidth;

                float t = Fbm(x, y, fillSeed);
                if (_fillSharpness > 0f)
                {
                    t = t + ((t - 0.5f) * _fillSharpness);
                    if (t < 0f) t = 0f;
                    if (t > 1f) t = 1f;
                }

                int paletteIndex = (int)(t * paletteLen);
                if (paletteIndex < 0) paletteIndex = 0;
                if (paletteIndex >= paletteLen) paletteIndex = paletteLen - 1;

                CellType cell = palette[paletteIndex];
                cells[index] = cell;
            }
        }

        /// <summary>
        /// Сектор пересекает два сета — заливка по интервалу глубины.
        /// Верхняя часть — верхний сет, нижняя — нижний сет,
        /// посередине — смешивание двух сетов («капли»).
        /// </summary>
        private void FillWithTwoSets(
            CellType[] cells,
            GeneratedSector sector,
            int setTop,
            int setBottom,
            Random rand)
        {
            DepthSet topSet = _sets[setTop];
            DepthSet bottomSet = _sets[setBottom];

            bool gig = rand.Next(1, 101) <= _gigChancePercent;

            CellType[] topPalette = BuildPalette(topSet, gig);
            if (topPalette.Length == 0) topPalette = new[] { CellType.Rock };

            CellType[] bottomPalette = BuildPalette(bottomSet, gig);
            if (bottomPalette.Length == 0) bottomPalette = new[] { CellType.Rock };

            int fillSeed = _seed + (sector.Id * 31337);

            int sectorHeight = Math.Max(1, sector.MaxY - sector.MinY);

            for (int i = 0; i < sector.CellIndices.Length; i++)
            {
                int index = sector.CellIndices[i];
                int x = index % _worldWidth;
                int y = index / _worldWidth;

                // Нормализованная позиция в секторе: 0 = верх, 1 = низ.
                float localT = (float)(y - sector.MinY) / sectorHeight;
                if (localT < 0f) localT = 0f;
                if (localT > 1f) localT = 1f;

                // Выбор сета: смешивание около середины.
                CellType[] palette;
                if (localT < 0.4f)
                {
                    palette = topPalette;
                }
                else if (localT > 0.6f)
                {
                    palette = bottomPalette;
                }
                else
                {
                    // Смешивание: вероятность bottom растёт от 0 до 1.
                    float blend = (localT - 0.4f) / 0.2f;
                    palette = rand.NextDouble() < blend ? bottomPalette : topPalette;
                }

                // Узор внутри выбранного сета.
                float t = Fbm(x, y, fillSeed);
                if (_fillSharpness > 0f)
                {
                    t = t + ((t - 0.5f) * _fillSharpness);
                    if (t < 0f) t = 0f;
                    if (t > 1f) t = 1f;
                }

                int paletteLen = palette.Length;
                int paletteIndex = (int)(t * paletteLen);
                if (paletteIndex < 0) paletteIndex = 0;
                if (paletteIndex >= paletteLen) paletteIndex = paletteLen - 1;

                CellType cell = palette[paletteIndex];
                cells[index] = cell;
            }
        }

        private static CellType[] BuildPalette(DepthSet set, bool gig)
        {
            var list = new List<CellType>();

            if (gig)
            {
                list.AddRange(set.CrystalsGig);
            }
            else
            {
                list.AddRange(set.Minerals);
                list.AddRange(set.CrystalsNormal);
            }

            return list.ToArray();
        }

        private float Fbm(int x, int y, int fillSeed)
        {
            float frequency = _fillFrequency;
            float amplitude = 1f;
            float norm = 0f;
            float sum = 0f;

            for (int o = 0; o < _fillOctaves; o++)
            {
                int octaveSeed = fillSeed + (o * 1013);
                float v = ValueNoise(x * frequency, y * frequency, octaveSeed);
                sum += v * amplitude;
                norm += amplitude;
                amplitude *= _fillGain;
                frequency *= _fillLacunarity;
            }

            return norm > 0f ? sum / norm : 0f;
        }

        private static float ValueNoise(float x, float y, int seed)
        {
            int x0 = (int)MathF.Floor(x);
            int y0 = (int)MathF.Floor(y);
            float fx = x - x0;
            float fy = y - y0;

            float v00 = Hash01(x0, y0, seed);
            float v10 = Hash01(x0 + 1, y0, seed);
            float v01 = Hash01(x0, y0 + 1, seed);
            float v11 = Hash01(x0 + 1, y0 + 1, seed);

            float sx = fx * fx * (3f - (2f * fx));
            float sy = fy * fy * (3f - (2f * fy));

            float a = v00 + ((v10 - v00) * sx);
            float b = v01 + ((v11 - v01) * sx);
            return a + ((b - a) * sy);
        }

        private static float Hash01(int x, int y, int seed)
        {
            uint hash = (uint)((x * 374761393) ^ (y * 668265263) ^ (seed * 1274126177));
            hash = (hash ^ (hash >> 13)) * 1274126177;
            hash ^= hash >> 16;
            return (hash & 0xFFFFFF) / 16777216f;
        }

        private static DepthSet[] BuildSets() => new[]
        {
            new DepthSet(
                crystalsGig: new[] { CellType.Green, CellType.Blue },
                crystalsNormal: new[] { CellType.Green, CellType.Blue },
                minerals: new[] { CellType.Rock, CellType.BlueRock, CellType.YellowSand, CellType.Lava }),

            new DepthSet(
                crystalsGig: new[] { CellType.Green, CellType.Blue, CellType.XGreen, CellType.XBlue },
                crystalsNormal: new[] { CellType.Green, CellType.Blue },
                minerals: new[] { CellType.Rock, CellType.BlueRock, CellType.YellowSand, CellType.Lava }),

            new DepthSet(
                crystalsGig: new[] { CellType.Green, CellType.Blue, CellType.XGreen, CellType.XBlue },
                crystalsNormal: new[] { CellType.Green, CellType.Blue, CellType.XGreen },
                minerals: new[] { CellType.Rock, CellType.BlueRock, CellType.YellowSand, CellType.DarkYellowSand, CellType.Lava }),

            new DepthSet(
                crystalsGig: new[] { CellType.Blue, CellType.XBlue, CellType.Green, CellType.XGreen },
                crystalsNormal: new[] { CellType.Green, CellType.Blue, CellType.XBlue },
                minerals: new[] { CellType.Rock, CellType.BlueRock, CellType.GoldenRock, CellType.BlueSand }),

            new DepthSet(
                crystalsGig: new[] { CellType.Blue, CellType.Red, CellType.XBlue, CellType.XRed },
                crystalsNormal: new[] { CellType.Blue, CellType.Red },
                minerals: new[] { CellType.BlueRock, CellType.GoldenRock, CellType.BlueSand, CellType.DarkBlueSand }),

            new DepthSet(
                crystalsGig: new[] { CellType.Red, CellType.Violet, CellType.XRed, CellType.XViolet },
                crystalsNormal: new[] { CellType.Blue, CellType.Red, CellType.Violet },
                minerals: new[] { CellType.BlueRock, CellType.GoldenRock, CellType.WhiteSand, CellType.DarkWhiteSand }),

            new DepthSet(
                crystalsGig: new[] { CellType.Red, CellType.Violet, CellType.XRed, CellType.XViolet },
                crystalsNormal: new[] { CellType.Red, CellType.Violet },
                minerals: new[] { CellType.BlueRock, CellType.GoldenRock, CellType.GreenRock, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.PassiveAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.Violet, CellType.XViolet, CellType.White },
                crystalsNormal: new[] { CellType.Violet, CellType.XViolet },
                minerals: new[] { CellType.GoldenRock, CellType.GreenRock, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.PassiveAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.Violet, CellType.XViolet, CellType.Cyan, CellType.XCyan },
                crystalsNormal: new[] { CellType.Violet, CellType.Cyan },
                minerals: new[] { CellType.GoldenRock, CellType.GreenRock, CellType.WhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.PassiveAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.Cyan, CellType.XCyan, CellType.White },
                crystalsNormal: new[] { CellType.Cyan, CellType.White, CellType.XCyan },
                minerals: new[] { CellType.GreenRock, CellType.GRock, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.XGreen, CellType.XBlue, CellType.XViolet, CellType.XCyan },
                crystalsNormal: new[] { CellType.XGreen, CellType.XBlue, CellType.XViolet },
                minerals: new[] { CellType.GreenRock, CellType.GoldenRock, CellType.GRock, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.XGreen, CellType.XBlue, CellType.XViolet, CellType.XCyan },
                crystalsNormal: new[] { CellType.XGreen, CellType.XBlue, CellType.XCyan },
                minerals: new[] { CellType.GoldenRock, CellType.GreenRock, CellType.GRock, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.XGreen, CellType.XRed, CellType.XViolet, CellType.XCyan },
                crystalsNormal: new[] { CellType.XRed, CellType.XViolet, CellType.XCyan },
                minerals: new[] { CellType.GoldenRock, CellType.GreenRock, CellType.GRock, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.XGreen, CellType.XRed, CellType.XViolet, CellType.XCyan },
                crystalsNormal: new[] { CellType.XGreen, CellType.XBlue },
                minerals: new[] { CellType.GRock, CellType.Obsidian, CellType.Coralite, CellType.EtherealRock, CellType.GrayAcid }),

            new DepthSet(
                crystalsGig: new[] { CellType.XGreen, CellType.XBlue, CellType.XRed, CellType.XViolet, CellType.XCyan },
                crystalsNormal: Array.Empty<CellType>(),
                minerals: new[] { CellType.Ultralit, CellType.EtherealRock, CellType.Coralite, CellType.Obsidian, CellType.GRock, CellType.MetalBoulder1, CellType.MetalBoulder2, CellType.MetalBoulder3, CellType.GrayAcid, CellType.Pearl }),
        };
    }
}
