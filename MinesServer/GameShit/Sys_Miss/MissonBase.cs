using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.Network.TypicalEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinesServer.Network.Tutorial;
using System.Security.Policy;

namespace MinesServer.GameShit.Sys_Miss
{
    public abstract class MissonBase(string url)
    {
        public int id { get; set; }
        public MissonType type { get; init; }
        public int progress { get; set; }
        public int progressneed { get; set; }
        public string imageurl { get; set; } = url;
        public void CheckComplete()
        {

        }
        public void SendMisson(Player p)
        {
            var x = ImgSpace.LocateImg(p.id.ToString(), "https://www.kindpng.com/picc/m/123-1230504_hatsune-miku-wallpaper-phone-hd-png-download.png");
            p.connection?.SendU(new MissionPanelPacket(x, 200, 200, "Hitler", 0));
        }
    }
}
