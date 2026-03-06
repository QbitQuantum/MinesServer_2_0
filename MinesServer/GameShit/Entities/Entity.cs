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
            return World.W.GetChunk(x, y).GetNeighboringChunkCoordinates(radius);
        }

        public IEnumerable<Chunk> GetVisibleChunks(int radius = 2)
        {
            return World.W.GetChunk(x, y).GetNeighboringChunks(radius);
        }
    }
}
