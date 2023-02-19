using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

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
        var sql = "select e.* from elements e join teams t on t.Id = e.TeamId join seasons s on s.Id = t.SeasonId where s.Id = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Element>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }

    public async Task<bool> DeleteById(int id, SqlConnection conn = null)
    {
        var sql = "delete from elements where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = id });
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }

    public async Task DeleteAll()
    {
        var sql = "delete from elements";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }

    public async Task<int> Insert(Element element, SqlConnection conn = null)
    {
        var sql = "insert into elements (code, teamid, firstname, secondname, webname, elementid, elementteamid, elementtype) output inserted.id values (@Code, @TeamId, @FirstName, @SecondName, @WebName, @ElementId, @ElementTeamId, @ElementType)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, element);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> Update(Element element, SqlConnection conn = null)
    {
        var sql = "update elements set code = @Code, teamid = @TeamId, firstname = @FirstName, secondname = @SecondName, webname = @WebName, elementid = @ElementId, elementteamid = @ElementTeamId, elementType = @ElementType where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, element);
        var success = result > 0;
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return success;
    }
    public async Task<bool> ReplaceElementsBySeasonId(Element[] elements, int seasonId)
    {
        var existItems = await GetAllBySeasonId(seasonId);
        var newIds = elements.Select(x => x.ElementId).Except(existItems.Select(x => x.ElementId));
        var oldIds = existItems.Select(x => x.ElementId).Except(elements.Select(x => x.ElementId));
        var sameIds = existItems.Select(x => x.ElementId).Intersect(elements.Select(x => x.ElementId));

        using var conn = await OpenConnection();

        foreach (var id in newIds)
        {
            var element = elements.FirstOrDefault(x => x.ElementId == id);
            if (element == null) continue;

            var result = await Insert(element, conn);
            if (result <= 0)
                return false;
        }

        foreach (var id in oldIds)
        {
            var element = existItems.FirstOrDefault(x => x.ElementId == id);
            if (element == null) continue;

            var result = await DeleteById(element.Id, conn);
            if (result == false)
                return false;
        }

        foreach (var id in sameIds)
        {
            var newElement = elements.FirstOrDefault(x => x.ElementId == id);
            if (newElement == null) continue;
            var oldElement = existItems.FirstOrDefault(x => x.ElementId == id);
            if (oldElement == null) continue;

            newElement.Id = oldElement.Id;
            var hasChanged = JsonSerializer.Serialize(oldElement) != JsonSerializer.Serialize(newElement);
            if (hasChanged == false) continue;

            var result = await Update(newElement, conn);
            if (result == false)
                return false;
        }

        return true;
    }
}
