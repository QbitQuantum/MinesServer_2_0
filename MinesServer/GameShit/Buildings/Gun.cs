using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.Skills;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public sealed class Gun : PackCharge
    {
        private Gun() { }

        public Gun(int x, int y, int ownerid, int cid) : base(x, y, ownerid, 1000, 10000)
        {
            this.cid = cid;
            charge = 1000;
        }

        #region fields

        private static readonly TimeSpan AttackInterval = TimeSpan.FromSeconds(1);

        private const int attackRadius = 20;
        private const int chunkRadius = (attackRadius + Chunk.ChunkWidth - 1) / Chunk.ChunkWidth;
        public const int attackRadiusSq = attackRadius * attackRadius;

        private readonly List<Chunk> cachedChunks = [];
        private readonly List<Player> playersInRangeBuffer = [];

        private DateTime lastAttackTime = ServerTime.Now;

        [NotMapped] public override PackType type => PackType.Gun;
        [NotMapped] public override int PackId => 26;

        public override int off { get { return charge > 0 ? 1 : 0; } }

        #endregion

        #region affectworld

        private void CachedChunks()
            => cachedChunks.AddRange(World.W.GetChunksInRange(x, y, chunkRadius));

        public override void Build()
        {
            CachedChunks();
            World.SetCell(x, y, 32, true);
            World.SetCell(x + 1, y, 35, true);
            World.SetCell(x - 1, y, 35, true);
            World.SetCell(x, y - 1, 35, true);
            World.SetCell(x, y + 1, 35, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            base.Build();
        }
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 35, false);
            World.SetCell(x - 1, y, 35, false);
            World.SetCell(x, y - 1, 35, false);
            World.SetCell(x, y + 1, 35, false);
            World.SetCell(x + 1, y + 1, 35, false);
            World.SetCell(x - 1, y + 1, 35, false);
            World.SetCell(x + 1, y - 1, 35, false);
            World.SetCell(x - 1, y - 1, 35, false);
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.guns.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[26]++;
            }
        }
        #endregion
        public void Fill(Player p, long val)
        {
            if (charge == maxcharge)
            {
                return;
            }
            using var db = new DataBase();
            db.Attach(this);
            if (p.crys[CrystalType.Cyan] < val)
            {
                val = p.crys[CrystalType.Cyan];
            }
            if (p.crys.RemoveCrys(CrystalType.Cyan, val))
            {
                charge += (int)val;
            }
            db.SaveChanges();
            p.win = GUIWin(p);
            p.SendWindow();
        }
        public override Window? GUIWin(Player p)
        {
            MButton[] fillbuttons = [p.crys[CrystalType.Cyan] >= 100 ? new MButton("+100", "fill:100", (args) => Fill(p, 100)) : new MButton("+100", "fill:100"),
                p.crys[CrystalType.Cyan] >= 1000 ? new MButton("+1000", "fill:1000", (args) => Fill(p, 1000)) : new MButton("+1000", "fill:1000"),
                p.crys[CrystalType.Cyan] >= 0 ? new MButton("max", "fill:max", (args) => Fill(p, (long)(maxcharge - charge))) : new MButton("max", "fill:max")
               ];
            return new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "gun",
                    Label = "хуй",
                    Title = "Пушка",
                    InitialPage = new Page()
                    {
                        RichList = new RichListConfig()
                        {
                            Entries = [RichListEntry.Fill("заряд", (int)charge, (int)maxcharge, CrystalType.Cyan, fillbuttons[0], fillbuttons[1], fillbuttons[2])]
                        },
                        Buttons = []
                    }
                }]
            };
        }

        private void UpdateAttackPlayer()
        {
            // Проверяем, прошла ли секунда с последнего выстрела
            if (ServerTime.Now - lastAttackTime < AttackInterval)
                return;

            playersInRangeBuffer.Clear();

            foreach (var chunk in cachedChunks)
            {
                if (chunk.bots.IsEmpty) continue;

                foreach (var kvp in chunk.bots)
                {
                    var player = kvp.Value;

                    if (player.cid == cid)
                        continue;

                    int dx = player.x - x;
                    int dy = player.y - y;

                    if (dx * dx + dy * dy <= attackRadiusSq)
                        playersInRangeBuffer.Add(player);
                }
            }

            if (playersInRangeBuffer.Count != 0 && charge != 0)
            {
                foreach (var player in playersInRangeBuffer)
                {
                    player.Hurt(60, DamageTypePlayer.Gun);
                    player.SendDFToBots(7, x, y, player.id, 1);

                    float basecrys = 0.5f;

                    if (player.skillslist != null)
                    {
                        basecrys *= player.skillslist.HandleInductionReceived();
                    }

                    charge = (int)MathF.Max(0, charge - basecrys);
                }

                lastAttackTime = ServerTime.Now;
            }
        }

        public override void Update()
        {
            UpdateAttackPlayer();
            base.Update();
        }
    }
}
