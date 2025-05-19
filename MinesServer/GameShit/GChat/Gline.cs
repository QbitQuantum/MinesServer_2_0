using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.GChat
{
    public class GLine
    {
        [NotMapped]
        public int time = (int)(DateTime.Now.Ticks / 10000L / 60000L);
        public int id { get; set; }
        [NotMapped]
        public Player player {
            get => DataBase.GetPlayer(playerid);
            set => playerid = value.id;
                }
        public int playerid { get; set; }
        public string message { get; set; }
        public Chat owner { get; set; }
    }
}
