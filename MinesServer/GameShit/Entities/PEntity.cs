using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.Bots;
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
        public void AddHp(int Hp)
        {
            Health = Math.Min(Health + Hp, MaxHealth);
        }
        public void ResetHp()
        {
            Health = MaxHealth;
        }
        public abstract void Build(string type);
        public abstract void Bz();
        public virtual void Geo() { }
        private void InverseDirection(DirectionType Direction)
        {
            switch (Direction)
            {
                case DirectionType.Down:
                    dir = (int)DirectionType.Up; break;
                case DirectionType.Left:
                    dir = (int)DirectionType.Right; break;
                case DirectionType.Up:
                    dir = (int)DirectionType.Down; break;
                case DirectionType.Right:
                    dir = (int)DirectionType.Left; break;
                default: break;
            }
        }

        protected void UpdateDirection(int x, int y, DirectionType Direction)
        {
            if (Direction == DirectionType.Unknown || this.x != x || this.y != y)
                dir =
                    this.x > x ? (int)DirectionType.Left :
                    this.x < x ? (int)DirectionType.Right :
                    this.y > y ? (int)DirectionType.Up : (int)DirectionType.Down;
            else
                dir = (int)Direction;
        }

        public virtual void Beep() { }
        public virtual void SpecialAction(ActionType Action) { }
        public virtual void InverseDirection(ActionType Action)
        {
            switch (Action)
            {
                case ActionType.InvDirDown:
                    InverseDirection(DirectionType.Down); break;
                case ActionType.InvDirLeft:
                    InverseDirection(DirectionType.Left); break;
                case ActionType.InvDirUp:
                    InverseDirection(DirectionType.Up); break;
                case ActionType.InvDirRight:
                    InverseDirection(DirectionType.Right); break;
                default: break;
            }
        }
        public virtual void RestartProgram()
        {
            // Первый вызов останавливает программу
            programsData.Run();
            // Второй вызов запускает программу
            programsData.Run();
        }

        public virtual bool HasGun() => World.GetPack(x, y) is Gun;
        public abstract bool Heal();
        public abstract void Hurt(int num, DamageTypePlayer type = DamageTypePlayer.Pure);
        public abstract void Death();
        public abstract bool Move(int x, int y, DirectionType Type = DirectionType.Unknown);
        public abstract void Update();
        public long GetBox(int x, int y)
        {
            var b = Box.GetBox(x, y);
            if (b == null)return 0;
            crys?.BoxCrys(b.bxcrys);
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
                x = (this.x + (dir == 3 ? 3 : dir == 1 ? -3 : 0));
                y = (this.y + (dir == 0 ? 3 : dir == 2 ? -3 : 0));
            }
            return (x, y);
        }
        #region Renders

        public void SendDFToBots(int fx, int fxx, int fxy, int bid, int dir, int col = 0)
        {
            foreach (var chunk in World.W.GetVisibleChunks(x, y))
            {
                foreach (var player in chunk.bots.Select(id => DataBase.GetPlayer(id.Key)))
                {
                    player?.connection?.SendB(new HBPacket([new HBDirectedFXPacket(bid, fxx, fxy, fx, dir, col)]));
                }
            }
        }

        public void SendLeaveBot()
        {
            foreach (var chunk in World.W.GetVisibleChunks(x, y))
            {
                foreach (var player in chunk.bots.Select(id => DataBase.GetPlayer(id.Key)))
                {
                    player?.connection?.SendB(new HBPacket([new HBLeavePacket(id)]));
                }
            }
        }
        #endregion
    }
}
