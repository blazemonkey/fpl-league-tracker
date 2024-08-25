using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class Points
{
    [JsonPropertyName("event")]
    public int Event { get; set; }

    [JsonPropertyName("points")]
    public int EventPoints { get; set; }

    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }

    [JsonPropertyName("rank")]
    public int? Rank { get; set; }

    [JsonPropertyName("rank_sort")]
    public int? RankSort { get; set; }

    [JsonPropertyName("overall_rank")]
    public int OverallRank { get; set; }

    [JsonPropertyName("bank")]
    public int Bank { get; set; }

    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("event_transfers")]
    public int EventTransfers { get; set; }

    [JsonPropertyName("event_transfers_cost")]
    public int EventTransfersCost { get; set; }

    [JsonPropertyName("points_on_bench")]
    public int PointsOnBench { get; set; }

    public static implicit operator Database.Models.Points(Points points)
    {
        return new Database.Models.Points()
        {
            Gameweek = points.Event,
            GameweekPoints = points.EventPoints,
            GameweekPointsOnBench = points.PointsOnBench,
            Total = points.TotalPoints
        };
    }
}

public class PointsRoot
{
    [JsonPropertyName("current")]
    public Points[] Current { get; set; }
}