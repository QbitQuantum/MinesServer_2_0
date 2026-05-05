using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Generator
{
    // TODO: Использовать struct с ref
    // ref var _sector = ref map[index];
    public class SectorCell
    {
        public float value;
        public int sector;
        public (int x, int y) pos;
        public CellType type = CellType.Empty;
    }
}
