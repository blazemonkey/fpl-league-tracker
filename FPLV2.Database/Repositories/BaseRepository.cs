using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

public class BaseRepository
{
    protected IConfiguration Configuration { get; private init; }
    public BaseRepository(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected async Task<SqlConnection> OpenConnection()
    {
        var connString = GetConnectionString();
        var conn = new SqlConnection(connString);
        await conn.OpenAsync();
        return conn;
    }
    protected string GetConnectionString()
    {
        var connString = Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connString))
            throw new Exception("No Connection String found");

        return connString;
    }
}