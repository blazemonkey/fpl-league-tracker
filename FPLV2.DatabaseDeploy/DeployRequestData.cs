namespace FPLV2.DatabaseDeploy;

/// <summary>
/// Contains the data that is required when making a POST to the Function
/// </summary>
internal class DeployRequestData
{
    /// <summary>
    /// Gets or sets the name of the database server
    /// </summary>
    public string DatabaseServerName { get; set; }
    /// <summary>
    /// Gets or sets the name of the database
    /// </summary>
    public string DatabaseName { get; set; }
    /// <summary>
    /// Gets or sets the username of the administrator
    /// </summary>
    public string DatabaseAdminUserName { get; set; }
    /// <summary>
    /// Gets or sets the password of the administrator
    /// </summary>
    public string DatabaseAdminPassword { get; set; }
}
