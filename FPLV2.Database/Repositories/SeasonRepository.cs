using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

public class SeasonRepository : BaseRepository, ISeasonRepository
{
    public SeasonRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<Season> GetById(int id)
    {
        var sql = "select * from seasons where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<Season>(sql, new { Id = id });
        return result;
    }

    public async Task<Season> GetByYear(string year)
    {
        var sql = "select * from seasons where year = @Year";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<Season>(sql, new { Year = year });
        return result;
    }

    public async Task DeleteAll()
    {
        var sql = "delete from seasons";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }

    public async Task<Season[]> GetAll()
    {
        var sql = "select * from seasons";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Season>(sql);
        return result.ToArray();
    }

    public async Task<int> Insert(Season season)
    {
        var sql = "insert into seasons (year) output inserted.id values (@Year)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, season);
        return result;
    }

    public async Task<bool> Update(Season season)
    {
        var sql = "update seasons set year = @Year where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, season);
        var success = result > 0;
        return success;
    }
}
