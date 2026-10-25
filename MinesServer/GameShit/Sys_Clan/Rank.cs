using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.ClanSystem
{
    public class Rank
    {
        public int id { get; set; }
        public int priority { get; set; }
        public string colorhex { get; set; } = "#FFFFFF";
        public string name { get; set; } = string.Empty;
        public Clan owner { get; set; } = null!;
        [NotMapped] public int ClanId { get; set; }
        public bool Outranks(Rank? other) =>
            other is not null && priority > other.priority;
    }
}
