using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MinesServer.Enums;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Skills;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.World;
using MinesServer.Server;

public class BotSpot : PEntity
{
    public BotSpot() { }
    public BotSpot(int x, int y, Player owner)
    {
        _pdata = new(this);
        id = DataBase.GetNextId();
        this.x = x;
        this.y = y;
        this.owner = owner;
        crys = new Basket(true);
        crys.Changed += Translate;
        MaxHealth = 100;
        Health = 100;

        using var db = new DataBase();
        db.botspots.Remove(this);
        db.SaveChanges();
    }
    public int tail => 1;
    public int skin => 3;
    public override int cid => owner?.cid ?? 0;
    [NotMapped] public Player? owner { get; set; }
    public override Basket crys { get; set; }

    public CrystalCBStorage CrystalCB = new();
    private float cb; // Для дробной части добычи

    private void Translate()
    {
        if (owner is null)
            return;

        try
        {
            using var db = new DataBase();
            var spot = db.spots.FirstOrDefault(s => s.ownerid == owner.id);
            if (spot is null)
                return;

            db.Attach(spot);
            spot.botx = x;
            spot.boty = y;
            spot.basket = crys.serialazed ?? string.Empty;
            db.SaveChanges();
        }
        catch
        {
            // ignore persistence issues for bot state
        }
    }

    public override void Build(string type) { } // Боты не строят

    public override void Bz()
    {
        if (owner is null)
            return;

        // TODO: Пофиксить. Проблема может возникать из-за серрилизации из бд
        if (CrystalCB == null)
        {
            CrystalCB = new CrystalCBStorage();
            Console.WriteLine("CrystalCB был null в Bz(), создан новый");
        }
        // Просто вызываем сервис, опыт начисляется внутри через owner.skillslist
        ResourceExtractionService.PerformDig(this, owner, ref cb, ref CrystalCB, crys);
    }

    public override void Death()
    {
        if (crys.AllCry > 0 && owner is not null)
        {
            var (bx, by) = World.FindEmptyForBox(x, y);
            Box.BuildBox(bx, by, crys.cry, owner, true);
        }
        ResetHp();
        Translate();

        using var db = new DataBase();
        db.botspots.Add(this);
        db.SaveChanges();
    }

    public override void Geo()
    {
        if (owner == null)
            return;

        ResourceExtractionService.PerformGeo(this, owner);
    }

    public override bool Heal()
    {
        if (owner == null)
            return false;

        return ResourceExtractionService.PerformRepair(this, owner);
    }

    public override void Hurt(int damage, DamageTypePlayer type = DamageTypePlayer.Pure)
    {
        if (owner is null)
            return;

        // Обработка опыта через PlayerSkills 
        owner.skillslist.HandleDamageExperience(owner, type, 1);

        // Получение модифицированного урона
        int modifiedDamage = owner.skillslist.HandleDamageReceived(damage);

        if (Health - modifiedDamage > 0)
        {
            Health -= modifiedDamage;
            SendDFToBots(6, 0, 0, id, 0);
        }
        else
        {
            Death();
        }
    }

    public override bool Move(int x, int y, DirectionType Direction = DirectionType.Unknown)
    {
        if (!World.ValidCoord(x, y))
            return false;

        UpdateDirection(x, y, Direction);

        var cell = World.GetCell(x, y);
        if (!World.GetProp(cell).isEmpty)
            return false;

        this.x = x;
        this.y = y;

        if (Vector2.Distance(new Vector2(this.x, this.y), new Vector2(x, y)) < 1.2f)
        {
            owner.skillslist.HandleExperience(owner, SkillType.Movement, 1);
        }
        Translate();
        return true;
    }

    public void SendMyMove()
    {
        World.W.SendBotsInfo(id, x, y, dir, skin, cid, tail);
    }

    public override void Update()
    {
        if (_pdata.ProgRunning)
        {
            SendMyMove();
            _pdata.Step();
        }
    }
}