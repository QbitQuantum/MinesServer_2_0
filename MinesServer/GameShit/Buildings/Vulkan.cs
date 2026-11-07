using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Generator;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.VulkSystem
{
    public enum VulkanState { Sleeping, Warning }

    public class Vulkan : Pack
    {
        #region fields

        [NotMapped] public override PackType type => PackType.Vulkan;
        [NotMapped] public override int PackId { get; set; }
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }
        [NotMapped] public override int ownerid { get; set; }

        private DateTime lastvulkanupdate { get; set; } = ServerTime.Now;
        private DateTime lastStateCheck { get; set; } = ServerTime.Now;
        public DateTime startTime { get; set; } = ServerTime.Now;
        public DateTime wakeTime { get; set; } = ServerTime.Now;
        public DateTime explodeTime { get; set; } = ServerTime.Now;
        public VulkanState state { get; set; }
        public int intensity { get; set; }

        #endregion

        private Vulkan() { }

        public Vulkan(int x, int y, int intensity) : base(x, y, 0)
        {
            if (intensity < 1) intensity = 1;
            if (intensity > 5) intensity = 5;
            
            this.intensity = intensity;
            startTime = ServerTime.Now;
            
            // Фиксированное время сна: 10 минут
            wakeTime = startTime + TimeSpan.FromMinutes(10);
            
            // Время до взрыва после пробуждения (чем выше intensity, тем быстрее)
            int explodeMinutes = intensity switch
            {
                1 => 10,
                2 => 8,
                3 => 5,
                4 => 3,
                5 => 2,
                _ => 5
            };
            explodeTime = wakeTime + TimeSpan.FromMinutes(explodeMinutes);
            
            state = VulkanState.Sleeping;
            
            using var db = new DataBase();
            db.vulkans.Add(this);
            db.SaveChanges();
        }

        public override Window? GUIWin(Player p) => null;

        protected override void ClearBuilding()
        {

        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.vulkans.Remove(this);
            db.SaveChanges();
        }

        public override void Update()
        {
            var now = ServerTime.Now;

            if ((now - lastStateCheck).TotalSeconds >= 1)
            {
                lastStateCheck = now;

                if (now >= wakeTime && state == VulkanState.Sleeping)
                {
                    Activate();
                }

                if (now >= explodeTime && state == VulkanState.Warning)
                {
                    Explode();
                }
            }

            if ((now - lastvulkanupdate).TotalSeconds >= 1)
            {
                lastvulkanupdate = now;
                base.Update();
            }
        }

        private void Activate()
        {
            state = VulkanState.Warning;

            // Отправляем эффект FX всем в радиусе
            World.W.SendFx(x, y, 5);
        }

        private void Explode()
        {
            // 1. Наносим урон всем игрокам в радиусе 3x3
            DamagePlayersInRadius();

            // 2. Разрушаем клетки в радиусе 3x3
            DestroyCellsInRadius();

            // 3. Спавним кристаллы в секторе вокруг
            SpawnCrystals();

            // 4. Уничтожаем пак
            Destroy(null!);
        }

        private void DamagePlayersInRadius()
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx, ny = y + dy;
                    var players = World.W.GetPlayersFromPos(nx, ny);
                    foreach (var p in players)
                    {
                        p.Hurt(50 + (int)(intensity * 10));
                        p.SendDFToBots(7, x, y, p.id, 1);
                    }
                }
            }
        }

        private void DestroyCellsInRadius()
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (World.ValidCoord(nx, ny))
                    {
                        World.Destroy(nx, ny);
                    }
                }
            }
        }

        private void SpawnCrystals()
        {
            // Создаём "виртуальный сектор" вокруг точки взрыва
            int sectorRadius = 10 + intensity * 3;

            // Генерируем индексы клеток в круге
            var cellIndices = new List<int>();
            Random rand = new Random((int)DateTime.UtcNow.Ticks ^ intensity ^ x ^ y);

            for (int dx = -sectorRadius; dx <= sectorRadius; dx++)
            {
                for (int dy = -sectorRadius; dy <= sectorRadius; dy++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (!World.ValidCoord(nx, ny)) continue;

                    // Проверяем что в круге
                    if (dx * dx + dy * dy > sectorRadius * sectorRadius) continue;

                    // Проверяем что клетка пустая или песок
                    var prop = World.GetProp(nx, ny);
                    if (!(prop.isEmpty || prop.isSand))
                        continue;

                    cellIndices.Add(nx + ny * World.CellsWidth);
                }
            }

            if (cellIndices.Count == 0) return;

            // Создаём виртуальный сектор
            var sector = new GeneratedSector(
                id: int.MaxValue,
                cellIndices: cellIndices.ToArray(),
                minX: x - sectorRadius,
                minY: y - sectorRadius,
                maxX: x + sectorRadius,
                maxY: y + sectorRadius,
                worldWidth: World.CellsWidth
            );
            
            int setIndex = (int)((float)(sector.CenterY) / World.CellsHeight * 14);
            setIndex = Math.Clamp(setIndex, 0, 14);

            var sets = SectorFiller.BuildSets();
            var set = sets[setIndex];

            bool gig = rand.Next(1, 101) <= 20;
            CellType[] palette = gig ? set.CrystalsGig : set.CrystalsNormal;

            if (palette.Length == 0) palette = set.Minerals;

            // Заполняем клетки кристаллами
            foreach (int index in sector.CellIndices)
            {
                int cx = index % World.CellsWidth;
                int cy = index / World.CellsWidth;

                // Случайный выбор из палитры
                CellType type = palette[rand.Next(palette.Length)];

                // Проверяем что клетка всё ещё пустая
                var currentProp = World.GetProp(cx, cy);
                if (!(currentProp.isEmpty || currentProp.isSand))
                    continue;

                World.SetCell(cx, cy, (byte)type);
                World.DamageCell(cx, cy, currentProp.durability, Operator.Unknown);
            }
        }
    }
}
