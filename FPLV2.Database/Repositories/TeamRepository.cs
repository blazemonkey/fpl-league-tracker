using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FPLV2.Database.Repositories;

public class TeamRepository : BaseRepository, ITeamRepository
{
    public TeamRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<Team> GetById(int id)
    {
        var sql = "select * from teams where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<Team>(sql, new { Id = id });
        return result;
    }

    public async Task<Team[]> GetAll()
    {
        var sql = "select * from teams";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Team>(sql);
        return result.ToArray();
    }

    public async Task<Team[]> GetAllBySeasonId(int seasonId)
    {
        var sql = "select * from teams where seasonid = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<Team>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }


    public async Task<bool> DeleteById(int id, SqlConnection conn = null)
    {
        var sql = "delete from teams where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = id });
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }

    public async Task DeleteAll()
    {
        var sql = "delete from teams";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }

    public async Task<int> Insert(Team team, SqlConnection conn = null)
    {
        var sql = "insert into teams (seasonid, teamid, code, name, shortName) output inserted.id values (@SeasonId, @TeamId, @Code, @Name, @ShortName)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, team);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> Update(Team team, SqlConnection conn = null)
    {
        var sql = "update teams set seasonid = @SeasonId, teamid = @TeamId, code = @Code, name = @Name, shortName = @ShortName where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, team);
        var success = result > 0;
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return success;
    }

    public async Task<bool> ReplaceTeamsBySeasonId(Team[] teams, int seasonId)
    {
        var existItems = await GetAllBySeasonId(seasonId);
        var newIds = teams.Select(x => x.TeamId).Except(existItems.Select(x => x.TeamId));
        var oldIds = existItems.Select(x => x.TeamId).Except(teams.Select(x => x.TeamId));
        var sameIds = existItems.Select(x => x.TeamId).Intersect(teams.Select(x => x.TeamId));

        using var conn = await OpenConnection();

        foreach (var id in newIds)
        {
            var team = teams.FirstOrDefault(x => x.TeamId == id);
            if (team == null) continue;

            var result = await Insert(team, conn);
            if (result <= 0)
                return false;
        }

        foreach (var id in oldIds)
        {
            var team = existItems.FirstOrDefault(x => x.TeamId == id);
            if (team == null) continue;

            var result = await DeleteById(team.Id, conn);
            if (result == false)
                return false;
        }

        foreach (var id in sameIds)
        {
            var newTeam = teams.FirstOrDefault(x => x.TeamId == id);
            if (newTeam == null) continue;
            var oldTeam = existItems.FirstOrDefault(x => x.TeamId == id);
            if (oldTeam == null) continue;

            newTeam.Id = oldTeam.Id;
            var hasChanged = JsonSerializer.Serialize(oldTeam) != JsonSerializer.Serialize(newTeam);
            if (hasChanged == false) continue;

            var result = await Update(newTeam, conn);
            if (result == false)
                return false;
        }

        return true;
    }
}
