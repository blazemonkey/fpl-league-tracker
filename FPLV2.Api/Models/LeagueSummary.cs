using FPLV2.Database.Models;

namespace FPLV2.Api.Models;

public class LeagueSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Player[] Players { get; set; }
    public Team[] Teams { get; set; }
}
