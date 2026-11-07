using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.WorldSystem
{
    public class Box : Entity
    {
        private Box() { }

        public long AllCrys => _crysinside.Sum();

        public long[] BoxCrystall => _crysinside;

        [NotMapped] private readonly long[] _crysinside = new long[6];

        public static Box? GetBox(int x, int y)
        {
            if (!World.ValidCoord(x, y))
                return null;
            return DataBase.GetBox(x, y);
        }

        /// <summary>
        /// Установка бокса на месте разрушения пака
        /// </summary>
        public static void BuildBox(int x, int y, long[] crystalls)
        {
            BuildBox(x, y, crystalls, null, false);
        }

        public static void BuildBox(int x, int y, long[] crystalls, Player? p, bool force = false)
        {
            var (boxX, boxY) = force ? World.FindEmptyForBox(x, y) : (x, y);

            var cell = World.GetCell(boxX, boxY);

            if (!(World.GetProp(cell).can_place_over && 
                World.IsEmpty(boxX, boxY)) && !force)
            {
                return;
            }

            var box = new Box();

            for (int i = 0; i < 6; i++)
            {
                long count_cry = crystalls[i];

                if (p == null)
                {
                    box.BoxCrystall[i] = count_cry;
                }
                else
                {
                    if (p.crys.RemoveCrys(i, count_cry))
                    {
                        box.BoxCrystall[i] = count_cry;
                    }
                }
            }

            if (box.BoxCrystall.Sum() <= 0) return;

            box.x = boxX;
            box.y = boxY; 

            using (var db = new DataBase())
            {
                db.boxes.Add(box);
                db.SaveChanges();
            }

            World.SetCell(boxX, boxY, 90);
        }
    }
}
