namespace FPLV2.DatabaseDeploy;

/// <summary>
/// Data that is returned by the POST Function
/// </summary>
internal class DeployResponseData
{
    /// <summary>
    /// Gets or sets if the Function was successful
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Gets or sets the time taken to execute the Function
    /// </summary>
    public TimeSpan TimeTaken { get; set; }
    /// <summary>
    /// Gets or sets the error message if the Function was not successful
    /// </summary>
    public string ErrorMessage { get; set; }
}
