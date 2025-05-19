using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Sys_Miss
{
    public static class MissonSerializer
    {
        public static void Load()
        {
            if (!Directory.Exists("missons")) Directory.CreateDirectory("missons");
            var files = Directory.GetFiles("missons/");
            var count = 0;
            foreach (var i in files)
            {
                var miss = JsonConvert.DeserializeObject<MissonBase>(File.ReadAllText(i));
                miss.id = 0;
                //missonsbase.Add();
            }
        }
        public static List<MissonBase> missonsbase = new();
    }
}
