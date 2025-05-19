using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.World;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MinesServer.GameShit.Entities
{
    /// <summary>
    /// Base class for Player-like Entities
    /// </summary>
    public abstract class PEntity : Entity
    {
        public ProgrammatorData programsData
        {
            get
            {
                _pdata ??= new ProgrammatorData(this);
                return _pdata;
            }
        }
        [NotMapped]
        public virtual Stack<byte> geo { get; set; } = new();
        protected ProgrammatorData? _pdata { get; set; }
        public abstract Basket? crys { get; set; }
        public virtual int Health { get; set; }
        public virtual int MaxHealth { get; set; }
        public virtual int pause { get; set; }
        public virtual double ServerPause { get; }
        public virtual int cid { get; set; }
        public int dir { get; set; }
        public abstract void Build(string type);
        public abstract void Bz();
        public virtual void Geo()
        {
            int x = (int)GetDirCord().x, y = (int)GetDirCord().y;
            if (!World.W.ValidCoord(x, y) || !World.AccessGun(x, y, cid).access)
            {
                return;
            }
            var cell = World.GetCell(x, y);
            if (World.GetProp(cell).isPickable && !World.GetProp(cell).isEmpty)
            {
                geo.Push(cell);
                World.Destroy(x, y);
            }
            else if (World.GetProp(cell).isEmpty && World.GetProp(cell).can_place_over && geo.Count > 0 && !World.PackPart(x, y))
            {
                var cplaceable = geo.Pop();
                World.SetCell(x, y, cplaceable);
                World.SetDurability(x, y, World.isCry(cplaceable) ? 0 : Physics.r.Next(1, 101) > 99 ? 0 : World.GetProp(cplaceable).durability);
            }
        }
        public abstract bool Heal(int num = -1);
        public abstract void Hurt(int num, DamageType type = DamageType.Pure);
        public abstract void Death();
        public abstract bool Move(int x, int y, int dir = -1,bool prog = false);
        public abstract void Update();
        public long GetBox(int x, int y)
        {
            var b = Box.GetBox(x, y);
            if (b == null)return 0;
            crys?.Boxcrys(b.bxcrys);
            using var db = new DataBase();
            db.Remove(b);
            db.SaveChanges();
            return b.AllCrys;
        }
        public (int x, int y) GetDirCord(bool pack = false)
        {
            var x = (this.x + (dir == 3 ? 1 : dir == 1 ? -1 : 0));
            var y = (this.y + (dir == 0 ? 1 : dir == 2 ? -1 : 0));
            if (pack)
            {
                x = (this.x + (dir == 3 ? 2 : dir == 1 ? -2 : 0));
                y = (this.y + (dir == 0 ? 2 : dir == 2 ? -2 : 0));
            }
            return (x, y);
        }
        #region Renders
        public void SendFXoBots(int fx, int fxx, int fxy)
        {
            foreach (var chunk in vChunksAroundEx())
            {
                foreach (var player in chunk.bots.Select(id => DataBase.GetPlayer(id.Key)))
                {
                    player?.connection?.SendB(new HBPacket([new HBFXPacket(fxx, fxy, fx)]));
                }
            }
        }
        public void SendLocalMsg(string msg)
        {
            foreach (var chunk in vChunksAroundEx())
            {
                foreach (var player in chunk.bots.Select(id => DataBase.GetPlayer(id.Key)))
                {
                    player?.connection?.SendB(new HBPacket([new HBChatPacket(id, x, y, msg)]));
                }
            }
        }
        public void SendDFToBots(int fx, int fxx, int fxy, int bid, int dir, int col = 0)
        {
            foreach (var chunk in vChunksAroundEx())
            {
                foreach (var player in chunk.bots.Select(id => DataBase.GetPlayer(id.Key)))
                {
                    player?.connection?.SendB(new HBPacket([new HBDirectedFXPacket(bid, fxx, fxy, fx, dir, col)]));
                }
            }
        }
        #endregion
    }
}
