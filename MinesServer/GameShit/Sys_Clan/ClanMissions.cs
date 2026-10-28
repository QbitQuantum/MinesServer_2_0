using System;
using System.Collections.Generic;
using System.Text;

// Структура миссии — только данные
public readonly struct ClanMission
{
    public string Title { get; } 
    public string Description { get; }
    public string Progress { get; }
    public string Reward { get; }

    public ClanMission(string title, string description, string progress, string reward)
    {
        Title = title;
        Description = description;
        Progress = progress;
        Reward = reward;
    }
}
public static class ClanMissions
{
    // Ключ -> структура миссии
    private static readonly Dictionary<int, ClanMission> _data = new()
    {
        [0] = new ClanMission(
            "<color=white>МИССИЯ \"ЧЕКАНКА\"</color>",
            "Копать валуны смотря наверх",
            "<color=white>830 241 / 862 500</color>",
            "<color=white>+1 СЛОТ ДЛЯ ТЕЛЕПОРТОВ</color>"
        ),
    };

    /// <summary>Строка миссии по ключу. Пустота, если нет.</summary>
    public static string GetMission(int id) =>
        _data.TryGetValue(id, out var m) ? GetString(m) : "";

    /// <summary>Формирует итоговую строку для вывода.</summary>
    public static string GetString(ClanMission m)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ПРОКАЧКА КЛАНА: {m.Title}");
        sb.AppendLine($"ЧТО ДЕЛАТЬ: {m.Description}");
        sb.AppendLine($"ВЫПОЛНЕНО: {m.Progress}");
        sb.Append($"НАГРАДА: {m.Reward}");
        return sb.ToString();
    }
}