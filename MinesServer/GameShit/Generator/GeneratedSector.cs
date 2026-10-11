namespace MinesServer.GameShit.Generator
{
    public sealed class GeneratedSector
    {
        public int Id { get; }
        public int[] CellIndices { get; }
        public int MinX { get; }
        public int MinY { get; }
        public int MaxX { get; }
        public int MaxY { get; }
        public int Size => CellIndices.Length;
        public int CenterX => (MinX + MaxX) / 2;
        public int CenterY => (MinY + MaxY) / 2;

        private readonly int _worldWidth;

        public GeneratedSector(
            int id,
            int[] cellIndices,
            int minX, int minY, int maxX, int maxY,
            int worldWidth)
        {
            Id = id;
            CellIndices = cellIndices;
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            _worldWidth = worldWidth;
        }

        public int GetX(int i) => CellIndices[i] % _worldWidth;
        public int GetY(int i) => CellIndices[i] / _worldWidth;
    }
}
