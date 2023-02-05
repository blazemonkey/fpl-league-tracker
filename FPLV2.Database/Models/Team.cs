namespace FPLV2.Database.Models;

public class Team : BaseModel
{
    public int SeasonId { get; set; }
    public int TeamId { get; set; }
    public int Code { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
}
