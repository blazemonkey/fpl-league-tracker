namespace FPLV2.Database.Models;

public class ElementStat : BaseModel
{
    public int ElementId { get; set; }
    public int ApiElementId { get; set; }
    public int Gameweek { get; set; }
    public int Minutes { get; set; }
    public int GoalsScored { get; set; }
    public int Assists { get; set; }
    public int CleanSheets { get; set; }
    public int GoalsConceded { get; set; }
    public int OwnGoals { get; set; }
    public int PenaltiesSaved { get; set; }
    public int PenaltiesMissed { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int Saves { get; set; }
    public int Bonus { get; set; }
    public int Bps { get; set; }
    public string Influence { get; set; }
    public string Creativity { get; set; }
    public string Threat { get; set; }
    public string IctIndex { get; set; }
    public int TotalPoints { get; set; }
    public bool InDreamteam { get; set; }
}
