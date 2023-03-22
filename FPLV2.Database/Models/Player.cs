using System.Text.Json.Serialization;

namespace FPLV2.Database.Models;

public class Player : BaseModel
{
    /// <summary>
    /// Gets or sets the id of the League
    /// This column was removed from the players table and moved into a players_in_leagues mapping table because a player can belong to multiple leagues
    /// This property remains so we can pass the league id in the player object still
    /// Need to JsonIgnore it so when we serialize it to compare, it doesn't get used
    /// </summary>
    [JsonIgnore]
    public int LeagueId { get; set; }
    public int EntryId { get; set; }
    public string PlayerName { get; set; }
    public string TeamName { get; set; }
}
