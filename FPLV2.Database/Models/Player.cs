namespace FPLV2.Database.Models;

public class Player : BaseModel
{
    public int LeagueId { get; set; }
    public int EntryId { get; set; }
    public string PlayerName { get; set; }
    public string TeamName { get; set; }
}
