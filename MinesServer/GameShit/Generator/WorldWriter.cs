using MinesServer.GameShit.WorldSystem;

namespace MinesServer.GameShit.Generator
{
    public sealed class WorldWriter
    {
        private readonly int _chunkWidth;
        private readonly int _chunkHeight;
        private readonly int _worldWidth;
        private readonly int _worldHeight;

        public WorldWriter(
            int worldWidth, int worldHeight,
            int chunkWidth, int chunkHeight)
        {
            _worldWidth = worldWidth;
            _worldHeight = worldHeight;
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
        }

        public void Write(GenerationResult result)
        {
            for (int baseX = 0; baseX < _worldWidth; baseX += _chunkWidth)
            {
                for (int baseY = 0; baseY < _worldHeight; baseY += _chunkHeight)
                {
                    int endX = Math.Min(baseX + _chunkWidth, _worldWidth);
                    int endY = Math.Min(baseY + _chunkHeight, _worldHeight);

                    for (int x = baseX; x < endX; x++)
                    {
                        for (int y = baseY; y < endY; y++)
                        {
                            World.SetCell(x, y, result.Cells[(y * result.Width) + x]);
                        }
                    }
                }
            }
        }
    }
}
