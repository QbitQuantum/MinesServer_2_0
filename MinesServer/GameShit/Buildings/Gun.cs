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
using System.Numerics;

namespace MinesServer.GameShit.Buildings
{
    public class Gun : Pack, IDamagable
    {
        #region fields
        public override PackType type => PackType.Gun;
        public int hp { get; set; }
        public int maxhp { get; set; }
        public override float charge { get => base.charge; set => base.charge = value; }
        public float maxcharge { get; set; }
        public override int cid { get; set; }
        public override int off { get { return charge > 0 ? 1 : 0; } }
        public DateTime brokentimer { get; set; }
        #endregion
        private const int attackRadius = 20;
        private const int chunkRadius = (attackRadius + Chunk.ChunkWidth - 1) / Chunk.ChunkWidth;
        public const int attackRadiusSq = attackRadius * attackRadius;

        public override int PackId => 26;
        public Gun(int x, int y, int ownerid, int cid) : base(x, y, ownerid)
        {
            this.cid = cid;
            hp = 1000;
            maxhp = 1000;
            charge = 1000;
            maxcharge = 10000;
        }
        private Gun() { }
        #region affectworld
        public override void Build()
        {
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
            World.RemovePack(x, y);
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
            if (p.crys.RemoveCrys((int)CrystalType.Cyan, val))
            {
                charge += (int)val;
                World.W.GetChunk(x, y).ResendPack(this);
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
        public override void Update()
        {
            int chunkX = x / Chunk.ChunkWidth;
            int chunkY = y / Chunk.ChunkHeight;

            List<Player> playersInRange = new List<Player>();

            for (int cx = -chunkRadius; cx <= chunkRadius; cx++)
            {
                for (int cy = -chunkRadius; cy <= chunkRadius; cy++)
                {
                    int tgChunkX = chunkX + cx;
                    int tgChunkY = chunkY + cy;

                    if (tgChunkX < 0 || tgChunkY < 0 ||
                        tgChunkX >= Chunk.ChunksW || tgChunkY >= Chunk.ChunksH)
                        continue;

                    var chunk = World.W.chunks[tgChunkX, tgChunkY];
                    if (chunk.bots.Count == 0) continue;

                    foreach (var playerId in chunk.bots.Keys)
                    {
                        var player = DataBase.GetPlayer(playerId);
                        if (player == null || player.cid == cid) continue;

                        float dx = player.x - x;
                        float dy = player.y - y;
                        float sqrDistance = dx * dx + dy * dy;

                        if (sqrDistance <= attackRadiusSq)
                            playersInRange.Add(player);
                    }
                }
            }
            if (playersInRange.Count != 0 && charge != 0)
            {
                foreach (var player in playersInRange)
                {
                    player.Hurt(60, DamageTypePlayer.Gun);
                    player.SendDFToBots(7, x, y, player.id, 1);

                    float basecrys = 0.5f;

                    if (player.skillslist?.skills != null && player.skillslist.skills.Count > 0)
                    {
                        float inductionMultiplier = 1f;
                        foreach (var skill in player.skillslist.skills.Values)
                        {
                            if (skill == null || skill.type != SkillType.Induction)
                                continue;

                            if (skill.UseSkill(SkillEffectType.OnHurt, player))
                                inductionMultiplier *= (skill.Effect / 100f);
                        }

                        if (inductionMultiplier != 1f)
                        {
                            basecrys *= inductionMultiplier;
                        }
                    }

                    if (charge - basecrys > 0)
                    {
                        charge -= basecrys;
                    }
                    else
                    {
                        charge = 0;
                    }
                }
            }
            World.W.GetChunk(x, y).ResendPack(this);
        }
    }
}
