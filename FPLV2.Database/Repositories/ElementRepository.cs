using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FPLV2.Database.Repositories;

public class ElementRepository : BaseRepository, IElementRepository
{
    public ElementRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<Element> GetById(int id)
    {
        var sql = "select * from elements where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<Element>(sql, new { Id = id });
        return result;
    }

    public async Task<Element[]> GetAll()
    {
        var sql = "select * from elements";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Element>(sql);
        return result.ToArray();
    }

    public async Task<Element[]> GetAllByTeamId(int teamId)
    {
        var sql = "select * from elements where teamid = @TeamId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Element>(sql, new { TeamId = teamId });
        return result.ToArray();
    }

    public async Task<Element[]> GetAllBySeasonId(int seasonId)
    {
        var sql = "select * from elements e join teams t on t.Id = e.TeamId join seasons s on s.Id = t.SeasonId where s.Id = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Element>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }

    public async Task<bool> DeleteById(int id)
    {
        var sql = "delete from elements where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task DeleteAll()
    {
        var sql = "delete from elements";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }

    public async Task<int> Insert(Element element)
    {
        var sql = "insert into elements (code, teamid, firstname, secondname, webname, elementid, elementteamid, elementtype) output inserted.id values (@Code, @TeamId, @FirstName, @SecondName, @WebName, @ElementId, @ElementTeamId, @ElementType)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, element);
        return result;
    }

    public async Task<bool> Update(Element element)
    {
        var sql = "update elements set code = @Code, teamid = @TeamId, firstname = @FirstName, secondname = @SecondName, webname = @WebName, elementid = @ElementId, elementteamid = @ElementTeamId, elementType = @ElementType where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, element);
        var success = result > 0;
        return success;
    }
}
