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

        private Basket() { }

        public long this[CrystalType type]
        {
            get => cry[(int)type];
            set => cry[(int)type] = value;
        }
        private long[] _cry = null;
        [NotMapped]
        public long[] cry
        {
            get
            {
                _cry ??= JsonConvert.DeserializeObject<long[]>(serialazed ?? "[]");
                return _cry;
            }
        }

        // НОВЫЙ МЕТОД: явно сохранить изменения
        public void SaveToDatabase()
        {
            if (_cry != null)
            {
                serialazed = JsonConvert.SerializeObject(_cry);
                using var db = new DataBase();
                db.baskets.Update(this);
                db.SaveChanges();
            }
        }
        public void AddCrys(int index, long val)
        {
            cry[index] += val;
            if (cry[index] < 0)
                cry[index] = long.MaxValue;
            Changed?.Invoke();
        }
        public void Boxcrys(long[] crys)
        {
            for (var i = 0; i < cry.Length; i++)
                cry[i] += crys[i];
            if (Changed is not null) Changed();

        }
        public bool RemoveCrys(int index, long val)
        {
            if (val < 0) return false;
            if (cry[index] >= val)
            {
                cry[index] -= val;
                Changed?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Notify subscribers that basket contents were changed (e.g. after direct array mutation).
        /// </summary>
        public void NotifyChanged() => Changed?.Invoke();

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
                    Label = "Box",
                    Action = "dropbox",
                    InitialPage = new Page()
                    {
                        CrystalConfig = new CrystalConfig("  останется", "будет в боксе", [new CrysLine("", 0, 0, cry[0], 0),
                            new CrysLine("", 0, 0, cry[1], 0),
                            new CrysLine("", 0, 0, cry[2], 0),
                            new CrysLine("", 0, 0, cry[3], 0),
                            new CrysLine("", 0, 0, cry[4], 0),
                            new CrysLine("", 0, 0, cry[5], 0)]),
                        Text = "\nИспользуйте полосы прокрутки, чтобы выбрать сколько положить в бокс\",\r\n" +
                        "                    \"ВНИМАНИЕ! При создании бокса, количество кристаллов не уменьшается\n",
                        Buttons = [new MButton("<color=green>Добавить в бокс</color>", $"dropbox:{ActionMacros.CrystalSliders}", (args) => { p.BBox(args.CrystalSliders); })]
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
