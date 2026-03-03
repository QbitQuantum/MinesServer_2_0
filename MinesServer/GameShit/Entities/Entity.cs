using MinesServer.GameShit.WorldSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Entities
{
    /// <summary>
    /// Base class for all entities.
    /// </summary>
    public abstract class Entity
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int ChunkX
        {
            get => (int)Math.Floor((float)x / 32);
        }
        public int ChunkY
        {
            get => (int)Math.Floor((float)y / 32);
        }
        public IEnumerable<(int x, int y)> vChunksAround(int radius = 2)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int chunkX = ChunkX + dx;
                    int chunkY = ChunkY + dy;

                    if (World.ValidChunk(chunkX, chunkY))
                        yield return (chunkX, chunkY);
                }
            }
        }
        public IEnumerable<Chunk> vChunksAroundEx(int radius = 2)
        {
            foreach (var (chunkX, chunkY) in vChunksAround(radius))
                yield return World.W.chunks[chunkX, chunkY];
        }
    }
}
