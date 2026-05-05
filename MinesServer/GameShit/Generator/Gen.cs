using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.GameShit.Generator
{
    // TODO: Унести этот TODO в World.cs
    // TODO: Забрать все логику генерации мира из World.cs
    // И сделать все статически.
    // И даже сам класс, потому что это сиглтон пофакту
    // Будешь делать вид что будем "мокать" - получишь леща по шее
    public class Gen
    {
        public List<(int, int)> spawns;
        public static Gen THIS;
        public Gen(int width, int height)
        {
            THIS = this;
            Gen.height = height;
            Gen.width = width;
            spawns = new List<(int, int)>();

        }
        public static int height;
        public static int width;
        public void StartGeneration()
        {
            Console.WriteLine("Generating sectors");
            var sec = new Sectors((width, height));
            sec.GenerateENoise(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Cubic);
            sec.AddW(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Linear);
            sec.AddW(25, 5, RcherNZ.AccidentalNoise.InterpolationType.Linear);
            sec.AddW(35, 20, RcherNZ.AccidentalNoise.InterpolationType.Quintic);
            sec.End();
            var map = sec.map;
            var rc = 0;
            for (int x = 0; x < width; x += 32)
            {
                for (int y = 0; y < height; y += 32)
                {
                    for (int chx = 0; chx < 32; chx++)
                    {
                        for (int chy = 0; chy < 32; chy++)
                        {
                            int index = (x + chx) * height + (y + chy);
                            var m_value = map[index].value;
                            var t = 
                                m_value == 2 ? (byte)CellType.BlackRock : 
                                m_value == 1 ? (byte)CellType.RedRock : 
                                (byte)CellType.Empty;
                            World.SetCell((x + chx), (y + chy), t);
                            rc++;
                        }

                    }
                }
                Console.Write($"\r{rc}/{map.Length} saving rocks");
            }
            sec.DetectAndFillSectors();
            Console.WriteLine("END END");
        }
    }
}
