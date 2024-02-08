namespace FPLV2.Database.Models;

public class Points : BaseModel
{
    public int PlayerId { get; set; }
    public int Gameweek { get; set; }
    public int GameweekPoints { get; set; }
    public int GameweekPointsOnBench { get; set; }
    public int Total { get; set; }
}