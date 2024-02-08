using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FPLV2.DatabaseDeploy;

public class DeployFunction
{
    public readonly string DATABASE_VERSION = "103";

    private readonly ILogger _logger;

    public DeployFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DeployFunction>();
    }

    [Function("DeployFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("Deploy Function began processing a request.");

        var sw = new Stopwatch();
        sw.Start();        

        var data = req.Body.Length == 0 ? new DeployRequestData() : await JsonSerializer.DeserializeAsync<DeployRequestData>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        var requestDataError = CheckRequestData(data);
        if (string.IsNullOrEmpty(requestDataError) == false)
        {
            _logger.LogError(requestDataError);
            return await WriteResponse(req, HttpStatusCode.BadRequest, false, sw.Elapsed, requestDataError);
        }

        using var conn = await OpenSqlConnection(data.DatabaseServerName, data.DatabaseName, data.DatabaseAdminUserName, data.DatabaseAdminPassword);
        if (conn == null)
        {
            var error = "Could not open a connection to the Database Server";
            _logger.LogError(error);
            return await WriteResponse(req, HttpStatusCode.BadRequest, false, sw.Elapsed, error);
        }

        var trans = conn.BeginTransaction();

        var createError = await CreateDatabase(conn, trans);
        if (string.IsNullOrEmpty(createError) == false)
        {
            _logger.LogError(createError);
            await trans.RollbackAsync();
            return await WriteResponse(req, HttpStatusCode.InternalServerError, false, sw.Elapsed, createError);
        }

        var upgradeError = await UpgradeDatabase(conn, trans);
        if (string.IsNullOrEmpty(upgradeError) == false)
        {
            _logger.LogError(upgradeError);
            await trans.RollbackAsync();
            return await WriteResponse(req, HttpStatusCode.InternalServerError, false, sw.Elapsed, upgradeError);
        }

        await trans.CommitAsync();
        _logger.LogInformation("Deploy Function successfuly processed a request.");
        return await WriteResponse(req, HttpStatusCode.OK, true, sw.Elapsed);
    }

    /// <summary>
    /// Check if the passed in body to the POST Function contains all the required data 
    /// </summary>
    /// <param name="req">Request to check</param>
    /// <returns>Empty if no properties are missing, otherwise the error message to return</returns>
    private static string CheckRequestData(DeployRequestData req)
    {
        var missingProperties = new List<string>();
        if (string.IsNullOrEmpty(req.DatabaseServerName))
            missingProperties.Add(nameof(DeployRequestData.DatabaseServerName));
        if (string.IsNullOrEmpty(req.DatabaseName))
            missingProperties.Add(nameof(DeployRequestData.DatabaseName));
        if (string.IsNullOrEmpty(req.DatabaseAdminUserName))
            missingProperties.Add(nameof(DeployRequestData.DatabaseAdminUserName));
        if (string.IsNullOrEmpty(req.DatabaseAdminPassword))
            missingProperties.Add(nameof(DeployRequestData.DatabaseAdminPassword));

        if (missingProperties.Any() == false)
            return string.Empty;

        return $"The following properties are missing from the POST Body Request: {(string.Join(", ", missingProperties.Select(x => "'" + x + "'")))}";
    }

    /// <summary>
    /// Open a connection to the database
    /// </summary>
    /// <param name="serverName">Name of the database server</param>
    /// <param name="databaseName">Name of the database</param>
    /// <param name="adminName">User name of the admin</param>
    /// <param name="adminPassword">Password of the admin</param>
    /// <returns>A SqlConnection object to use to make queries to, or null if connection could not be made</returns>
    private static async Task<SqlConnection> OpenSqlConnection(string serverName, string databaseName, string adminName, string adminPassword)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                InitialCatalog = databaseName,
                UserID = adminName,
                Password = adminPassword
            };

            var connectionString = builder.ConnectionString;
            var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            return conn;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Re-create the initial tables and stored procedures for a database. The scripts themselves should ensure that if a table already exists, it won't be created again
    /// </summary>
    /// <param name="conn">Used to make connection to the database</param>
    /// <param name="trans">Used to rollback if any errors occur during the deployment</param>
    /// <returns>Empty if no errors occured, otherwise the error message</returns>
    private async Task<string> CreateDatabase(SqlConnection conn, SqlTransaction trans)
    {
        var success = await ExecuteScript(Path.Combine("Scripts", "CreateTables.sql"), conn, trans);
        if (success == false) return "Could not re-create the database tables";

        success = await ExecuteScript(Path.Combine("Scripts", "InitialData.sql"), conn, trans);
        if (success == false) return "Could not insert the initial data";

        success = await ExecuteScript(Path.Combine("Scripts", "Programmability", "CreateStoredProcedures.sql"), conn, trans);
        if (success == false) return "Could not re-create the stored procedures";

        // insert the GetVersion Stored Procedure if it doesn't already exist
        var cmd = new SqlCommand($"IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'GetVersion')\r\nBEGIN\r\n\texec('CREATE PROCEDURE GetVersion AS BEGIN SELECT {DATABASE_VERSION} AS Version END -- this should be override in the deploy code')\r\nEND\r\n", conn, trans);
        await cmd.ExecuteNonQueryAsync();

        return string.Empty;
    }

    /// <summary>
    /// Upgrade the database
    /// </summary>
    /// <param name="conn">Used to make connection to the database</param>
    /// <param name="trans">Used to rollback if any errors occur during the deployment</param>
    /// <returns>Empty if no errors occured, otherwise the error message</returns>
    private async Task<string> UpgradeDatabase(SqlConnection conn, SqlTransaction trans)
    {
        var upgradeScripts = Directory.GetFiles(Path.Combine("Scripts", "Upgrades"));
        var latestVersion = int.Parse(DATABASE_VERSION);

        var cmd = new SqlCommand("EXEC [GetVersion]", conn, trans);
        var databaseVersion = (int)(await cmd.ExecuteScalarAsync());

        foreach (var script in upgradeScripts)
        {
            var success = int.TryParse(Path.GetFileNameWithoutExtension(script), out int scriptVersion);
            if (success == false)
                continue;

            if (databaseVersion > scriptVersion || databaseVersion == latestVersion)
                continue;

            success = await ExecuteScript(script, conn, trans);
            if (success == false)
                return $"An error occured when attempting to run the upgrade script {scriptVersion}";
        }

        return string.Empty;

    }

    /// <summary>
    /// Execute the individual commands that are in a SQL script
    /// </summary>
    /// <param name="path">Path of the script</param>
    /// <param name="conn">Used to make connection to the database</param>
    /// <param name="trans">Used to rollback if any errors occur during the deployment</param>
    /// <returns>True if success, false if error occurs</returns>
    private static async Task<bool> ExecuteScript(string path, SqlConnection conn, SqlTransaction trans)
    {
        try
        {
            var script = await File.ReadAllTextAsync(path);
            if (string.IsNullOrEmpty(script))
                return false;

            var commands = script.Split("GO", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToArray();
            foreach (var c in commands)
            {
                var cmd = new SqlCommand(c, conn, trans);
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Create a Http Response depending on if the Function was successful or not
    /// </summary>
    /// <param name="req">Used to create the HttpResponseData</param>
    /// <param name="statusCode">Status code of the repsonse</param>
    /// <param name="success">If the Function succeeded</param>
    /// <param name="timeTaken">Time taken for the Function execute</param>
    /// <param name="errorMessage">Any error message if the Function was not successful</param>
    /// <returns>Http response returned back to caller</returns>
    private static async Task<HttpResponseData> WriteResponse(HttpRequestData req, HttpStatusCode statusCode, bool success, TimeSpan timeTaken, string errorMessage = null)
    {
        var response = req.CreateResponse();
        response.StatusCode = statusCode;

        var responseData = new DeployResponseData
        {
            Success = success,
            TimeTaken = timeTaken,
            ErrorMessage = errorMessage
        };

        await response.WriteAsJsonAsync(responseData);
        return response;
    }
}
