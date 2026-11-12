using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Buildings
{
    public abstract class Pack : Entity
    {
        public Pack()
        {
        }
        public Pack(int x, int y, int ownerid)
        {
            if (x == 0 && y == 0)
                throw new Exception("Админ, не занимайся фигнёй");
            this.x = x; this.y = y; this.ownerid = ownerid;
        }

        public virtual int cid { get; set; }
        public virtual int off { get; set; }
        public virtual int PackId { get; set; } = -1;
        protected virtual bool Roll { get; set; } = Physics.r.Next(1, 101) < 40;
        public virtual IReadOnlyDictionary<(int x, int y), Func<Player, Window?>> KeysGUIOpen
        {
            get { return new Dictionary<(int x, int y), Func<Player, Window?>> { [(x, y)] = GUIWin }; }
        }

        protected Func<Player, Window?> MakeGui(Func<Player, Window?> factory)
            => p => (p.cid == cid || p.id == ownerid) && !p.HasActiveProgram ? factory(p) : null;

        protected void TryToReturnInventory(Player p)
        {
            if (Roll)
            {
                p.SendLocalPrivateMessage(0, x, y, "Пак разобран. Часть деталей сохранена.");
                p.inventory[PackId]++;
            }
        }

        public virtual PackType type { get; }
        public virtual int ownerid { get; set; }

        public abstract Window? GUIWin(Player p);
        protected abstract void ClearBuilding();
        public abstract void Destroy(Player p);

        // ---- Кэш последнего отправленного состояния ----
        private char _lastSentType;
        private int _lastSentX = int.MinValue;
        private int _lastSentY = int.MinValue;
        private int _lastSentCid = -1;
        private int _lastSentOff = -1;
        private bool _hasSent;

        /// <summary>
        /// Отправляет пакет клиенту только если состояние пака изменилось
        /// (type / x / y / cid / off). Защищает от пересоздания объекта на клиенте каждый тик.
        /// </summary>
        protected void SendPackIfChanged()
        {
            if (_hasSent
                && _lastSentType == (char)type
                && _lastSentX == x
                && _lastSentY == y
                && _lastSentCid == cid
                && _lastSentOff == off)
                return;

            RememberSentState();
            World.UpdatePack(this);
        }

        /// <summary>
        /// Запоминает текущее состояние как «уже отправленное».
        /// Вызывается из SendPackIfChanged и Build (после AddPack, который сам шлёт пакет).
        /// </summary>
        private void RememberSentState()
        {
            _hasSent = true;
            _lastSentType = (char)type;
            _lastSentX = x;
            _lastSentY = y;
            _lastSentCid = cid;
            _lastSentOff = off;
        }

        public virtual void Update()
        {
            SendPackIfChanged();
        }

        public virtual void Build()
        {
            World.AddPack(this);
            RememberSentState();
        }
    }
}