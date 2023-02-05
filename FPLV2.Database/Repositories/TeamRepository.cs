using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    public async Task<bool> DeleteById(int id)
    {
        var sql = "delete from teams where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task DeleteAll()
    {
        var sql = "delete from teams";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }

    public async Task<int> Insert(Team team)
    {
        var sql = "insert into teams (seasonid, teamid, code, name, shortName) output inserted.id values (@SeasonId, @TeamId, @Code, @Name, @ShortName)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, team);
        return result;
    }

    public async Task<bool> Update(Team team)
    {
        var sql = "update teams set seasonid = @SeasonId, teamid = @TeamId, code = @Code, name = @Name, shortName = @ShortName where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, team);
        var success = result > 0;
        return success;
    }
}
