using Microsoft.EntityFrameworkCore.Diagnostics;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Consumables;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Server;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Inventory
    { 
        public int Id { get; set; }
        public Inventory()
        {
            itemstobd ??= JsonConvert.SerializeObject(new Dictionary<int, int>());
            typeditems = new Dictionary<int, ItemUsage>
            {
                {
                    0,(p) => {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -2, 1, coord.x, coord.y, p))
                        {
                            new Teleport(coord.x, coord.y, p.id).Build();
                            return true;
                        }
                        return false;
                     }
                },
                {
                    1,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 6, -2, 3, coord.x, coord.y, p))
                        {
                            new Resp(coord.x, coord.y, p.id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    2,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -3, 4, coord.x, coord.y, p))
                        {
                            new Up(coord.x, coord.y, p.id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    3,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-3, 3, -3, 3, coord.x, coord.y, p))
                        {
                            new Market(coord.x, coord.y, p.id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    4,(p) => true
                },
                {
                    5,ShitClass.Boom
                },
                {
                    6,ShitClass.Prot
                },
                {
                    7,(p) => ShitClass.Raz(p)
                },
                {
                    10,(p) => ShitClass.Geopack(10,p)
                },
                {
                    11,(p) => ShitClass.Geopack(11,p)
                },
                {
                    12,(p) => ShitClass.Geopack(12,p)
                },
                {
                    13,(p) => ShitClass.Geopack(13,p)
                },
                {
                    14,(p) => ShitClass.Geopack(14,p)
                },
                {
                    15,(p) => ShitClass.Geopack(15,p)
                },
                {
                    16,(p) => ShitClass.Geopack(16,p)
                },
                {
                    24,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        Console.WriteLine(coord);
                        if (World.W.CanBuildPack(-2, 2, -2, 2, coord.x, coord.y, p))
                        {
                            new Crafter(coord.x, coord.y, p.id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    26,
                    (p) =>
                    {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -2, 2, coord.x, coord.y, p) && p.clan != null)
                        {
                            new Gun(coord.x, coord.y, p.id, p.cid).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    27,
                    (p) =>
                    {
                        var coord = p.GetDirCord();
                        var c = World.AccessGun(coord.x,coord.y,p.cid);
                        if (p.clan != null && c.access && c.anygun)
                        {
                            ShitClass.Gate(coord.x,coord.y,p);
                            return true;
                        }
                        return false;
                    }
                },
                {
                    29, (p) => {
                        var coord = p.GetDirCord(true);
                        if (World.W.CanBuildPack(-2, 2, -2, 1, coord.x, coord.y, p))
                        {
                            new Storage(coord.x, coord.y, p.id).Build();
                            return true;
                        }
                        return false;
                    }
                },
                {
                    34,(p) => ShitClass.Geopack(34,p)
                },
                {
                    35,ShitClass.Poli
                },
                {
                    40,ShitClass.C190Shot
                },
                {
                    42,(p) => ShitClass.Geopack(42,p)
                },
                {
                    43,(p) => ShitClass.Geopack(42,p)
                },
                {
                    46,(p) => ShitClass.Geopack(46,p)
                },
            };
        }
        public int this[int index]
        {
            get
            {
                if (!items.ContainsKey(index))
                    items[index] = 0;
                return items[index];
            }
            set
            {
                using var db = new DataBase();
                db.inventories.Attach(this);
                items[index] = value;
                itemstobd = JsonConvert.SerializeObject(items);
                DataBase.GetPlayer(Id)?.SendInventory();
                db.SaveChanges();
            }
        }
        public InventoryPacket InvToSend()
        {
            if (miniq.Count < 4 && Lenght > 0) foreach (var i in items) AddChoose(i.Key);
            var invgrid = minv ? miniq.Select(i => new KeyValuePair<int, int>(i, this[i])).ToDictionary() : items;
            return new InventoryPacket(new InventoryShowPacket(invgrid, selected, Lenght));
        }
        public DateTime time = DateTime.Now;
        public void Use(Player p)
        {
            if (DateTime.Now - time >= TimeSpan.FromMilliseconds(400))
            {
                if (typeditems.ContainsKey(selected) && !World.ContainsPack(p.GetDirCord().x, p.GetDirCord().y, out var pack)
                    && (World.GetProp(p.GetDirCord().x, p.GetDirCord().y).can_place_over
                    || selected is 40 or (>=10and<17) or 34 or 42 or 43 or 46) && this[selected] > 0)
                {
                    if (typeditems[selected](p))
                    {
                        this[selected]--;
                        if (this[selected] == 0)miniq.Remove(selected);
                        p.SendInventory();
                    }
                }
                time = DateTime.Now;
            }
        }
        private void AddChoose(int item)
        {
            if (miniq.Contains(item) || item == -1)
                return;
            if (miniq.Count >= 4) miniq.Remove(miniq.First());
            miniq.Add(item);
        }
        public bool minv = true;
        private List<int> miniq = new();
        public Dictionary<int, ItemUsage> typeditems;
        public delegate bool ItemUsage(Player p);
        public void Choose(int id, Player p)
        {
            AddChoose(id);
            ITopLevelPacket packet = InventoryPacket.Choose("ты хуесос", new bool[0, 0], 123, 123, 12);
            selected = id;
            if (id == -1)
                packet = InventoryPacket.Close();
            p.connection?.SendU(InvToSend());
            p.connection?.SendU(packet);
        }
        public int selected = -1;
        [NotMapped]
        public int Lenght
        {
            get
            {
                var l = 0;
                List<int> remove = new();
                foreach (var i in items)
                    if (i.Value > 0) l++;
                    else remove.Add(i.Key);
                foreach (var key in remove)
                    items.Remove(key);
                return l;
            }
        }
        public string? itemstobd { get; set; }
        private Dictionary<int, int> _items;
        [NotMapped]
        private Dictionary<int, int> items
        {
            get => _items ??= JsonConvert.DeserializeObject<Dictionary<int, int>>(itemstobd);
            set => itemstobd = JsonConvert.SerializeObject(_items = value);
        }
    }
}
