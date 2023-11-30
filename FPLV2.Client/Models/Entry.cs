using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class Entry
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("player_first_name")]
    public string PlayerFirstName { get; set; }
    [JsonPropertyName("player_last_name")]
    public string PlayerLastName { get; set; }
    public string PlayerName => $"{PlayerFirstName} {PlayerLastName}";
}
