using Microsoft.IdentityModel.Tokens;
using MinesServer.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.Network.GUI;
using MinesServer.Server;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;


namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Basket
    {
        public int Id { get; set; }
        public string? serialazed { get; set; }
        public Basket(bool n)
        {
            _cry = [0, 0, 0, 0, 0, 0];
            serialazed = JsonConvert.SerializeObject(_cry);
        }
        public event Action Changed;
        public bool shouldsubscribe => Changed is null;
        private Basket()
        {
        }
        public long this[CrystalType type]
        {
            set => cry[(int)type] = value;

            get => cry[(int)type];
        }
        private long[] _cry = null;
        [NotMapped]
        public long[] cry
        {
            get
            {
                _cry ??= JsonConvert.DeserializeObject<long[]>(serialazed);
                using var db = new DataBase();
                db.baskets.Attach(this);
                serialazed = JsonConvert.SerializeObject(_cry);
                db.SaveChanges();
                return _cry;
            }
        }
        public void AddCrys(int index, long val)
        {
            cry[index] += val;
            if (cry[index] < 0)
                cry[index] = long.MaxValue;
            if (Changed is not null)Changed();
        }
        public void Boxcrys(long[] crys)
        {
            for (var i = 0; i < cry.Length; i++)
                cry[i] += crys[i];
            if (Changed is not null) Changed();

        }
        public void ClearCrys()
        {
            for (var i = 0; i < cry.Length; i++)
                cry[i] = 0;
            Changed();
        }
        public bool RemoveCrys(int index, long val)
        {
            if (val < 0)return false;
            
            if (cry[index] - val >= 0)
            {
                cry[index] -= val;
                if (Changed is not null) Changed();
                return true;
            }
            return false;
        }
        private int Buildcap()
        {
            return 1;
        }
        public Window OpenBoxGui(Player p)
        {
            return new Window()
            {
                ShowTabs = false,
                Title = "Создание бокса",
                Tabs = [new Tab()
                {
                    Label = "хуй",
                    Action = "dropbox",
                    InitialPage = new Page()
                    {
                        CrystalConfig = new CrystalConfig("  останется", "будет в боксе", [new CrysLine("", 0, 0, cry[0], 0),
                            new CrysLine("", 0, 0, cry[1], 0),
                            new CrysLine("", 0, 0, cry[2], 0),
                            new CrysLine("", 0, 0, cry[3], 0),
                            new CrysLine("", 0, 0, cry[4], 0),
                            new CrysLine("", 0, 0, cry[5], 0)]),
                        Text = "\nИспользуйте полосы прокрутки, чтобы выбрать сколько положить в бокс\",\r\n                    \"ВНИМАНИЕ! При создании бокса теряется нихуя кристаллов\n",
                        Buttons = [new MButton("<color=green>В БОКС</color>", $"dropbox:{ActionMacros.CrystalSliders}", (args) => { p.BBox(args.CrystalSliders); })]
                    }
                }]
            };
        }
        public BasketPacket BPacket => new BasketPacket(cry[0], cry[1], cry[2], cry[3], cry[4], cry[5], Buildcap());
        public int cap = 0;
        public long AllCry => cry.Select((t, i) => cry[i]).Sum();
        public string GetCry => cry.Aggregate("", (current, t) => current + t + ":") + cap;
    }
}
