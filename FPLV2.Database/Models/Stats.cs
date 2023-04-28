namespace FPLV2.Database.Models;

public class Stats : BaseModel
{
    public StatType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StoredProcedureName { get; set; }
    public int DisplayOrder { get; set; }
    public bool Active { get; set; }
}
public enum StatType
{
    Overall = 1,
    Team = 2
}