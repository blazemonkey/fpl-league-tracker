namespace FPLV2.Database.Models;

/// <summary>
/// Class that represents the logging table
/// </summary>
public class Logging : BaseModel
{
    /// <summary>
    /// Gets or sets the type of logging this is
    /// </summary>
    public LogType Type { get; set; }
    /// <summary>
    /// Gets or sets the logging message
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Gets or sets the logging time
    /// </summary>
    public DateTime LogTimeUtc { get; set; }

    /// <summary>
    /// Constructor to automatically set the log time to current utc time
    /// </summary>
    public Logging()
    {
        LogTimeUtc = DateTime.UtcNow;
    }
}

public enum LogType
{
    Debug = 1,
    Information,
    Warning,
    Error
}
