using System.Text.Json.Serialization;

namespace FPLV2.Updater.Models;

public class Player
{
    public int Id { get; set; }
    [JsonPropertyName("event_total")]
    public int EventTotal { get; set; }
    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; }
    public int Rank { get; set; }
    [JsonPropertyName("last_rank")]
    public int LastRank { get; set; }
    [JsonPropertyName("rank_sort")]
    public int RankSort { get; set; }
    public int Total { get; set; }
    public int Entry { get; set; }
    [JsonPropertyName("entry_name")]
    public string EntryName { get; set; }

    public static implicit operator Database.Models.Player(Player player)
    {
        return new Database.Models.Player()
        {
            EntryId = player.Entry,
            PlayerName = player.PlayerName,
            TeamName = player.EntryName
        };
    }
}

public class PlayerResults
{
    [JsonPropertyName("has_next")]
    public bool HasNext { get; set; }
    public int Page { get; set; }
    [JsonPropertyName("results")]
    public Player[] Players { get; set; }
}
