namespace FPLV2.Database.Models;

public class Pick : BaseModel
{
    public int PlayerId { get; set; }
    public int Gameweek { get; set; }
    public int ElementId { get; set; }
    public int Multiplier { get; set; }
    public int Position { get; set; }
}
