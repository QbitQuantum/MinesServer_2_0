using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Sys_Clan
{
    public class Rank
    {
        public const int OwnerPriority = 100;
        public const int OfficerPriority = 20;
        public const int Recruit = 0;

        public int id { get; set; }
        public int priority { get; set; }
        public string colorhex { get; set; } = "#FFFFFF";
        public string name { get; set; } = string.Empty;
        public Clan owner { get; set; } = null!;
        public bool Outranks(Rank? other) =>
            other is not null && priority > other.priority;

        public static List<Rank> DefaultRanks =>
        [
            new Rank { name = "Новобранец", priority = Recruit,         colorhex = "#00FF00" },
            new Rank { name = "Ветеран",    priority = OfficerPriority, colorhex = "#ff0000" },
            new Rank { name = "Создатель",  priority = OwnerPriority,   colorhex = "#006400" }
        ];

    }
}
