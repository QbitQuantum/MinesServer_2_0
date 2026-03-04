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
        public IEnumerable<(int x, int y)> vChunksAround(int radius = 2)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    var ChunkPos = Chunk.GetChunkPosByCoords(x, y);
                    int chunkX = ChunkPos.x + dx;
                    int chunkY = ChunkPos.y + dy;

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
