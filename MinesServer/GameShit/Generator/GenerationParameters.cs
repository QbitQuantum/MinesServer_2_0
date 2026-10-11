using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Generator
{
    public readonly record struct GenerationParameters(
        int Seed,

        // Длины
        float ThickLengthFactor,
        float MidLengthFactor,
        float ThinLengthFactor,

        // Толщины
        float ThickThicknessFactor,
        float MidThicknessFactor,

        // Количество
        float ThickDensityFactor,
        float MidDensityFactor,
        float ThinDensityFactor,

        // Частоты извилистости
        float ThickWiggleFrequency,
        float MidWiggleFrequency,
        float ThinWiggleFrequency,

        // Извилистость
        float ThickAngleScale,
        float MidAngleScale,
        float ThinAngleScale,

        // Антипетли
        float MaxAngleDelta,

        int MinSectorSize,
        bool DiagonalConnectivity,

        int SectorCount,
        int GigChancePercent,

        float FillFrequency,      // частота шума заливки (0.02..0.2)
        int FillOctaves,          // число октав шума заливки (2..5)
        float FillLacunarity,     // множитель частоты между октавами
        float FillGain,           // множитель амплитуды между октавами
        float FillSharpness)      // 0 = плавные переходы, 1 = резкие границы
    {
        public static GenerationParameters Default(int seed = 1337) => new(
            Seed: seed,
            ThickLengthFactor: 1.5f,
            MidLengthFactor: 0.6f,
            ThinLengthFactor: 0.2f,
            ThickThicknessFactor: 0.002f,
            MidThicknessFactor: 0.001f,
            ThickDensityFactor: 200f,
            MidDensityFactor: 20f,
            ThinDensityFactor: 3f,
            ThickWiggleFrequency: 30f,
            MidWiggleFrequency: 80f,
            ThinWiggleFrequency: 200f,
            ThickAngleScale: 0.15f,
            MidAngleScale: 0.5f,
            ThinAngleScale: 1.0f,
            MaxAngleDelta: 0.15f,
            MinSectorSize: 8,
            DiagonalConnectivity: true,
            SectorCount: 15,
            GigChancePercent: 20,
            FillFrequency: 0.06f,
            FillOctaves: 3,
            FillLacunarity: 2.0f,
            FillGain: 0.5f,
            FillSharpness: 0.7f);
    }

    public sealed class GenerationResult
    {
        public CellType[] Cells { get; }
        public IReadOnlyList<GeneratedSector> Sectors { get; }
        public int Width { get; }
        public int Height { get; }

        public GenerationResult(
            CellType[] cells,
            IReadOnlyList<GeneratedSector> sectors,
            int width,
            int height)
        {
            Cells = cells;
            Sectors = sectors;
            Width = width;
            Height = height;
        }

        public CellType GetCell(int x, int y) => Cells[(y * Width) + x];

        public bool InBounds(int x, int y) =>
            (uint)x < (uint)Width && (uint)y < (uint)Height;
    }
}
