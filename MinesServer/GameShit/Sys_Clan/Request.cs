using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.GameShit.Entities.PlayerStaff;

namespace MinesServer.GameShit.ClanSystem
{
    public class Request
    {
        public int id { get; set; }
        public int PlayerId { get; set; }
        public Player player { get; set; } = null!;
        public DateTime reqtime { get; set; } = DateTime.UtcNow;
        [NotMapped] public int ClanId { get; set; }
        [NotMapped] public TimeSpan TimeLeft => TimeSpan.FromDays(1) - (DateTime.UtcNow - reqtime);
        [NotMapped] public bool IsExpired => TimeLeft <= TimeSpan.Zero;
    }
}
