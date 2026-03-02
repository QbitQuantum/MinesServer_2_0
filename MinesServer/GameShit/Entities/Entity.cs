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
        public IEnumerable<(int x, int y)> vChunksAround()
        {
            for (int y = -2; y <= 2; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    var lchunkx = ChunkX + x;
                    var lchunky = ChunkY + y;
                    if (World.W.ValidChunk(lchunkx, lchunky))
                    {
                        yield return (lchunkx, lchunky);
                    }
                }
            }
            yield break;
        }
        public IEnumerable<Chunk> vChunksAroundEx()
        {
            for (int y = -2; y <= 2; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    var lchunkx = ChunkX + x;
                    var lchunky = ChunkY + y;
                    if (World.W.ValidChunk(lchunkx, lchunky))
                    {
                        yield return World.W.chunks[lchunkx,lchunky];
                    }
                }
            }
            yield break;
        }
    }
}
