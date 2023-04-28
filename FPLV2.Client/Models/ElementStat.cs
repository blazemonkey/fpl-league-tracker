using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class ElementStat
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("stats")]
    public Stats Stats { get; set; }
}

public class Stats
{
    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }

    [JsonPropertyName("goals_scored")]
    public int GoalsScored { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("clean_sheets")]
    public int CleanSheets { get; set; }

    [JsonPropertyName("goals_conceded")]
    public int GoalsConceded { get; set; }

    [JsonPropertyName("own_goals")]
    public int OwnGoals { get; set; }

    [JsonPropertyName("penalties_saved")]
    public int PenaltiesSaved { get; set; }

    [JsonPropertyName("penalties_missed")]
    public int PenaltiesMissed { get; set; }

    [JsonPropertyName("yellow_cards")]
    public int YellowCards { get; set; }

    [JsonPropertyName("red_cards")]
    public int RedCards { get; set; }

    [JsonPropertyName("saves")]
    public int Saves { get; set; }

    [JsonPropertyName("bonus")]
    public int Bonus { get; set; }

    [JsonPropertyName("bps")]
    public int Bps { get; set; }

    [JsonPropertyName("influence")]
    public string Influence { get; set; }

    [JsonPropertyName("creativity")]
    public string Creativity { get; set; }

    [JsonPropertyName("threat")]
    public string Threat { get; set; }

    [JsonPropertyName("ict_index")]
    public string IctIndex { get; set; }

    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }

    [JsonPropertyName("in_dreamteam")]
    public bool InDreamteam { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("value")]
    public int Value { get; set; }


    public static implicit operator Database.Models.ElementStat(Stats stats)
    {
        return new Database.Models.ElementStat()
        {
            Assists = stats.Assists,
            Bonus = stats.Bonus,
            Bps = stats.Bps,
            CleanSheets = stats.CleanSheets,
            Creativity = stats.Creativity,
            GoalsConceded = stats.GoalsConceded,
            GoalsScored = stats.GoalsScored,
            IctIndex = stats.IctIndex,
            InDreamteam = stats.InDreamteam,
            Influence = stats.Influence,
            Minutes = stats.Minutes,
            OwnGoals = stats.OwnGoals,
            PenaltiesMissed = stats.PenaltiesMissed,
            PenaltiesSaved = stats.PenaltiesSaved,
            Saves = stats.Saves,
            Threat = stats.Threat,
            TotalPoints = stats.TotalPoints,
            YellowCards = stats.YellowCards            
        };
    }
}

public class ElementStatRoot
{
    [JsonPropertyName("elements")]
    public ElementStat[] Elements { get; set; }
}

