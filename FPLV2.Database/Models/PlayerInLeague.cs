namespace FPLV2.Database.Models;

/// <summary>
/// Class that represents the players_in_leagues table. Used because a player can belong to multiple leagues
/// </summary>
public class PlayerInLeague : BaseModel
{
    /// <summary>
    /// Gets or sets the player id from the players table
    /// </summary>
    public int PlayerId { get; set; }
    /// <summary>
    /// Gets or sets the league id from the leagues table
    /// </summary>
    public int LeagueId { get; set; }
}
