using MinesServer.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.Network.GUI;
using MinesServer.Server;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Basket
    {
        public int id { get; set; }
        public string? serialazed { get; set; }

        // TODO: Сделать частью БД
        [NotMapped] public CrystalCBStorage CrystalCB { get; set; } = new();

        public Basket(bool n)
        {
            _cry = new long[6];
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

        private long[]? _cry = null;

        [NotMapped]
        public long[] cry
        {
            get
            {
                _cry ??= JsonConvert.DeserializeObject<long[]>(serialazed ?? "[]") ?? new long[6];
                return _cry;
            }
        }

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

        public void AddCrys(CrystalType type, long val)
        {
            this[type] += val;
            if (this[type] < 0)
                this[type] = long.MaxValue;
            Changed?.Invoke();
        }

        public void BoxCrys(IReadOnlyDictionary<CrystalType, long> crys)
        {
            foreach (var kvp in crys)
            {
                this[kvp.Key] += kvp.Value;
            }
            Changed?.Invoke();
        }

        public void BoxCrys(long[] crys)
        {
            if (crys.Length != 6) throw new ArgumentException("Массив должен содержать 6 элементов", nameof(crys));

            this[CrystalType.Green] += crys[0];
            this[CrystalType.Blue] += crys[1];
            this[CrystalType.Red] += crys[2];
            this[CrystalType.Violet] += crys[3];
            this[CrystalType.White] += crys[4];
            this[CrystalType.Cyan] += crys[5];

            Changed?.Invoke();
        }

        public bool RemoveCrys(CrystalType type, long val)
        {
            if (val < 0) return false;

            if (this[type] >= val)
            {
                this[type] -= val;
                Changed?.Invoke();
                return true;
            }
            return false;
        }

        public long GetCrys(CrystalType type) => this[type];

        public void SetCrys(CrystalType type, long value)
        {
            this[type] = value;
            Changed?.Invoke();
        }

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
                        CrystalConfig = new CrystalConfig("  останется", "будет в боксе",
                            [
                                new CrysLine("", 0, 0, this[CrystalType.Green], 0),
                                new CrysLine("", 0, 0, this[CrystalType.Blue], 0),
                                new CrysLine("", 0, 0, this[CrystalType.Red], 0),
                                new CrysLine("", 0, 0, this[CrystalType.Violet], 0),
                                new CrysLine("", 0, 0, this[CrystalType.White], 0),
                                new CrysLine("", 0, 0, this[CrystalType.Cyan], 0)
                            ]),
                        Text = "\nИспользуйте полосы прокрутки, чтобы выбрать сколько положить в бокс\",\r\n" +
                        "                    \"ВНИМАНИЕ! При создании бокса, количество кристаллов не уменьшается\n",
                        Buttons = [new MButton("<color=green>Добавить в бокс</color>", $"dropbox:{ActionMacros.CrystalSliders}", (args) => { p.BBox(args.CrystalSliders); })]
                    }
                }]
            };
        }

        public BasketPacket BPacket => new BasketPacket(
            this[CrystalType.Green],
            this[CrystalType.Blue],
            this[CrystalType.Red],
            this[CrystalType.Violet],
            this[CrystalType.White],
            this[CrystalType.Cyan],
            Buildcap()
        );

        public int cap = 0;

        public long AllCry => CrystalTypeExt.CrysType.Sum(t => this[t]);

        public string GetCry => string.Join(":", CrystalTypeExt.CrysType.Select(t => this[t])) + ":" + cap;
    }
}