using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.GChat
{
    public class LineChat
    {
        public int id { get; set; }
        public int playerid { get; set; }
        public string playername { get; set; } = "";
        public int playercid { get; set; }
        public string message { get; set; } = "";
        public int time { get; set; } = (int)(DateTime.Now.Ticks / 10000L / 60000L);
    }
}