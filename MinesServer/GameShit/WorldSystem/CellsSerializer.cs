using Newtonsoft.Json;

namespace MinesServer.GameShit.WorldSystem
{
    public static class CellsSerializer
    {
        public static void Load()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var targetPath = Path.Combine(baseDir, "cells.json");

            // Поднимаемся из bin/Debug/net8.0-windows7.0/ в корень проекта
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var sourcePath = Path.Combine(projectDir, "cells.json");

            // Если исходный файл существует, копируем его
            if (File.Exists(sourcePath))
            {
                // Проверяем, нужно ли обновить (по дате или содержимому)
                if (!File.Exists(targetPath) ||
                    File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
                {
                    File.Copy(sourcePath, targetPath, true);
                    Console.WriteLine($"Copied cells.json to: {targetPath}");
                }
                cells = JsonConvert.DeserializeObject<Cell[]>(File.ReadAllText(targetPath));
            }
            else
            {
                // Если нет исходного, создаем в папке бинарников
                cells = new Cell[126];
                for (int i = 0; i < 126; i++)
                {
                    cells[i] = new Cell((byte)i);
                }
                File.WriteAllText(targetPath, JsonConvert.SerializeObject(cells, Formatting.Indented));
                Console.WriteLine($"Created default cells.json at: {targetPath}");
            }
        }
        public static Cell[] cells;
    }
}
