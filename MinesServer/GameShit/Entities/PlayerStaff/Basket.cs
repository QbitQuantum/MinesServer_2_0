using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Crystal;
using MinesServer.Network.GUI;
using MinesServer.Server;
using Newtonsoft.Json;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Basket
    {
        public Basket() { }

        [NotMapped] public CrystalCBStorage CrystalCB { get; set; } = new();
        public int id { get; set; }
        public string Serialazed { get; set; } = "[]";
        public long AllCry => CrystalTypeExt.CrysType.Sum(t => this[t]);
        public bool ShouldSubscribe => Changed is null;

        public event Action? Changed;
        public int capacity = 1;

        private long[] _cry = new long[6];

        [NotMapped] public long[] crystall
        {
            get
            {
                _cry ??= JsonConvert.DeserializeObject<long[]>(Serialazed) ?? new long[6];
                return _cry;
            }
        }

        public long this[CrystalType type]
        {
            get => this[(int)type];
            set => this[(int)type] = value;
        }

        public long this[int type]
        {
            get => crystall[type];
            set => crystall[type] = value;
        }

        public long Sum() => crystall.Sum();
        public void NotifyChanged() => Changed?.Invoke();

        public void SaveToDatabase()
        {
            Serialazed = JsonConvert.SerializeObject(_cry);
            using var db = new DataBase();
            db.baskets.Update(this);
            db.SaveChanges();
        }

        public void AddCrys(CrystalType type, long val)
        {
            this[type] += val;
            if (this[type] < 0)
                this[type] = long.MaxValue;
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

        public bool RemoveCrys(int indexCrystallType, long value)
        {
            if (indexCrystallType >= 6 || indexCrystallType < 0 || value < 0) return false;
            CrystalType type = CrystalTypeExt.CrysType[indexCrystallType];
            return RemoveCrys(type, value);
        }

        public bool RemoveCrys(CrystalType type, long value)
        {
            if (type == CrystalType.Unknown || value < 0) return false;

            if (this[type] >= value)
            {
                this[type] -= value;
                NotifyChanged();
                return true;
            }
            return false;
        }

        public BasketPacket BPacket => new BasketPacket(
            this[CrystalType.Green],
            this[CrystalType.Blue],
            this[CrystalType.Red],
            this[CrystalType.Violet],
            this[CrystalType.White],
            this[CrystalType.Cyan],
            capacity
        );
    }
}