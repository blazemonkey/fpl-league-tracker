using System.Text.Json.Serialization;

namespace FPLV2.Updater.Models;

public class Team
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("short_name")]
    public string ShortName { get; set; }

    public static implicit operator Database.Models.Team(Team team)
    {
        return new Database.Models.Team()
        {
            TeamId = team.Id,
            Code = team.Code,
            Name = team.Name,
            ShortName = team.ShortName
        };
    }
}
