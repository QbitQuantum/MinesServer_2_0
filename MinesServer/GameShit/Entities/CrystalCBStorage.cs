
// Структура для хранения CB разных типов кристаллов
using MinesServer.Enums;

namespace MinesServer.GameShit.Entities
{
    public class CrystalCBStorage
    {
        public float GlobalCB { get; set; }

        public Dictionary<CrystalType, float> Values { get; private set; } = new();

        public float Get(CrystalType type)
        {
            return Values.TryGetValue(type, out float value) ? value : 0f;
        }

        public void Set(CrystalType type, float value)
        {
            Values[type] = value;
        }

        public void Add(CrystalType type, float value)
        {
            Values[type] = Get(type) + value;
        }
    }
}