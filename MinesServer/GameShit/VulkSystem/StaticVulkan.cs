using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.VulkSystem
{
    public static class StaticVulkan
    {
        private static bool already = false;
        public static async Task<int> CheckSpace(int x,int y)
        {
            if (already) return 0;
            already = true;
            var result = 0;
            result = await Task.Run(() =>
            {
                for (int cx = -1; cx <= 1; cx++)
                {
                    for (int cy = -1; cy <= 1; cy++)
                    {
                        if (!World.TrueEmpty(x + cx, y + cy)) return -1;
                    }
                }
                using var db = new DataBase();
                if (db.vulkans.AsEnumerable().Where((i) => Vector2.Distance(new Vector2(x, y), new Vector2(i.x, i.y)) < 70).Count() > 0) return -1;
                return 0;
            });
            var count = result != -1 ? await Task.Run(() =>
            {
                var dirs = new (int, int)[] { (0, 1), (1, 0), (-1, 0), (0, -1) };
                var q = new Queue<(int, int)>();
                var valid = bool (int tx, int ty) => Vector2.Distance(new Vector2(x, y), new Vector2(tx, ty)) < 50 && World.TrueEmpty(tx, ty) && World.W.ValidCoord(tx, ty);
                var count = 0;
                q.Enqueue((x, y));
                while (q.Count > 0)
                {
                    var b = q.Dequeue();
                    count++;
                    foreach (var dir in dirs)
                    {
                        if (valid(b.Item1 + dir.Item1, b.Item2 + dir.Item2))
                        {
                            q.Enqueue((b.Item1 + dir.Item1, b.Item2 + dir.Item2));
                            continue;
                        }
                    }
                    if (count > 999) break;
                }
                using var db = new DataBase();
                if (db.vulkans.AsEnumerable().Where((i) => Vector2.Distance(new Vector2(x, y), new Vector2(i.x, i.y)) < 70).Count() > 0) return -1;
                return count;
            }) : 0;
            already = false;
            return count;
        }
    }
}
