using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Skills
{
    public class SkillConflict
    {
        /// <summary>
        /// Навык, который конфликтует
        /// </summary>
        public SkillType Skill { get; set; }

        /// <summary>
        /// С кем конфликтует
        /// </summary>
        public SkillType ConflictsWith { get; set; }
    }

    public static class SkillConflicts
    {
        private static readonly List<SkillConflict> _conflicts = new()
        {
            // Разрушение
            new SkillConflict { Skill = SkillType.TotalDestruction, ConflictsWith = SkillType.Destruction},
            
            // Стройка
            new SkillConflict { Skill = SkillType.BuildUniversal, ConflictsWith = SkillType.BuildGreen},
            new SkillConflict { Skill = SkillType.BuildUniversal, ConflictsWith = SkillType.BuildYellow},
            new SkillConflict { Skill = SkillType.BuildUniversal, ConflictsWith = SkillType.BuildRed},

            new SkillConflict { Skill = SkillType.Architecture, ConflictsWith = SkillType.BuildStructure},
            new SkillConflict { Skill = SkillType.Architecture, ConflictsWith = SkillType.BuildRoad},
            new SkillConflict { Skill = SkillType.Architecture, ConflictsWith = SkillType.BuildQuadro},
            
            // Добыча
            new SkillConflict { Skill = SkillType.ExpertMining, ConflictsWith = SkillType.MineGeneral},

        };

        /// <summary>
        /// Проверяет, конфликтуют ли два навыка
        /// </summary>
        public static bool HasConflict(SkillType skill1, SkillType skill2)
        {
            return _conflicts.Any(c =>
                c.Skill == skill1 && c.ConflictsWith == skill2 ||
                c.Skill == skill2 && c.ConflictsWith == skill1);
        }

        /// <summary>
        /// Получает все конфликты для навыка
        /// </summary>
        public static IEnumerable<SkillType> GetConflictsFor(SkillType skill)
        {
            return _conflicts
                .Where(c => c.Skill == skill)
                .Select(c => c.ConflictsWith);
        }

        /// <summary>
        /// Проверяет, можно ли изучить навык при текущих навыках
        /// </summary>
        public static SkillType? CanLearn(
            SkillType newSkill,
            IEnumerable<SkillType> currentSkills)
        {
            foreach (var currentSkill in currentSkills)
            {
                if (HasConflict(newSkill, currentSkill))
                {
                    return currentSkill;
                }
            }
            return null;
        }
    }
}
