using Newtonsoft.Json;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace MinesServer.GameShit.SysCraft
{
    public static class RDes
    {
        public static List<Recipie> recipies;
        public static void Init()
        {
            recipies = GetResipies();
        }
        public static List<Recipie> GetResipies()
        {
            var rid = 0;
            var list = new List<Recipie>();
            if (!Directory.Exists("recipies"))
            {
                Directory.CreateDirectory("recipies");
                // Basic default recipes for core items.
                var defaults = new[]
                {
                    new Recipie
                    {
                        time = 10,
                        result = new RC(0, 1), // Teleport
                        costcrys = new[] { new RC(0, 10) }, // green crystals
                        costres = Array.Empty<RC>()
                    },
                    new Recipie
                    {
                        time = 20,
                        result = new RC(1, 1), // Respawn
                        costcrys = new[] { new RC(0, 20), new RC(2, 5) },
                        costres = new[] { new RC(0, 1) } // Teleport
                    },
                    new Recipie
                    {
                        time = 30,
                        result = new RC(24, 1), // Crafter
                        costcrys = new[] { new RC(0, 30), new RC(1, 20) },
                        costres = new[] { new RC(2, 2), new RC(3, 1) } // simple building components
                    },
                    new Recipie
                    {
                        time = 25,
                        result = new RC(29, 1), // Storage
                        costcrys = new[] { new RC(0, 20), new RC(1, 10) },
                        costres = new[] { new RC(2, 1) }
                    },
                    new Recipie
                    {
                        time = 40,
                        result = new RC(26, 1), // Gun
                        costcrys = new[] { new RC(2, 20), new RC(3, 10) },
                        costres = new[] { new RC(24, 1) } // requires Crafter
                    }
                };

                var index = 0;
                foreach (var recipe in defaults)
                {
                    var fileName = Path.Combine("recipies", $"default_{index}.json");
                    File.WriteAllText(fileName, JsonConvert.SerializeObject(recipe, Formatting.Indented));
                    index++;
                }
            }
            foreach (var path in Directory.GetFiles("recipies/", "*.json"))
            {
                var r = JsonConvert.DeserializeObject<Recipie>(File.ReadAllText(path));
                var n = r;
                n.id = rid;
                r = n;
                list.Add(r);
                rid++;
            }
            return list;
        }
        public static Recipie ByResultId(int res_id)
        {
            return recipies.FirstOrDefault(i => i.result.id == res_id);
        }
    }
}
