using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class Pick
{
    [JsonPropertyName("element")]
    public int Element { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("multiplier")]
    public int Multiplier { get; set; }

    [JsonPropertyName("is_captain")]
    public bool IsCaptain { get; set; }

    [JsonPropertyName("is_vice_captain")]
    public bool IsViceCaptain { get; set; }

    public static implicit operator Database.Models.Pick(Pick pick)
    {
        return new Database.Models.Pick()
        {
            ElementId = pick.Element,
            Multiplier = pick.Multiplier,
            Position = pick.Position
        };
    }
}

public class PickRoot
{
    [JsonPropertyName("active_chip")]
    public object ActiveChip { get; set; }

    [JsonPropertyName("automatic_subs")]
    public AutomaticSub[] AutomaticSubs { get; set; }

    [JsonPropertyName("entry_history")]
    public EntryHistory EntryHistory { get; set; }

    [JsonPropertyName("picks")]
    public Pick[] Picks { get; set; }
}
