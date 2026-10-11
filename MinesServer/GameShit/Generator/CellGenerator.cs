using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Generator
{
    public sealed class CellGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly GenerationParameters _p;

        private readonly int _thickLength;
        private readonly int _midLength;
        private readonly int _thinLength;
        private readonly int _thickThickness;
        private readonly int _midThickness;
        private readonly int _thinThickness;
        private readonly int _thickCount;
        private readonly int _midCount;
        private readonly int _thinCount;

        public CellGenerator(int width, int height, in GenerationParameters parameters)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;
            _p = parameters;

            float diagonal = MathF.Sqrt((float)((width * width) + (height * height)));

            _thickLength = Math.Max(20, (int)(diagonal * _p.ThickLengthFactor));
            _midLength = Math.Max(10, (int)(diagonal * _p.MidLengthFactor));
            _thinLength = Math.Max(5, (int)(diagonal * _p.ThinLengthFactor));

            _thickThickness = Math.Max(1, (int)MathF.Round(diagonal * _p.ThickThicknessFactor));
            _midThickness = Math.Max(1, (int)MathF.Round(diagonal * _p.MidThicknessFactor));
            _thinThickness = 1;

            int area = width * height;
            _thickCount = Math.Max(2, (int)(area / (diagonal * _p.ThickDensityFactor)));
            _midCount = Math.Max(5, (int)(area / (diagonal * _p.MidDensityFactor)));
            _thinCount = Math.Max(10, (int)(area / (diagonal * _p.ThinDensityFactor)));
        }

        public CellType[] Generate()
        {
            var cells = new CellType[_width * _height];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = CellType.Empty;
            }

            DrawLevel(cells, _thickCount, _thickLength, _thickThickness, _p.ThickAngleScale, _p.ThickWiggleFrequency, 0);
            DrawLevel(cells, _midCount, _midLength, _midThickness, _p.MidAngleScale, _p.MidWiggleFrequency, 1);
            DrawLevel(cells, _thinCount, _thinLength, _thinThickness, _p.ThinAngleScale, _p.ThinWiggleFrequency, 2);

            return cells;
        }

        private void DrawLevel(
            CellType[] cells,
            int lineCount,
            int lineLength,
            int thickness,
            float angleScale,
            float wiggleFrequency,
            int levelSeed)
        {
            for (int line = 0; line < lineCount; line++)
            {
                int lineSeed = _p.Seed + (levelSeed * 10007) + (line * 1013);
                DrawStroke(cells, lineLength, angleScale, wiggleFrequency, thickness, lineSeed);
            }
        }

        private void DrawStroke(
            CellType[] cells,
            int length,
            float angleScale,
            float wiggleFrequency,
            int thickness,
            int lineSeed)
        {
            var rand = new Random(lineSeed);

            float margin = 0.1f;
            float x = rand.Next((int)(_width * margin), (int)(_width * (1f - margin)));
            float y = rand.Next((int)(_height * margin), (int)(_height * (1f - margin)));

            float angle = (float)(rand.NextDouble() * Math.PI * 2);

            bool straight = angleScale <= 0f;
            float maxDelta = _p.MaxAngleDelta;

            float prevX = x;
            float prevY = y;

            for (int step = 0; step < length; step++)
            {
                if (!straight)
                {
                    float angleNoise = ValueNoise(step * wiggleFrequency, 0f, lineSeed + 9999);
                    float delta = (angleNoise - 0.5f) * angleScale;
                    if (delta > maxDelta) delta = maxDelta;
                    if (delta < -maxDelta) delta = -maxDelta;

                    angle += delta;
                }

                float nextX = x + MathF.Cos(angle);
                float nextY = y + MathF.Sin(angle);

                if (nextX < 0) { nextX = 0; angle = MathF.PI - angle; }
                if (nextX >= _width) { nextX = _width - 1; angle = MathF.PI - angle; }
                if (nextY < 0) { nextY = 0; angle = -angle; }
                if (nextY >= _height) { nextY = _height - 1; angle = -angle; }

                DrawThickSegment(cells, prevX, prevY, nextX, nextY, thickness);

                prevX = nextX;
                prevY = nextY;
                x = nextX;
                y = nextY;
            }
        }

        private void DrawThickSegment(
            CellType[] cells,
            float x0, float y0,
            float x1, float y1,
            int thickness)
        {
            float dx = x1 - x0;
            float dy = y1 - y0;
            float distance = MathF.Sqrt((dx * dx) + (dy * dy));

            int steps = Math.Max(1, (int)(distance * 2f));

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                float px = x0 + (dx * t);
                float py = y0 + (dy * t);
                DrawThickPoint(cells, (int)px, (int)py, thickness);
            }
        }

        private void DrawThickPoint(CellType[] cells, int cx, int cy, int thickness)
        {
            int r = thickness - 1;
            int r2 = r * r;

            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (r > 0 && (dx * dx) + (dy * dy) > r2) continue;

                    int px = cx + dx;
                    int py = cy + dy;

                    if ((uint)px >= (uint)_width || (uint)py >= (uint)_height) continue;

                    int index = (py * _width) + px;
                    cells[index] = CellType.RedRock;
                }
            }
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
    }
}
