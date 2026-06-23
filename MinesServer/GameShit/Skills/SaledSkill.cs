using MinesServer.Enums;

namespace MinesServer.GameShit.Skills
{
    public readonly struct SaledSkill(int Lvl, bool IsUp, SkillType Type)
    {
        public int Lvl { get; } = Lvl;
        public bool IsUp { get; } = IsUp;
        public SkillType Type { get; } = Type;
    }
}