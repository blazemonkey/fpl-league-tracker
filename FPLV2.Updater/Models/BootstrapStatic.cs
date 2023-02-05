using System.Text.Json.Serialization;

namespace FPLV2.Updater.Models;

/// <summary>
/// Class to describe the "Main" JSON value
/// </summary>
public class BootstrapStatic
{
    [JsonPropertyName("events")]
    public Gameweek[] Gameweeks { get; set; }
    public Team[] Teams { get; set; }
    public Element[] Elements { get; set; }
}
