namespace FPLV2.Database.Models;

public class LeagueSearch : BaseModel
{
    public int LeagueId { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; }
    public string LeagueType { get; set; }
    public DateTime CreatedTimeUtc { get; set; }
    public int? AdminEntryId { get; set; }
    public string AdminPlayerName { get; set; }
}
