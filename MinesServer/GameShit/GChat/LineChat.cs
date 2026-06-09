using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.Server;

namespace MinesServer.GameShit.GChat
{
    public class LineChat
    {
        public int id { get; set; }
        public int playerid { get; set; }
        public string message { get; set; }
        public Chat owner { get; set; }

        [NotMapped] public int time = (int)(DateTime.Now.Ticks / 10000L / 60000L);
        
        [NotMapped] public Player? player 
        {
            get => DataBase.GetPlayer(playerid);
            set => playerid = value!.id;
        }
    }
}
