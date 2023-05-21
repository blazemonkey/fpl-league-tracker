namespace FPLV2.Database.Models;

public class Chart : BaseModel
{
    public ChartType Type { get; set; }
    public GraphType GraphType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool Active { get; set; }
}
public enum ChartType
{
    Overall = 1,
    Team = 2
}

public enum GraphType
{
    Line = 1
}

public class LineChartPoint
{
    public int X {  get; set;}
    public int Y { get; set;}
}