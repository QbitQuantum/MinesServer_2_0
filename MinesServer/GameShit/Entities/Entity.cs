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
            var valid = bool (int x, int y) => x >= 0 && y >= 0 && x < World.ChunksW && y < World.ChunksH;
            for (int y = -2; y <= 2; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    var lchunkx = ChunkX + x;
                    var lchunky = ChunkY + y;
                    if (valid(lchunkx, lchunky))
                    {
                        yield return (lchunkx, lchunky);
                    }
                }
            }
            yield break;
        }
        public IEnumerable<Chunk> vChunksAroundEx()
        {
            var valid = bool (int x, int y) => x >= 0 && y >= 0 && x < World.ChunksW && y < World.ChunksH;
            for (int y = -2; y <= 2; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    var lchunkx = ChunkX + x;
                    var lchunky = ChunkY + y;
                    if (valid(lchunkx, lchunky))
                    {
                        yield return World.W.chunks[lchunkx,lchunky];
                    }
                }
            }
            yield break;
        }
    }
}
