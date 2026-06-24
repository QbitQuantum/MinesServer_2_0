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
        public Gen(int width, int height)
        {
            Height = height;
            Width = width;
        }

        private int Height { get; }
        private int Width { get; }

        public void StartGeneration()
        {
            Console.WriteLine("Generating sectors");
            var sec = new Sectors((Width, Height));
            sec.GenerateENoise(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Cubic);
            sec.AddW(15, 1, RcherNZ.AccidentalNoise.InterpolationType.Linear);
            sec.AddW(25, 5, RcherNZ.AccidentalNoise.InterpolationType.Linear);
            sec.AddW(35, 20, RcherNZ.AccidentalNoise.InterpolationType.Quintic);
            sec.End();
            var map = sec.map;
            var rc = 0;
            for (int x = 0; x < Width; x += 32)
            {
                for (int y = 0; y < Height; y += 32)
                {
                    for (int chx = 0; chx < 32; chx++)
                    {
                        for (int chy = 0; chy < 32; chy++)
                        {
                            int index = (x + chx) * Height + (y + chy);
                            var m_value = map[index].value;
                            var cell = m_value == 2 ? CellType.BlackRock : 
                                m_value == 1 ? CellType.RedRock : CellType.Empty;
                            World.SetCell((x + chx), (y + chy), cell);
                            rc++;
                        }
                    }
                }
            }
            Console.Write($"\r{rc}/{map.Length} saving rocks");
            sec.DetectAndFillSectors();
            Console.WriteLine("END END");
        }
    }
}
