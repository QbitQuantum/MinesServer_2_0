using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.GameShit.Entities;

namespace MinesServer.GameShit.Sys_Clan
{
    public class Request
    {
        public int id { get; set; }
        public int PlayerId { get; set; }
        public Player player { get; set; } = null!;
        public DateTime reqtime { get; set; } = DateTime.UtcNow;
        public int ClanId { get; set; }
        [NotMapped] public TimeSpan TimeLeft => TimeSpan.FromDays(1) - (DateTime.UtcNow - reqtime);
        [NotMapped] public bool IsExpired => TimeLeft <= TimeSpan.Zero;
    }
}
