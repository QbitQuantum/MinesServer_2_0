using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.Server;

namespace MinesServer.GameShit.SysMarket
{
    public class Order
    {
        public int id { get; set; }
        public int initiatorid { get; set; }
        public int itemid { get; set; }
        public int num { get; set; }
        public long cost { get; set; }
        public DateTime bettime { get; set; }
        public void Bet(Player p, long money)
        {
            if ((buyerid > 0 ? Math.Ceiling(cost + (cost * 0.01f)) : cost) > money || p.money < cost)
            {
                return;
            }
            using var db = new DataBase();
            Player? buyer = null;
            if (buyerid != 0)
            {
                buyer = DataBase.GetPlayer(buyerid);
                buyer.money += cost;
                buyer.SendMoney();
            }
            cost = money;
            buyerid = p.id;
            p.money -= money;
            p.SendMoney();
            bettime = ServerTime.Now;
            db.SaveChanges();
        }
        public void CheckReady()
        {
            if (TimeSpan.FromMinutes(5) <= (ServerTime.Now - bettime) && buyerid > 0)
            {
                using var db = new DataBase();
                db.orders.Remove(this);
                var buyer = DataBase.GetPlayer(buyerid);
                if (buyer != null && buyer.inventory != null)
                {
                    buyer.inventory[itemid] += num;
                }
                else
                {
                    db.inventories.First(i => i.Id == buyerid)[itemid] += num;
                }
                var initiator = DataBase.GetPlayer(initiatorid);
                if (initiator != null)
                {
                    initiator.money += cost;
                    initiator.SendMoney();
                }
                db.SaveChanges();
            }
        }
        public void OwnerCancelBet()
        {

        }
        public int buyerid { get; set; }
    }
}
