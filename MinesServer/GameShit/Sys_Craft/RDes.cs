using MinesServer.Enums;
using Newtonsoft.Json;

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
                    // Базовые ресурсы (кристаллы)
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(3).TotalSeconds,
                        result = new RC((int)Item.Polymer, 1), // POLY
                        costcrys = [new RC((int)CrystalType.Green, 2400)], // зеленые кристаллы
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(4).TotalSeconds,
                        result = new RC((int)Item.Accumulator, 1), // ACCU
                        costcrys = [new RC((int)CrystalType.Blue, 2400)], // синие кристаллы
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(5).TotalSeconds,
                        result = new RC((int)Item.NanoBot, 1), // NANO
                        costcrys = [new RC((int)CrystalType.Red, 2400)], // красные кристаллы
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(6).TotalSeconds,
                        result = new RC((int)Item.Translator, 1), // TRANS
                        costcrys = [new RC((int)CrystalType.Violet, 2400)], // фиолетовые кристаллы
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(7).TotalSeconds,
                        result = new RC((int)Item.Compressor, 1), // COMP
                        costcrys = [new RC((int)CrystalType.White, 2400)], // белые кристаллы
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(8).TotalSeconds,
                        result = new RC((int)Item.C190, 1), // C190
                        costcrys = [new RC((int)CrystalType.Cyan, 2400)], // голубые кристаллы
                    },
    
                    // Компоненты и предметы
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(10).TotalSeconds,
                        result = new RC((int)Item.PlasmaBomb, 1), // BOOM
                        costres = [new RC((int)Item.C190, 4)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(300).TotalSeconds,
                        result = new RC((int)Item.GeopackRedSkal, 1), // G117 (Geopack с красноскалом)
                        costres = [new RC((int)Item.GeopackBlackSkal, 12)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(40).TotalSeconds,
                        result = new RC((int)Item.ProtonBomb, 1), // PROT
                        costres = [new RC((int)Item.Compressor, 140), new RC((int)Item.C190, 280)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(15).TotalSeconds,
                        result = new RC((int)Item.DischargeBomb, 1), // RAZ
                        costres = [new RC((int)Item.PlasmaBomb, 12), new RC((int)Item.C190, 4)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(360).TotalSeconds,
                        result = new RC((int)Item.Gun, 1), // GUN
                        costres = [new RC((int)Item.ProtonBomb, 120), new RC((int)Item.Polymer, 120)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(20).TotalSeconds,
                        result = new RC((int)Item.Geopack, 1), // GEO (пустой геопак)
                        costres = [new RC((int)Item.Compressor, 12), new RC((int)Item.Accumulator, 12)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(120).TotalSeconds,
                        result = new RC((int)Item.Teleporter, 1), // TPR (портативный телепортер)
                        costres = [new RC((int)Item.Translator, 120), new RC((int)Item.Accumulator, 40)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(360).TotalSeconds,
                        result = new RC((int)Item.Storage, 1), // STOCK (строительный пак склада)
                        costres = [new RC((int)Item.Geopack, 20), new RC((int)Item.Polymer, 28)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(300).TotalSeconds,
                        result = new RC((int)Item.Market, 1), // MARKET
                        costres = [new RC((int)Item.Storage, 4), new RC((int)Item.Polymer, 240)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(180).TotalSeconds,
                        result = new RC((int)Item.ConstructorBot, 1), // KR
                        costres = [new RC((int)Item.NanoBot, 16), new RC((int)Item.Accumulator, 24)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(200).TotalSeconds,
                        result = new RC((int)Item.Teleport, 1), // TP (строительный пак телепорта)
                        costres = [new RC((int)Item.Teleporter, 16), new RC((int)Item.Polymer, 160)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(650).TotalSeconds,
                        result = new RC((int)Item.Crafter, 1), // CRAFT
                        costres = [new RC((int)Item.ConstructorBot, 400), new RC((int)Item.Polymer, 800)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(180).TotalSeconds,
                        result = new RC((int)Item.DefenseCharge, 1), // ZZ
                        costres = [new RC((int)Item.NanoBot, 16), new RC((int)Item.C190, 24)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(160).TotalSeconds,
                        result = new RC((int)Item.Up, 1), // UP
                        costres = [new RC((int)Item.DefenseCharge, 28), new RC((int)Item.Polymer, 200)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(10).TotalSeconds,
                        result = new RC((int)Item.RepairBot, 1), // REM
                        costres = [new RC((int)Item.NanoBot, 4)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(250).TotalSeconds,
                        result = new RC((int)Item.Respawn, 1), // RESP
                        costres = [new RC((int)Item.RepairBot, 28), new RC((int)Item.Polymer, 160)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(360).TotalSeconds,
                        result = new RC((int)Item.ClanBuilding, 1), // CLANS
                        costres = [new RC((int)Item.Respawn, 8), new RC((int)Item.Polymer, 240)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(360).TotalSeconds,
                        result = new RC((int)Item.ClanGate, 1), // GATE
                        costres = [new RC((int)Item.Teleport, 4), new RC((int)Item.Polymer, 160)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(25).TotalSeconds,
                        result = new RC((int)Item.VolcanoRadar, 1), // VRD
                        costres = [new RC((int)Item.Translator, 4)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(50).TotalSeconds,
                        result = new RC((int)Item.GeodeRadar, 1), // ARD
                        costres = [new RC((int)Item.VolcanoRadar, 4), new RC((int)Item.Translator, 12)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(1200).TotalSeconds,
                        result = new RC((int)Item.CombatGenerator, 1), // BG
                        costres = [new RC((int)Item.ProtonBomb, 16), new RC((int)Item.C190, 100)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(180).TotalSeconds,
                        result = new RC((int)Item.BombShop, 1), // BSHOP
                        costres = [new RC((int)Item.CombatGenerator, 8), new RC((int)Item.Polymer, 120)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        result = new RC((int)Item.BotRadar, 1), // BRD
                        costres = [new RC((int)Item.GeodeRadar, 4), new RC((int)Item.Translator, 16)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        result = new RC((int)Item.ScanRadar, 1), // SKAN
                        costres = [new RC((int)Item.BotRadar, 4), new RC((int)Item.Translator, 4)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(400).TotalSeconds,
                        result = new RC((int)Item.Automator, 1), // AM
                        costres = [new RC((int)Item.Teleporter, 28), new RC((int)Item.ScanRadar, 28), new RC((int)Item.ConstructorBot, 28)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(500).TotalSeconds,
                        result = new RC((int)Item.EMIBomb, 1), // EMI
                        costres = [new RC((int)Item.Teleporter, 16), new RC((int)Item.ScanRadar, 12), new RC((int)Item.ProtonBomb, 8)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(480).TotalSeconds,
                        result = new RC((int)Item.SpotBot, 1), // SPOT
                        costres = [new RC((int)Item.RepairBot, 4), new RC((int)Item.ConstructorBot, 4), new RC((int)Item.Polymer, 24)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(300).TotalSeconds,
                        result = new RC((int)Item.GeopackBlackSkal, 1), // G114 (геопак с черноскалом)
                        costres = [new RC((int)Item.GeopackBlack, 4)]
                    },
                    new Recipie
                    {
                        time = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        result = new RC((int)Item.Disassembler, 1), // DIZZ
                        costres = [new RC((int)Item.C190, 160), new RC((int)Item.ConstructorBot, 40)]
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
