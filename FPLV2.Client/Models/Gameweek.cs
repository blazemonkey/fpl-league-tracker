using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class Gameweek
{
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("deadline_time")]
    public DateTime DeadlineTime { get; set; }
    [JsonPropertyName("average_entry_score")]
    public int AverageEntryScore { get; set; }
    public bool Finished { get; set; }
    [JsonPropertyName("data_checked")]
    public bool DataChecked { get; set; }
    [JsonPropertyName("highest_scoring_entry")]
    public int? HighestScoringEntry { get; set; }
    [JsonPropertyName("deadline_time_epoch")]
    public int DeadlineTimeEpoch { get; set; }
    [JsonPropertyName("deadline_time_game_offset")]
    public int DeadlineTimeGameOffset { get; set; }
    [JsonPropertyName("highest_score")]
    public int? HighestScore { get; set; }
    [JsonPropertyName("is_previous")]
    public bool IsPrevious { get; set; }
    [JsonPropertyName("is_current")]
    public bool IsCurrent { get; set; }
    [JsonPropertyName("is_next")]
    public bool IsNext { get; set; }
    [JsonPropertyName("chip_plays")]
    public object[] ChipPlays { get; set; }
    [JsonPropertyName("most_selected")]
    public int? MostSelected { get; set; }
    [JsonPropertyName("most_transferred_in")]
    public int? MostTransferredIn { get; set; }
    [JsonPropertyName("top_element")]
    public int? TopElement { get; set; }
    [JsonPropertyName("top_element_info")]
    public object TopElementInfo { get; set; }
    [JsonPropertyName("transfers_made")]
    public int? TransfersMade { get; set; }
    [JsonPropertyName("most_captained")]
    public int? MostCaptained { get; set; }
    [JsonPropertyName("most_vice_captained")]
    public int? MostViceCaptained { get; set; }
}
