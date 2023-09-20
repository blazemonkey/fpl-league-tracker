using Microsoft.Identity.Client;

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
    Line = 1,
    Points = 2
}

public class LineChartPoint
{
    public int X {  get; set;}
    public int Y { get; set;}
}

/// <summary>
/// Class that will hold the data for the elements returned used in the Points Chart
/// </summary>
public class PointsChartElement
{
    public int Id { get; set; }
    public int TeamCode { get; set; }
    public int ElementType { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string WebName { get; set; }
    public int Gameweek { get; set; }
    public int TotalPoints { get; set; }
    public Pick[] Picks { get; set; }
}

/// <summary>
/// Class that will hold the data for the picks returned used in the Points Chart
/// </summary>
public class PointsChartGroupedData
{
    public int Id { get; set; }
    public int TeamCode { get; set; }
    public int ElementType { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string WebName { get; set; }
    public int TotalPoints { get; set; }
    public List<PointsChartValueData> Values { get; set; }
}

public class PointsChartOptions
{
    public bool IgnoreElementsWithNoPicks { get; set; }
    public bool ShowCaptainsOnly { get; set; }    
    public int ElementType { get; set; }
}

public class PointsChartValueData
{
    public int Gameweek { get; set; }
    public int Points { get; set; }
    public Pick[] Picks { get; set; }
}