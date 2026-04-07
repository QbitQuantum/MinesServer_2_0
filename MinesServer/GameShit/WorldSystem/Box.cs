using MinesServer.Enums;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.WorldSystem
{
    public class Box : Entity
    {
        [NotMapped]
        public long[] bxcrys = new long[6];
        private Box() { }
        private Box(bool n) { }
        public static Box? GetBox(int x, int y)
        {
            if (!World.ValidCoord(x, y))
                return null;
            return DataBase.GetBox(x, y);
        }
        public static void BuildBox(int x, int y, long[] cry, Player? p, bool force = false)
        {
            var cell = World.GetCell(x, y);
            if (!(World.GetProp(cell).can_place_over && World.IsEmpty(x, y)) && !force)
            {
                return;
            }
            var box = new Box(true);
            for (int i = 0; i < 6; i++)
            {
                long remcry = cry[i];
                if (p == null)
                {
                    box.bxcrys[i] = remcry;
                }
                else if (p.crys.RemoveCrys(CrystalTypeExt.CrysType[i], remcry))
                {
                    box.bxcrys[i] = remcry;
                }
            }
            if (box.bxcrys.Sum() <= 0)
            {
                return;
            }
            box.y = y; box.x = x;
            using (var db = new DataBase())
            {
                db.boxes.Add(box);
                db.SaveChanges();
            }
            World.SetCell(x, y, 90);
        }
        public long AllCrys => bxcrys.Sum();
        public long this[CrystalType crystal]
        {
            get => bxcrys[(int)crystal];
            set => bxcrys[(int)crystal] = value;
        }
        public long ze
        {
            get { return bxcrys[0]; }
            set { bxcrys[0] = value; }
        }

        public long cr
        {
            get { return bxcrys[1]; }
            set { bxcrys[1] = value; }
        }

        public long si
        {
            get { return bxcrys[2]; }
            set { bxcrys[2] = value; }
        }

        public long be
        {
            get { return bxcrys[3]; }
            set { bxcrys[3] = value; }
        }

        public long fi
        {
            get { return bxcrys[4]; }
            set { bxcrys[4] = value; }
        }

        public long go
        {
            get { return bxcrys[5]; }
            set { bxcrys[5] = value; }
        }
    }
}
