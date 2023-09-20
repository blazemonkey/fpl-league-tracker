namespace FPLV2.Database.Models;

public class Element : BaseModel
{
    public int ElementId { get; set; }
    public int Code { get; set; }
    public int ElementTeamId { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string WebName { get; set; }
    public ElementType ElementType { get; set; }
    public int TeamId { get; set; }
}

public enum ElementType
{
    Goalkeeper = 1,
    Defender = 2,
    Midfielder = 3,
    Forward = 4
}
