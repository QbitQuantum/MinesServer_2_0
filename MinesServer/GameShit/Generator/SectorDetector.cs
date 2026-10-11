using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Generator
{
    public sealed class SectorDetector
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _minSize;
        private readonly bool _diagonal;

        public SectorDetector(int width, int height, int minSize, bool diagonal)
        {
            _width = width;
            _height = height;
            _minSize = minSize;
            _diagonal = diagonal;
        }

        public List<GeneratedSector> Detect(CellType[] cells)
        {
            var sectors = new List<GeneratedSector>();
            var visited = new bool[cells.Length];
            var queue = new Queue<int>();

            for (int i = 0; i < cells.Length; i++)
            {
                if (visited[i]) continue;
                if (cells[i] != CellType.Empty) continue;

                var sectorCells = new List<int>();
                queue.Clear();
                queue.Enqueue(i);
                visited[i] = true;

                int minX = int.MaxValue, minY = int.MaxValue;
                int maxX = int.MinValue, maxY = int.MinValue;

                while (queue.Count > 0)
                {
                    int index = queue.Dequeue();
                    int x = index % _width;
                    int y = index / _width;

                    sectorCells.Add(index);
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;

                    TryEnqueue(x - 1, y, cells, visited, queue);
                    TryEnqueue(x + 1, y, cells, visited, queue);
                    TryEnqueue(x, y - 1, cells, visited, queue);
                    TryEnqueue(x, y + 1, cells, visited, queue);

                    if (_diagonal)
                    {
                        TryEnqueue(x - 1, y - 1, cells, visited, queue);
                        TryEnqueue(x + 1, y - 1, cells, visited, queue);
                        TryEnqueue(x - 1, y + 1, cells, visited, queue);
                        TryEnqueue(x + 1, y + 1, cells, visited, queue);
                    }
                }

                if (sectorCells.Count < _minSize)
                {
                    foreach (int cellIndex in sectorCells)
                    {
                        cells[cellIndex] = CellType.RedRock;
                    }
                    continue;
                }

                sectors.Add(new GeneratedSector(
                    id: sectors.Count,
                    cellIndices: sectorCells.ToArray(),
                    minX: minX, minY: minY, maxX: maxX, maxY: maxY,
                    worldWidth: _width));
            }

            return sectors;
        }

        private void TryEnqueue(
            int x, int y, CellType[] cells, 
            bool[] visited, Queue<int> queue)
        {
            if ((uint)x >= (uint)_width || (uint)y >= (uint)_height) return;
            int index = (y * _width) + x;
            if (visited[index]) return;
            if (cells[index] != CellType.Empty) return;

            visited[index] = true;
            queue.Enqueue(index);
        }
    }
}
