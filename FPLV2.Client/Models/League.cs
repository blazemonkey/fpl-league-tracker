using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class League
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public bool Closed { get; set; }
    [JsonPropertyName("max_entries")]
    public int? MaxEntries { get; set; }
    [JsonPropertyName("league_type")]
    public string LeagueType { get; set; }
    public string Scoring { get; set; }
    [JsonPropertyName("admin_entry")]
    public int? AdminEntry { get; set; }
    [JsonPropertyName("start_event")]
    public int StartEvent { get; set; }
    [JsonPropertyName("code_privacy")]
    public string CodePrivacy { get; set; }
    public int? Rank { get; set; }
}
